using Autofac;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Publisher;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Lykke.RabbitMqBroker
{

    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds RabbitMq infrastructure services to the service collection:
        /// - RabbitMq connection provider
        /// - Listeners registry
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddRabbitMq(
            this IServiceCollection services,
            RabbitMqConfiguration configuration)
        {
            services.AddRabbitMqConnectionProvider();
            services.AddListenersRegistry(configuration.ListenersRegistryHandleIntervalMs);

            return services;
        }

        /// <summary>
        /// Adds a plethora of RabbitMq monitoring services to the service collection.
        /// RabbitMQ connection provider and Listeners registry should be already 
        /// registered in the container.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqMonitoring<TMessageDeliveryStorage>(
            this IServiceCollection services,
            RabbitMqMonitoringConfiguration configuration,
            string connectionString)
            where TMessageDeliveryStorage : class, IMessageDeliveryStorage
        {
            services.AddSingleton<IListenerRegistrationHandler, MonitoringHandler>();
            services.AddSingleton<IMessageProducer<MonitoringMessage>, MonitoringMessagePublisher>();
            services.AddSingleton<ITrackableMessagePublisher<MonitoringMessage>, TrackableMessagePublisher<MonitoringMessage>>();
            services.TryAddSingleton<IMessageDeliveryStorage, TMessageDeliveryStorage>();
            services.Configure<RabbitMqPublisherOptions<MonitoringMessage>>(opt =>
                opt.CopyFrom(MonitoringMessagePublisherOptions.Create(
                    configuration.PublishConfirmationWaitTimeoutMs,
                    configuration.MessageExpirationMs)));
            services.AddSingleton<IPurePublisher<MonitoringMessage>, ImmediatePublisher<MonitoringMessage>>(p =>
                new ImmediatePublisher<MonitoringMessage>(
                    p.GetRequiredService<IConnectionProvider>(),
                    p.GetRequiredService<IOptions<RabbitMqPublisherOptions<MonitoringMessage>>>(),
                    MonitoringMessagePublisherSettingsFactory.Create(connectionString)));

            return services;
        }

        /// <summary>
        /// Adds RabbitMq infrastructure services to the container:
        /// - RabbitMq connection provider
        /// - Listeners registry
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        public static void AddRabbitMq(
            this ContainerBuilder builder,
            RabbitMqConfiguration configuration)
        {
            builder.AddRabbitMqConnectionProvider();
            builder.AddListenersRegistry(configuration.ListenersRegistryHandleIntervalMs);
        }

        /// <summary>
        /// Adds a plethora of RabbitMq monitoring services to the service collection.
        /// RabbitMQ connection provider and Listeners registry should be already 
        /// registered in the container.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static void AddRabbitMqMonitoring<TMessageDeliveryStorage>(
            this ContainerBuilder builder,
            RabbitMqMonitoringConfiguration configuration,
            string connectionString)
        {
            builder.RegisterType<MonitoringHandler>()
                .As<IListenerRegistrationHandler>()
                .SingleInstance();

            builder.RegisterType<MonitoringMessagePublisher>()
                .As<IMessageProducer<MonitoringMessage>>()
                .SingleInstance();

            builder.RegisterType<TrackableMessagePublisher<MonitoringMessage>>()
                .As<ITrackableMessagePublisher<MonitoringMessage>>()
                .SingleInstance();

            builder.RegisterType<TMessageDeliveryStorage>()
                .As<IMessageDeliveryStorage>()
                .SingleInstance()
                .IfNotRegistered(typeof(IMessageDeliveryStorage));

            builder.Register(
                _ => new ConfigureNamedOptions<RabbitMqPublisherOptions<MonitoringMessage>>(
                    string.Empty,
                    opt => opt.CopyFrom(
                        MonitoringMessagePublisherOptions.Create(
                            configuration.PublishConfirmationWaitTimeoutMs,
                            configuration.MessageExpirationMs))))
                .As<IConfigureOptions<RabbitMqPublisherOptions<MonitoringMessage>>>()
                .SingleInstance();

            builder.RegisterType<ImmediatePublisher<MonitoringMessage>>()
                .As<IPurePublisher<MonitoringMessage>>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MonitoringMessagePublisherSettingsFactory.Create(connectionString)));
        }
    }
}