using System;
using System.Linq;

using Autofac;
using Autofac.Core;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Publisher;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Lykke.RabbitMqBroker;

public static class MonitoringDIExtensions
{
    /// <summary>
    /// Adds a plethora of RabbitMq monitoring services to the service collection.
    /// RabbitMQ connection provider and Listeners registry should be already 
    /// registered in the container.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqMonitoring<TMessageDeliveryStorage, TIssueNotifier>(
        this IServiceCollection services,
        RabbitMqMonitoringConfiguration configuration,
        string connectionString)
        where TMessageDeliveryStorage : class, IMessageDeliveryStorage
        where TIssueNotifier : class, IMonitoringIssueNotifier
    {
        services.AddSingleton<IListenerRegistrationHandler, ListenerRegistrationHandler>();
        services.AddSingleton<IMessageProducer<MonitoringHeartbeat>, MonitoringHeartbeatPublisher>();
        services.AddSingleton<ITrackableMessagePublisher<MonitoringHeartbeat>, TrackableMessagePublisher<MonitoringHeartbeat>>();

        services.TryAddSingleton<TMessageDeliveryStorage>();
        services.TryAddSingleton<IMessageDeliveryStorage>(p => p.GetRequiredService<TMessageDeliveryStorage>());

        services.TryAddSingleton<IMonitoringIssueNotifier, TIssueNotifier>();

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

        services.AddSingleton<IMessageDeliveryAnalysisWorker, MessageDeliveryAnalysisWorker>();
        services.AddHostedService(p =>
            new MessageDeliveryAnalysisTimer(
                p.GetRequiredService<IMessageDeliveryAnalysisWorker>(),
                configuration.AnalysisPeriod));

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
    public static void AddRabbitMqMonitoring<TMessageDeliveryStorage, TIssueNotifier>(
        this ContainerBuilder builder,
        RabbitMqMonitoringConfiguration configuration,
        string connectionString)
        where TMessageDeliveryStorage : class, IMessageDeliveryStorage
        where TIssueNotifier : class, IMonitoringIssueNotifier
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
            .As<IMessageDeliveryStorage>()
            .SingleInstance();

        builder.RegisterType<TIssueNotifier>()
            .As<IMonitoringIssueNotifier>()
            .SingleInstance()
            .IfNotRegistered(typeof(IMonitoringIssueNotifier));

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

        builder.RegisterType<MessageDeliveryAnalysisWorker>()
            .As<IMessageDeliveryAnalysisWorker>()
            .SingleInstance();

        builder.RegisterType<MessageDeliveryAnalysisTimer>()
            .As<IHostedService>()
            .SingleInstance()
            .WithParameter(TypedParameter.From(configuration.AnalysisPeriod));
    }

    /// <summary>
    /// Adds monitoring delivery messages retention policy
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqMonitoringRetentionPolicy<TMessageDeliveryMaintenance>(
        this IServiceCollection services,
        RabbitMqMonitoringConfiguration configuration)
        where TMessageDeliveryMaintenance : class, IMessageDeliveryMaintenance
    {
        if (services.Any(s => s.ServiceType == typeof(TMessageDeliveryMaintenance)))
        {
            services.TryAddSingleton<IMessageDeliveryMaintenance>(p => p.GetRequiredService<TMessageDeliveryMaintenance>());
        }
        else
        {
            services.TryAddSingleton<IMessageDeliveryMaintenance, TMessageDeliveryMaintenance>();
        }

        services.TryAddSingleton(TimeProvider.System);
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
    /// Adds monitoring delivery messages retention policy
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static void AddRabbitMqMonitoringRetentionPolicy<TMessageDeliveryMaintenance>(
        this ContainerBuilder builder,
        RabbitMqMonitoringConfiguration configuration)
        where TMessageDeliveryMaintenance : class, IMessageDeliveryMaintenance
    {
        builder.Register(ctx => ctx.Resolve<TMessageDeliveryMaintenance>())
            .As<IMessageDeliveryMaintenance>()
            .SingleInstance()
            .OnlyIf(r => r.IsRegistered(new TypedService(typeof(TMessageDeliveryMaintenance))));

        builder.RegisterType<TMessageDeliveryMaintenance>()
            .As<IMessageDeliveryMaintenance>()
            .SingleInstance()
            .IfNotRegistered(typeof(IMessageDeliveryMaintenance));

        builder.Register(_ => TimeProvider.System)
            .AsSelf()
            .SingleInstance()
            .IfNotRegistered(typeof(TimeProvider));

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