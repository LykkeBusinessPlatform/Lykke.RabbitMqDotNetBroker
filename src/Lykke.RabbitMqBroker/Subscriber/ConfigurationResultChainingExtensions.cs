using System;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class ConfigurationResultChainingExtensions
{
    public static IConfigurationResult Match(
        this IConfigurationResult result,
        Func<IConfigurationResult> onSuccess)
    {
        return result.IsSuccess ? onSuccess() : result;
    }

    public static IConfigurationResult<TResponse> Match<TResponse>(
       this IConfigurationResult<TResponse> result,
       Func<TResponse, IConfigurationResult<TResponse>> onSuccess)
    {
        return result.IsSuccess ? onSuccess(result.Response) : result;
    }

    public static IConfigurationResult<TResponse> Match<TResponse>(
        this IConfigurationResult<TResponse> result,
        Func<ConfigurationError, IConfigurationResult<TResponse>> onFailure)
    {
        return result.IsSuccess ? result : onFailure(result.Error);
    }
}