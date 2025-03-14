// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber
{
    /// <summary>
    /// Listener is a concept that wraps subscriber or multiple subscriber instances,
    /// takes care of creating, starting and disposing subscriber instances with
    /// provided options. It injects message handler(-s) and runs them.
    /// Being registered in DI container as <see cref="IStartable"/>,
    /// it is started automatically when the application starts.
    /// Autofac is responsible for disposing the listener when the application stops.
    ///
    /// Requires Autofac to be used at least as service provider factory.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public sealed class RabbitMqListener<T> : IRabbitMqListener, IStartable, IDisposable
        where T : class
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly RabbitMqSubscriptionSettings _subscriptionSettings;
        private readonly RabbitMqListenerOptions<T> _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Action<RabbitMqSubscriber<T>> _configureSubscriber;
        private readonly IEnumerable<IMessageHandler<T>> _handlers;
        private readonly List<RabbitMqSubscriber<T>> _subscribers = [];
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Creates a new instance of <see cref="RabbitMqListener{T}"/>
        /// </summary>
        /// <param name="connectionProvider">Rabbit MQ connection provider</param>
        /// <param name="subscriptionSettings">Subscription configuration</param>
        /// <param name="optionsAccessor">Listener options</param>
        /// <param name="configureSubscriber">Low-level subscriber configuration callback</param>
        /// <param name="handlers">Message handlers</param>
        /// <param name="loggerFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RabbitMqListener(
            IConnectionProvider connectionProvider,
            RabbitMqSubscriptionSettings subscriptionSettings,
            IOptions<RabbitMqListenerOptions<T>> optionsAccessor,
            Action<RabbitMqSubscriber<T>> configureSubscriber,
            IEnumerable<IMessageHandler<T>> handlers,
            ILoggerFactory loggerFactory)
        {
            _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
            _subscriptionSettings = subscriptionSettings ?? throw new ArgumentNullException(nameof(subscriptionSettings));
            _options = optionsAccessor?.Value ?? RabbitMqListenerOptions<T>.Json.Default;
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            LoggerFactoryContainer.Instance = _loggerFactory;
            _configureSubscriber = configureSubscriber;
            _handlers = handlers;
            ContractType = typeof(T);
        }

        public void Start()
        {
            if (_subscribers.Any())
                throw new InvalidOperationException("The listener is already started");

            foreach (var _ in _options.ConsumerCount)
            {
                var connection = CreateConnection();
                var subscriber = CreateSubscriber(connection)
                    .Subscribe(Handle)
                    .Start();

                _subscribers.Add(subscriber);
            }
        }

        private IAutorecoveringConnection CreateConnection()
        {
            return _options.ShareConnection switch
            {
                true => _connectionProvider.GetOrCreateShared(_subscriptionSettings.ConnectionString),
                false => _connectionProvider.GetExclusive(_subscriptionSettings.ConnectionString)
            };
        }

        private RabbitMqSubscriber<T> CreateSubscriber(IAutorecoveringConnection connection)
        {
            return _options.SerializationFormat switch
            {
                SerializationFormat.Json => _options.SubscriptionTemplate switch
                {
                    SubscriptionTemplate.NoLoss => RabbitMqSubscriber<T>.Json
                        .CreateNoLossSubscriber(_subscriptionSettings, connection, _loggerFactory, _configureSubscriber),
                    SubscriptionTemplate.LossAcceptable => RabbitMqSubscriber<T>.Json
                        .CreateLossAcceptableSubscriber(_subscriptionSettings, connection, _loggerFactory, _configureSubscriber),
                    _ => throw new InvalidOperationException(
                        $"Unsupported subscription template: {_options.SubscriptionTemplate}")
                },
                SerializationFormat.Messagepack => _options.SubscriptionTemplate switch
                {
                    SubscriptionTemplate.NoLoss => RabbitMqSubscriber<T>.MessagePack
                        .CreateNoLossSubscriber(_subscriptionSettings, connection, _loggerFactory, _configureSubscriber),
                    SubscriptionTemplate.LossAcceptable => RabbitMqSubscriber<T>.MessagePack
                        .CreateLossAcceptableSubscriber(_subscriptionSettings, connection, _loggerFactory, _configureSubscriber),
                    _ => throw new InvalidOperationException(
                        $"Unsupported subscription template: {_options.SubscriptionTemplate}")
                },
                _ => throw new InvalidOperationException(
                    $"Unsupported serialization format: {_options.SerializationFormat}")
            };
        }

        private Task Handle(T message)
        {
            var allTasks = _handlers.Select(h => h.Handle(message));
            return Task.WhenAll(allTasks);
        }

        void IDisposable.Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            if (_subscribers.Count == 0) return;

            if (_cancellationTokenSource is { IsCancellationRequested: false })
            {
                _cancellationTokenSource.Cancel();
            }

            for (var i = _subscribers.Count - 1; i >= 0; i--)
            {
                _subscribers[i].Dispose();
                _subscribers.RemoveAt(i);
            }
        }

        public bool IsConnected()
        {
            return _subscribers.Count > 0 && _subscribers.Any(x => x.IsOpen);
        }

        public Type ContractType { get; }
    }
}