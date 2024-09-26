using System;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class ConfigurationResultExtensions
{
    public static T Match<T>(
        this IConfigurationResult result,
        Func<T> onSuccess,
        Func<ConfigurationError, T> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    public static T Match<T, TResponse>(
        this IConfigurationResult<TResponse> result,
        Func<TResponse, T> onSuccess,
        Func<ConfigurationError, T> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Response) : onFailure(result.Error);
    }
}
