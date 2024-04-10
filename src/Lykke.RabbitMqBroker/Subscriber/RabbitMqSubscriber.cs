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
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Subscriber
{
    /// <summary>
    /// Generic rabbitMq subscriber
    /// Not thread-safe
    /// </summary>
    [PublicAPI]
    public sealed class RabbitMqSubscriber<TTopicModel> : IStartStop
    {
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly ILogger<RabbitMqSubscriber<TTopicModel>> _logger;
        private readonly MiddlewareQueue<TTopicModel> _middlewareQueue;
        private readonly List<Action<IDictionary<string, object>>> _readHeadersActions = new();
        private readonly IAutorecoveringConnection _connection;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _disposed;
        private ushort? _prefetchCount;
        
        private IModel _channel;
        private EventingBasicConsumer _consumer;
        private string _consumerTag;
        
        public IMessageDeserializer<TTopicModel> MessageDeserializer { get; private set; }
        public IMessageReadStrategy MessageReadStrategy { get; private set; }
        public Func<TTopicModel, Task> EventHandler { get; private set; }
        public Func<TTopicModel, CancellationToken, Task> CancellableEventHandler { get; private set; }

        public static class MessagePack
        {
            public static RabbitMqSubscriber<TTopicModel> CreateNoLossSubscriber(
                ILoggerFactory loggerFactory, 
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, ILoggerFactory> configure = null)
            {
                return Create<NoLossMessageReadStrategy>(loggerFactory, settings, connection, configure);
            }

            public static RabbitMqSubscriber<TTopicModel> CreateLossAcceptableSubscriber(
                ILoggerFactory loggerFactory, 
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, ILoggerFactory> configure = null)
            {
                return Create<LossAcceptableMessageReadStrategy>(loggerFactory, settings, connection, configure);
            }

            private static RabbitMqSubscriber<TTopicModel> Create<TStrategy>(
                ILoggerFactory loggerFactory,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, ILoggerFactory> configure = null)
                where TStrategy : IMessageReadStrategy, new()
            {
                var logger = loggerFactory.CreateLogger<RabbitMqSubscriber<TTopicModel>>();
                var middlewareLogger = loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TTopicModel>>();

                var subscriber = new RabbitMqSubscriber<TTopicModel>(
                        logger,
                        settings,
                        connection)
                    .SetMessageDeserializer(new MessagePackMessageDeserializer<TTopicModel>())
                    .SetMessageReadStrategy(new TStrategy())
                    .UseMiddleware(new ExceptionSwallowMiddleware<TTopicModel>(middlewareLogger));

                configure?.Invoke(subscriber, loggerFactory);

                return subscriber;
            }
        }
        
        public static class Json
        {
            public static RabbitMqSubscriber<TTopicModel> CreateNoLossSubscriber(
                ILoggerFactory loggerFactory, 
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, ILoggerFactory> configure = null)
            {
                return Create<NoLossMessageReadStrategy>(loggerFactory, settings, connection, configure);
            }

            public static RabbitMqSubscriber<TTopicModel> CreateLossAcceptableSubscriber(
                ILoggerFactory loggerFactory, 
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, ILoggerFactory> configure = null)
            {
                return Create<LossAcceptableMessageReadStrategy>(loggerFactory, settings, connection, configure);
            }
            
            private static RabbitMqSubscriber<TTopicModel> Create<TStrategy>(
                ILoggerFactory loggerFactory,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, ILoggerFactory> configure = null) where TStrategy : IMessageReadStrategy, new()
            {
                var logger = loggerFactory.CreateLogger<RabbitMqSubscriber<TTopicModel>>();
                var middlewareLogger = loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TTopicModel>>();
            
                var subscriber = new RabbitMqSubscriber<TTopicModel>(
                        logger,
                        settings,
                        connection)
                    .SetMessageDeserializer(new JsonMessageDeserializer<TTopicModel>())
                    .SetMessageReadStrategy(new TStrategy())
                    .UseMiddleware(new ExceptionSwallowMiddleware<TTopicModel>(middlewareLogger));

                configure?.Invoke(subscriber, loggerFactory);

                return subscriber;
            }
        }

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
            if (_consumer is { IsRunning: true })
                return this;

            CheckStartPreConditionsOrThrow();

            if (_cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource = new CancellationTokenSource();
            }

            MessageReadStrategy ??= GetDefaultStrategy();

            var actualHandlerMiddleware = EventHandler != null
                ? new ActualHandlerMiddleware<TTopicModel>(EventHandler)
                : new ActualHandlerMiddleware<TTopicModel>(CancellableEventHandler);
            _middlewareQueue.AddMiddleware(actualHandlerMiddleware);

            _channel = GetOrCreateChannel();

            _consumer = GetOrCreateConsumer(_channel);
            
            var queueName = MessageReadStrategy.Configure(_settings, _channel);

            _consumerTag = _channel.BasicConsume(queueName, false, _consumer);

            return this;
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();

            if (_consumer.IsRunning)
            {
                _channel.BasicCancel(_consumerTag);
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
        
        private void CheckStartPreConditionsOrThrow()
        {
            if (MessageDeserializer == null)
                throw new InvalidOperationException("Please, specify message deserializer");

            if (EventHandler == null && CancellableEventHandler == null)
                throw new InvalidOperationException("Please, specify message handler");
        }
        
        private IModel GetOrCreateChannel()
        {
            if (_channel != null)
                return _channel;

            _channel = _connection.CreateModel();
            
            if (_prefetchCount.HasValue)
                _channel.BasicQos(0, _prefetchCount.Value, false);

            return _channel;
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
