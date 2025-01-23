using System.Diagnostics;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Code}")]
internal record ConfigurationErrorCode(ushort Code)
{
    public static ConfigurationErrorCode PreconditionsFailed => new(Constants.PreconditionFailed);
    public static ConfigurationErrorCode None => new(0);
    public static ConfigurationErrorCode Cancelled => new(1000);
}

[DebuggerDisplay("{Code}: {Message}")]
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

internal interface IConfigurationResult<T> : IConfigurationResult
{
    T Response { get; }
    public void Deconstruct(out T response, out ConfigurationError error)
    {
        response = Response;
        error = Error;
    }
}

internal abstract record ConfigurationResultBase(ConfigurationError Error) : IConfigurationResult;

internal abstract record ConfigurationResultBase<T>(ConfigurationError Error, T Response) : ConfigurationResultBase(Error), IConfigurationResult<T>;

internal record ConfigurationResult(ConfigurationError Error) : ConfigurationResultBase(Error)
{
    public static IConfigurationResult Success() => new ConfigurationResult(ConfigurationError.None);
    public static IConfigurationResult Failure(ConfigurationError error) => new ConfigurationResult(error);
    public static implicit operator ConfigurationResult(ConfigurationError error) => new(error);
}

internal record ConfigurationResult<T>(ConfigurationError Error, T Response) : ConfigurationResultBase<T>(Error, Response)
{
    public static IConfigurationResult<T> Success(T response) => new ConfigurationResult<T>(ConfigurationError.None, response);
    public static IConfigurationResult<T> Failure(ConfigurationError error) => new ConfigurationResult<T>(error, default);
    public static implicit operator ConfigurationResult<T>(ConfigurationError error) => new(error, default);
}