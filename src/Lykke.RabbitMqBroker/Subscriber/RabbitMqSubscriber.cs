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
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Subscriber
{
    /// <summary>
    /// Generic Rabbit MQ subscriber
    /// Not thread-safe
    /// </summary>
    [PublicAPI]
    public sealed partial class RabbitMqSubscriber<TTopicModel> : IStartStop
    {
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILogger<RabbitMqSubscriber<TTopicModel>> _logger;
        private readonly MiddlewareQueue<TTopicModel> _middlewareQueue;
        private readonly List<Action<IDictionary<string, object>>> _readHeadersActions = new();
        private readonly IAutorecoveringConnection _connection;

        private CancellationTokenSource _cancellationTokenSource = new();
        private bool _disposed;
        private ushort? _prefetchCount;

        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private string _consumerTag;
        private Task _startTask;

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
            SetMessageReadStrategy(GetDefaultStrategy());
            return this;
        }

        public RabbitMqSubscriber<TTopicModel> UseMiddleware(IEventMiddleware<TTopicModel> middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));

            if (_consumer is { IsRunning: true })
                throw new InvalidOperationException("New middleware can't be added after subscriber Start");

            _middlewareQueue.AddMiddleware(middleware);
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
            // make a copy of the body, as it can be released at any time
            var bodyCopy = new byte[args.Body.Length];
            Buffer.BlockCopy(args.Body.ToArray(), 0, bodyCopy, 0, args.Body.Length);

            var acceptor = new MessageAcceptor(_channel, args.DeliveryTag);
            _readHeadersActions.ForEach(x => x(args.BasicProperties?.Headers));

            try
            {
                var model = MessageDeserializer.Deserialize(bodyCopy);
                _middlewareQueue.RunMiddlewaresAsync(
                        bodyCopy,
                        args.BasicProperties,
                        model,
                        acceptor,
                        _cancellationTokenSource.Token)
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, _settings.GetSubscriberDisplayName());
                acceptor.Reject();
            }
        }

        void IStartable.Start()
        {
            Start();
        }

        public RabbitMqSubscriber<TTopicModel> Start()
        {
            StartAsync().GetAwaiter().GetResult();
            return this;
        }

        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (_consumer is { IsRunning: true })
                return Task.CompletedTask;

            CheckStartPreConditionsOrThrow();

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            MessageReadStrategy ??= GetDefaultStrategy();

            var actualHandlerMiddleware = EventHandler != null
                ? new ActualHandlerMiddleware<TTopicModel>(EventHandler)
                : new ActualHandlerMiddleware<TTopicModel>(CancellableEventHandler);
            _middlewareQueue.AddMiddleware(actualHandlerMiddleware);

            _channel = GetOrCreateConsumerChannel();

            _consumer = GetOrCreateConsumer(_channel);

            _startTask = Task.Run(() =>
            {
                try
                {
                    var queueName = MessageReadStrategy.Configure(
                        _settings,
                        CreateConfiguratorChannel,
                        _cancellationTokenSource.Token);

                    _consumerTag = _channel.BasicConsume(queueName.ToString(), false, _consumer);
                }
                catch (InvalidOperationException ex)
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        _logger.LogWarning(ex, "Error configuring strategy: {Message}", ex.Message);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error configuring strategy: {Message}", e.Message);
                }

            }, _cancellationTokenSource.Token);

            return _startTask;
        }

        public void Stop()
        {
            if (_cancellationTokenSource is { IsCancellationRequested: false })
            {
                _cancellationTokenSource.Cancel();
            }

            if (_consumer?.IsRunning ?? false)
            {
                _channel.BasicCancel(_consumerTag);
            }

            if (_consumer is not null)
            {
                _consumer.Received -= OnReceived;
            }

            _channel?.Close();
            _channel?.Dispose();
            _channel = null;
            _startTask.GetAwaiter().GetResult();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Stop();

            _disposed = true;
        }

        private void CheckStartPreConditionsOrThrow()
        {
            if (MessageDeserializer == null)
                throw new InvalidOperationException("Please, specify message deserializer");

            if (EventHandler == null && CancellableEventHandler == null)
                throw new InvalidOperationException("Please, specify message handler");
        }

        private IModel GetOrCreateConsumerChannel()
        {
            if (_channel != null)
                return _channel;

            _channel = _connection.CreateModel();

            if (_prefetchCount.HasValue)
                _channel.BasicQos(0, _prefetchCount.Value, false);

            return _channel;
        }

        private IModel CreateConfiguratorChannel()
        {
            return _connection.CreateModel();
        }

        private EventingBasicConsumer GetOrCreateConsumer(IModel channel)
        {
            if (_consumer != null)
                return _consumer;

            _consumer = new EventingBasicConsumer(channel);

            _consumer.Received += OnReceived;

            return _consumer;
        }

        private static IMessageReadStrategy GetDefaultStrategy()
        {
            return new MessageReadWithTemporaryQueueStrategy();
        }
    }
}