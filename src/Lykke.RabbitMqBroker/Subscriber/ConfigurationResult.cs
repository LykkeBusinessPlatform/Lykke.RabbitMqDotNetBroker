using System;

namespace Lykke.RabbitMqBroker.Subscriber;

internal record ConfigurationErrorCode(ushort Code)
{
    public ushort Code { get; } = Code < 0 ? throw new ArgumentOutOfRangeException(nameof(Code)) : Code;
    public static ConfigurationErrorCode None => new(0);
}

internal record ConfigurationError(ConfigurationErrorCode Code, string Message)
{
    public static ConfigurationError None => new(ConfigurationErrorCode.None, string.Empty);
}

internal interface IConfigurationResult
{
    bool IsSuccess => Error == ConfigurationError.None;
    bool IsFailure => !IsSuccess;
    ConfigurationError Error { get; }
}

internal interface IConfigurationResult<out T> : IConfigurationResult
{
    T Response { get; }
}

internal abstract record ConfigurationResultBase(ConfigurationError Error) : IConfigurationResult;

internal abstract record ConfigurationResultBase<T>(ConfigurationError Error, T Response) : ConfigurationResultBase(Error), IConfigurationResult<T>;

internal record ConfigurationResult(ConfigurationError Error) : ConfigurationResultBase(Error)
{
    public static IConfigurationResult Success() => new ConfigurationResult(ConfigurationError.None);
    public static IConfigurationResult Failure(ConfigurationError error) => new ConfigurationResult(error);
}

internal record ConfigurationResult<T>(ConfigurationError Error, T Response) : ConfigurationResultBase<T>(Error, Response)
{
    public static IConfigurationResult<T> Success(T response) => new ConfigurationResult<T>(ConfigurationError.None, response);
    public static IConfigurationResult<T> Failure(ConfigurationError error) => new ConfigurationResult<T>(error, default);
}