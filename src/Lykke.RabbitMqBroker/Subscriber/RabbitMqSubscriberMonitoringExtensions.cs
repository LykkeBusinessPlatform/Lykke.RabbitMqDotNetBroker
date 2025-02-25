using System;

using Autofac;

using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

using Microsoft.Extensions.DependencyInjection;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class RabbitMqSubscriberMonitoringExtensions
{
    /// <summary>
    /// Adds monitoring heartbeat middleware to the subscriber at the beginning of the pipeline.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="subscriber"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static RabbitMqSubscriber<TModel> UseMonitoringHeartbeatMiddleware<TModel>(
        this RabbitMqSubscriber<TModel> subscriber, IComponentContext context)
    {
        var heartbeatReceiver = context.ResolveOptional<IMonitoringHeartbeatReceiver>();
        subscriber.UseMiddlewareAt(0, new MonitoringHeartbeatMiddleware<TModel>(heartbeatReceiver));
        return subscriber;
    }

    /// <summary>
    /// Adds monitoring heartbeat middleware to the subscriber at the beginning of the pipeline.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="subscriber"></param>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public static RabbitMqSubscriber<TModel> UseMonitoringHeartbeatMiddleware<TModel>(
        this RabbitMqSubscriber<TModel> subscriber, IServiceProvider serviceProvider)
    {
        var heartbeatReceiver = serviceProvider.GetService<IMonitoringHeartbeatReceiver>();
        subscriber.UseMiddlewareAt(0, new MonitoringHeartbeatMiddleware<TModel>(heartbeatReceiver));
        return subscriber;
    }
}