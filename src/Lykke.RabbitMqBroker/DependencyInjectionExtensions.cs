using System;

using Autofac;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Publisher;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
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
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqMonitoring<TMessageDeliveryStorage>(
            this IServiceCollection services,
            RabbitMqMonitoringConfiguration configuration,
            string connectionString)
            where TMessageDeliveryStorage : class, IMessageDeliveryStorage, IMessageDeliveryMaintenance
        {
            services.AddSingleton<IListenerRegistrationHandler, ListenerRegistrationHandler>();
            services.AddSingleton<IMessageProducer<MonitoringHeartbeat>, MonitoringHeartbeatPublisher>();
            services.AddSingleton<ITrackableMessagePublisher<MonitoringHeartbeat>, TrackableMessagePublisher<MonitoringHeartbeat>>();

            services.TryAddSingleton<TMessageDeliveryStorage>();
            services.TryAddSingleton<IMessageDeliveryStorage>(p => p.GetRequiredService<TMessageDeliveryStorage>());
            services.TryAddSingleton<IMessageDeliveryMaintenance>(p => p.GetRequiredService<TMessageDeliveryStorage>());

            services.AddSingleton<IMonitoringHeartbeatReceiver, MonitoringHeartbeatReceiver>();
            services.Configure<RabbitMqPublisherOptions<MonitoringHeartbeat>>(opt =>
                opt.CopyFrom(MonitoringHeartbeatPublisherOptions.Create(
                    configuration.PublishConfirmationWaitTimeoutMs,
                    configuration.MessageExpirationMs)));
            services.AddSingleton<IPurePublisher<MonitoringHeartbeat>, ImmediatePublisher<MonitoringHeartbeat>>(p =>
                new ImmediatePublisher<MonitoringHeartbeat>(
                    p.GetRequiredService<IConnectionProvider>(),
                    p.GetRequiredService<IOptions<RabbitMqPublisherOptions<MonitoringHeartbeat>>>(),
                    MonitoringHeartbeatPublisherSettingsFactory.Create(connectionString)));

            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<IMessageDeliveryCleanupWorker>(p =>
                new MessageDeliveryCleanupWorker(
                    p.GetRequiredService<IMessageDeliveryMaintenance>(),
                    p.GetRequiredService<TimeProvider>(),
                    configuration.MessageRetentionPeriod));
            services.AddHostedService(p =>
                new MessageDeliveryCleanupTimer(
                    p.GetRequiredService<IMessageDeliveryCleanupWorker>(),
                    configuration.MessagesCleanupPeriod));

            return services;
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
            where TMessageDeliveryStorage : class, IMessageDeliveryStorage, IMessageDeliveryMaintenance
        {
            builder.RegisterType<ListenerRegistrationHandler>()
                .As<IListenerRegistrationHandler>()
                .SingleInstance();

            builder.RegisterType<MonitoringHeartbeatPublisher>()
                .As<IMessageProducer<MonitoringHeartbeat>>()
                .SingleInstance();

            builder.RegisterType<TrackableMessagePublisher<MonitoringHeartbeat>>()
                .As<ITrackableMessagePublisher<MonitoringHeartbeat>>()
                .SingleInstance();

            builder.RegisterType<TMessageDeliveryStorage>()
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance()
                .IfNotRegistered(typeof(TMessageDeliveryStorage));

            builder.RegisterType<MonitoringHeartbeatReceiver>()
                .As<IMonitoringHeartbeatReceiver>()
                .SingleInstance();

            builder.Register(
                _ => new ConfigureNamedOptions<RabbitMqPublisherOptions<MonitoringHeartbeat>>(
                    string.Empty,
                    opt => opt.CopyFrom(
                        MonitoringHeartbeatPublisherOptions.Create(
                            configuration.PublishConfirmationWaitTimeoutMs,
                            configuration.MessageExpirationMs))))
                .As<IConfigureOptions<RabbitMqPublisherOptions<MonitoringHeartbeat>>>()
                .SingleInstance();

            builder.RegisterType<ImmediatePublisher<MonitoringHeartbeat>>()
                .As<IPurePublisher<MonitoringHeartbeat>>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(MonitoringHeartbeatPublisherSettingsFactory.Create(connectionString)));

            builder.Register(_ => TimeProvider.System)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<MessageDeliveryCleanupWorker>()
                .As<IMessageDeliveryCleanupWorker>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(configuration.MessageRetentionPeriod));

            builder.RegisterType<MessageDeliveryCleanupTimer>()
                .As<IHostedService>()
                .SingleInstance()
                .WithParameter(TypedParameter.From(configuration.MessagesCleanupPeriod));
        }
    }
}