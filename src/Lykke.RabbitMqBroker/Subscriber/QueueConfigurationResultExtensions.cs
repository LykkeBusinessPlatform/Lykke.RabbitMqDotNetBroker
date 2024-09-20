using System;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class QueueConfigurationResultExtensions
{
    public static T Match<T>(
        this QueueConfigurationResult result,
        Func<T> onSuccess,
        Func<QueueConfigurationError, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    public static T Match<T, TResponse>(
        this QueueConfigurationResult<TResponse> result,
        Func<TResponse, T> onSuccess,
        Func<QueueConfigurationError, T> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Response) : onFailure(result.Error);
    }
}