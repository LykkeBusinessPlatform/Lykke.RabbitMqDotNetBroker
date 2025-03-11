using System;
using System.Collections.Generic;
using System.Linq;

using Lykke.RabbitMqBroker.Monitoring;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

internal static class EventContextExtensions
{
    public static bool IsMonitoringMessage<T>(this IEventContext<T> context)
    {
        return context.BasicProperties.Type == ServiceMessageType.Monitoring.ToString();
    }

    public static bool MonitoringMessageDestinationOneOf<T>(this IEventContext<T> context, IEnumerable<QueueName> destinations)
    {
        var queueName = context.BasicProperties.GetDestinationQueue();
        return queueName is null
            ? false
            : destinations.Contains(queueName);
    }
}