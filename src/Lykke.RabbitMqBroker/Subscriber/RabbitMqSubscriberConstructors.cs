using System;

using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber
{
    public sealed partial class RabbitMqSubscriber<TTopicModel>
    {
        public static class MessagePack
        {
            /// <summary>
            /// Create no loss message pack subscriber
            /// </summary>
            /// <param name="serviceProvider">Service collection provider</param>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateNoLossSubscriber(
                IServiceProvider serviceProvider,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, IServiceProvider> configure = null)
            {
                return Create<NoLossMessageReadStrategy>(serviceProvider, settings, connection, configure);
            }

            /// <summary>
            /// Create no loss message pack subscriber
            /// </summary>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="loggerFactory">Logger factory</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateNoLossSubscriber(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILoggerFactory loggerFactory,
                Action<RabbitMqSubscriber<TTopicModel>> configure = null)
            {
                return Create<NoLossMessageReadStrategy>(settings, connection, loggerFactory, configure);
            }

            /// <summary>
            /// Create loss acceptable message pack subscriber
            /// </summary>
            /// <param name="serviceProvider">Service collection provider</param>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateLossAcceptableSubscriber(
                IServiceProvider serviceProvider,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, IServiceProvider> configure = null)
            {
                return Create<LossAcceptableMessageReadStrategy>(serviceProvider, settings, connection, configure);
            }

            /// <summary>
            /// Create loss acceptable message pack subscriber
            /// </summary>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="loggerFactory">Logger factory</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateLossAcceptableSubscriber(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILoggerFactory loggerFactory,
                Action<RabbitMqSubscriber<TTopicModel>> configure = null)
            {
                return Create<LossAcceptableMessageReadStrategy>(settings, connection, loggerFactory, configure);
            }

            private static RabbitMqSubscriber<TTopicModel> Create<TStrategy>(
                IServiceProvider serviceProvider,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, IServiceProvider> configure = null)
                where TStrategy : IMessageReadStrategy, new()
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RabbitMqSubscriber<TTopicModel>>();
                var middlewareLogger = loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TTopicModel>>();
                LoggerFactoryContainer.Instance = loggerFactory;

                var subscriber = CreateRawSubscriber<TStrategy>(settings, connection, logger, middlewareLogger);

                configure?.Invoke(subscriber, serviceProvider);

                return subscriber;
            }

            private static RabbitMqSubscriber<TTopicModel> Create<TStrategy>(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILoggerFactory loggerFactory,
                Action<RabbitMqSubscriber<TTopicModel>> configure = null)
                where TStrategy : IMessageReadStrategy, new()
            {
                var logger = loggerFactory.CreateLogger<RabbitMqSubscriber<TTopicModel>>();
                var middlewareLogger = loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TTopicModel>>();
                LoggerFactoryContainer.Instance = loggerFactory;

                var subscriber = CreateRawSubscriber<TStrategy>(settings, connection, logger, middlewareLogger);

                configure?.Invoke(subscriber);

                return subscriber;
            }

            private static RabbitMqSubscriber<TTopicModel> CreateRawSubscriber<TStrategy>(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILogger<RabbitMqSubscriber<TTopicModel>> logger,
                ILogger<ExceptionSwallowMiddleware<TTopicModel>> exceptionMiddlewareLogger)
                where TStrategy : IMessageReadStrategy, new()
            {
                return new RabbitMqSubscriber<TTopicModel>(
                        logger,
                        settings,
                        connection)
                    .SetMessageDeserializer(new MessagePackMessageDeserializer<TTopicModel>())
                    .SetMessageReadStrategy(new TStrategy())
                    .UseMiddleware(new ExceptionSwallowMiddleware<TTopicModel>(exceptionMiddlewareLogger));
            }
        }

        public static class Json
        {
            /// <summary>
            /// Create no loss json subscriber
            /// </summary>
            /// <param name="serviceProvider">Service collection provider</param>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateNoLossSubscriber(
                IServiceProvider serviceProvider,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, IServiceProvider> configure = null)
            {
                return Create<NoLossMessageReadStrategy>(serviceProvider, settings, connection, configure);
            }

            /// <summary>
            /// Create no loss json subscriber
            /// </summary>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="loggerFactory">Logger factory</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateNoLossSubscriber(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILoggerFactory loggerFactory,
                Action<RabbitMqSubscriber<TTopicModel>> configure = null)
            {
                return Create<NoLossMessageReadStrategy>(settings, connection, loggerFactory, configure);
            }

            /// <summary>
            /// Create loss acceptable json subscriber
            /// </summary>
            /// <param name="serviceProvider">Service collection provider</param>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateLossAcceptableSubscriber(
                IServiceProvider serviceProvider,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, IServiceProvider> configure = null)
            {
                return Create<LossAcceptableMessageReadStrategy>(serviceProvider, settings, connection, configure);
            }

            /// <summary>
            /// Create loss acceptable json subscriber
            /// </summary>
            /// <param name="settings">Subscriber configuration</param>
            /// <param name="connection">Autorecovering connection</param>
            /// <param name="loggerFactory">Logger factory</param>
            /// <param name="configure">Callback for additional configuration</param>
            /// <returns></returns>
            public static RabbitMqSubscriber<TTopicModel> CreateLossAcceptableSubscriber(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILoggerFactory loggerFactory,
                Action<RabbitMqSubscriber<TTopicModel>> configure = null)
            {
                return Create<LossAcceptableMessageReadStrategy>(settings, connection, loggerFactory, configure);
            }

            private static RabbitMqSubscriber<TTopicModel> Create<TStrategy>(
                IServiceProvider serviceProvider,
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                Action<RabbitMqSubscriber<TTopicModel>, IServiceProvider> configure = null)
                where TStrategy : IMessageReadStrategy, new()
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<RabbitMqSubscriber<TTopicModel>>();
                var middlewareLogger = loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TTopicModel>>();
                LoggerFactoryContainer.Instance = loggerFactory;

                var subscriber = CreateRawSubscriber<TStrategy>(settings, connection, logger, middlewareLogger);

                configure?.Invoke(subscriber, serviceProvider);

                return subscriber;
            }

            private static RabbitMqSubscriber<TTopicModel> Create<TStrategy>(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILoggerFactory loggerFactory,
                Action<RabbitMqSubscriber<TTopicModel>> configure = null)
                where TStrategy : IMessageReadStrategy, new()
            {
                var logger = loggerFactory.CreateLogger<RabbitMqSubscriber<TTopicModel>>();
                var middlewareLogger = loggerFactory.CreateLogger<ExceptionSwallowMiddleware<TTopicModel>>();
                LoggerFactoryContainer.Instance = loggerFactory;

                var subscriber = CreateRawSubscriber<TStrategy>(settings, connection, logger, middlewareLogger);

                configure?.Invoke(subscriber);

                return subscriber;
            }

            private static RabbitMqSubscriber<TTopicModel> CreateRawSubscriber<TStrategy>(
                RabbitMqSubscriptionSettings settings,
                IAutorecoveringConnection connection,
                ILogger<RabbitMqSubscriber<TTopicModel>> logger,
                ILogger<ExceptionSwallowMiddleware<TTopicModel>> exceptionMiddlewareLogger)
                where TStrategy : IMessageReadStrategy, new()
            {
                return new RabbitMqSubscriber<TTopicModel>(
                        logger,
                        settings,
                        connection)
                    .SetMessageDeserializer(new JsonMessageDeserializer<TTopicModel>())
                    .SetMessageReadStrategy(new TStrategy())
                    .UseMiddleware(new ExceptionSwallowMiddleware<TTopicModel>(exceptionMiddlewareLogger));
            }
        }
    }
}