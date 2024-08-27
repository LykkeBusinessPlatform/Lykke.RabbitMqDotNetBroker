using System;

using Autofac;

using Microsoft.Extensions.DependencyInjection;

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
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddRabbitMqConnectionProvider();
            services.AddListenersRegistry();

            return services;
        }

        /// <summary>
        /// Adds RabbitMq monitoring services to the service collection:
        /// - Monitoring message sender
        /// - Listeners registry handlers runner
        /// - Listeners registry timer hosted service
        /// RabbitMQ connection provider and Listeners registry should be already 
        /// registered in the container.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqMonitoring(
            this IServiceCollection services,
            RabbitMqMonitoringConfiguration configuration,
            string connectionString)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IMonitoringMessagePropertiesFactory, MonitoringMessagePropertiesFactory>();
            services.AddSingleton<IMonitoringMessageChannelProvider, MonitoringMessageChannelProvider>(
                p => new MonitoringMessageChannelProvider(
                    p.GetRequiredService<IConnectionProvider>(),
                    connectionString));
            services.AddSingleton(p => new MonitoringMessageSender(
                        p.GetRequiredService<IMonitoringMessageChannelProvider>(),
                        p.GetRequiredService<IMonitoringMessagePropertiesFactory>(),
                        TimeSpan.FromMilliseconds(configuration.PublishConfirmationWaitTimeoutMs)));
            services.AddSingleton<IListenerRegistrationHandler>(p =>
                new MonitoringMessageConfirmationFailureHandler(
                    p.GetRequiredService<MonitoringMessageSender>()));
            services.AddSingleton<IListenersRegistryProcessor, ListenersRegistryProcessor>();
            services.AddHostedService(p =>
                new ListenersRegistryTimer(
                    p.GetRequiredService<IListenersRegistryProcessor>(),
                    TimeSpan.FromMilliseconds(configuration.IntervalMs)
                ));

            return services;
        }

        /// <summary>
        /// Adds RabbitMq infrastructure services to the container:
        /// - RabbitMq connection provider
        /// - Listeners registry
        /// </summary>
        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            builder.AddRabbitMqConnectionProvider();
            builder.AddListenersRegistry();
        }

        /// <summary>
        /// Adds RabbitMq monitoring services to the container:
        /// - Monitoring message sender
        /// - Listeners registry handlers runner
        /// - Listeners registry timer hosted service
        /// RabbitMQ connection provider and Listeners registry should be already
        /// registered in the container.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static void AddRabbitMqMonitoring(
            this ContainerBuilder builder,
            RabbitMqMonitoringConfiguration configuration,
            string connectionString)
        {
            builder.RegisterInstance(configuration)
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<MonitoringMessagePropertiesFactory>()
                .As<IMonitoringMessagePropertiesFactory>()
                .SingleInstance();

            builder.RegisterType<MonitoringMessageChannelProvider>()
                .As<IMonitoringMessageChannelProvider>()
                .WithParameter(TypedParameter.From(connectionString))
                .SingleInstance();

            builder.RegisterType<MonitoringMessageSender>()
                .As<IListenerRegistrationHandler>()
                .WithParameter(TypedParameter.From(connectionString))
                .WithParameter(TypedParameter.From(TimeSpan.FromMilliseconds(configuration.PublishConfirmationWaitTimeoutMs)))
                .SingleInstance();
            builder.RegisterDecorator<MonitoringMessageConfirmationFailureHandler, IListenerRegistrationHandler>();

            builder.RegisterType<ListenersRegistryProcessor>()
                .As<IListenersRegistryProcessor>()
                .SingleInstance();

            builder.RegisterType<ListenersRegistryTimer>()
                .AsSelf()
                .WithParameter("interval", TimeSpan.FromMilliseconds(configuration.IntervalMs))
                .SingleInstance();
        }
    }
}