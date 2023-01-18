// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Autofac;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Subscriber.Middleware;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Subscriber
{
    /// <summary>
    /// Generic rabbitMq subscriber
    /// </summary>
    [PublicAPI]
    public class RabbitMqSubscriber<TTopicModel> : IStartStop
    {
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILogger<RabbitMqSubscriber<TTopicModel>> _logger;
        private readonly MiddlewareQueue<TTopicModel> _middlewareQueue;
        private readonly RetryPolicy _retryPolicy;
        private readonly List<Action<IDictionary<string, object>>> _readHeadersActions = new List<Action<IDictionary<string, object>>>();
        private readonly IAutorecoveringConnection _connection;
        
        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;
        private ushort? _prefetchCount;
        // TODO: introduce domain channel model to make interaction with RabbitMQ safe and reliable with implicit retries
        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private string _consumerTag;
        
        public IMessageDeserializer<TTopicModel> MessageDeserializer { get; private set; }
        public IMessageReadStrategy MessageReadStrategy { get; private set; }
        public Func<TTopicModel, Task> EventHandler { get; private set; }
        public Func<TTopicModel, CancellationToken, Task> CancellableEventHandler { get; private set; }

        public RabbitMqSubscriber(
            [NotNull] ILogger<RabbitMqSubscriber<TTopicModel>> logger,
            [NotNull] RabbitMqSubscriptionSettings settings,
            [NotNull] IAutorecoveringConnection connection)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _middlewareQueue = new MiddlewareQueue<TTopicModel>(settings);

            // move to retry policy provider
            _retryPolicy = Policy
                .Handle<OperationInterruptedException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetryForever(attempt => TimeSpan.FromSeconds(Math.Min(Math.Pow(2, attempt), 30)),
                    (exception, retryCount, timeSpan) =>
                    {
                        if (exception is OperationInterruptedException operationInterruptedException)
                        {
                            _logger.LogWarning(
                                "The operation was interrupted with reason {ReasonCode}:{ReasonText}. Trying to reconnect for the {RetryCount} time in {Period}",
                                operationInterruptedException.ShutdownReason.ReplyCode,
                                operationInterruptedException.ShutdownReason.ReplyText,
                                retryCount,
                                timeSpan);
                        }
                        
                        if (exception is BrokerUnreachableException)
                        {
                            _logger.LogWarning(
                                "The broker is unreachable. Trying to reconnect for the {RetryCount} time in {Period}",
                                retryCount,
                                timeSpan);
                        }
                    });
        }

        #region Configurator

        public RabbitMqSubscriber<TTopicModel> SetMessageDeserializer(
            IMessageDeserializer<TTopicModel> messageDeserializer)
        {
            MessageDeserializer = messageDeserializer;
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> Subscribe(Func<TTopicModel, Task> callback)
        {
            EventHandler = callback;
            CancellableEventHandler = null;
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> Subscribe(Func<TTopicModel, CancellationToken, Task> callback)
        {
            CancellableEventHandler = callback;
            EventHandler = null;
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> SetMessageReadStrategy(IMessageReadStrategy messageReadStrategy)
        {
            MessageReadStrategy = messageReadStrategy;
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> UseDefaultStrategy()
        {
            SetMessageReadStrategy(new MessageReadWithTemporaryQueueStrategy());
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> UseMiddleware(IEventMiddleware<TTopicModel> middleware)
        {
            if (_consumer?.IsRunning ?? false)
                throw new InvalidOperationException("New middleware can't be added after subscriber Start");

            _middlewareQueue.AddMiddleware(middleware ?? throw new ArgumentNullException());
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> SetPrefetchCount(ushort prefetchCount)
        {
            _prefetchCount = prefetchCount;
            return this;
        }
        
        public RabbitMqSubscriber<TTopicModel> SetReadHeadersAction(Action<IDictionary<string, object>> action)
        {
            if (action != null)
            {
                _readHeadersActions.Add(action);
            }
            return this;
        }


        #endregion
        
        private void OnReceived(object sender, BasicDeliverEventArgs args)
        {
            _retryPolicy.Execute(() =>
            {
                var acceptor = new MessageAcceptor(_channel, args.DeliveryTag);

                _readHeadersActions.ForEach(x => x(args.BasicProperties?.Headers));
            
                try
                {
                    var model = MessageDeserializer.Deserialize(args.Body.ToArray());

                    _middlewareQueue.RunMiddlewaresAsync(
                            args.Body,
                            args.BasicProperties,
                            model,
                            acceptor,
                            _cancellationTokenSource.Token)
                        .GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, _settings.GetSubscriberName());

                    acceptor.Reject();
                }
            });
        }

        void IStartable.Start()
        {
            Start();
        }

        public RabbitMqSubscriber<TTopicModel> Start()
        {
            if (_consumer?.IsRunning ?? false)
                return this;
            
            if (MessageDeserializer == null)
                throw new InvalidOperationException("Please, specify message deserializer");

            if (EventHandler == null && CancellableEventHandler == null)
                throw new InvalidOperationException("Please, specify message handler");

            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            if (MessageReadStrategy == null)
                UseDefaultStrategy();

            var actualHandlerMiddleware = EventHandler != null
                ? new ActualHandlerMiddleware<TTopicModel>(EventHandler)
                : new ActualHandlerMiddleware<TTopicModel>(CancellableEventHandler);
            _middlewareQueue.AddMiddleware(actualHandlerMiddleware);

            _channel ??= _retryPolicy.Execute(_connection.CreateModel);

            _consumer = new EventingBasicConsumer(_channel);

            // TODO: move to read strategy
            if (_prefetchCount.HasValue)
            {
                _retryPolicy.Execute(() => _channel.BasicQos(0, _prefetchCount.Value, false));
            }

            var queueName = _retryPolicy.Execute(() => MessageReadStrategy.Configure(_settings, _channel));

            _consumer.Received += OnReceived;

            _consumerTag = _retryPolicy.Execute(() => _channel.BasicConsume(queueName, false, _consumer));

            return this;
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();

            if (_consumer.IsRunning)
            {
                _retryPolicy.Execute(() => _channel.BasicCancel(_consumerTag));
                _consumer.Received -= OnReceived;
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Stop();
            
            _channel?.Close();
            _channel?.Dispose();
            _channel = null;

            _disposed = true;
        }
    }
}
