using System;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class QueueConfigurationResultExtensions
{
    public static TResult Match<TResult>(
        this IQueueConfigurationResult result,
        Func<QueueConfigurationSuccess<TResult>, TResult> onSuccess,
        Func<QueueConfigurationPreconditionFailure, TResult> onFailure)
    {
        return result switch
        {
            QueueConfigurationSuccess<TResult> success => onSuccess(success),
            QueueConfigurationPreconditionFailure failure => onFailure(failure),
            _ => throw new InvalidOperationException("Unexpected queue configuration result")
        };
    }
}