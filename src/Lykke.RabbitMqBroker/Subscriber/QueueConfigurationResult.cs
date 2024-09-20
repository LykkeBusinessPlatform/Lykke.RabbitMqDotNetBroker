namespace Lykke.RabbitMqBroker.Subscriber;

internal record QueueConfigurationError(int Code, string Message)
{
    public static QueueConfigurationError None => new(0, string.Empty);
}

internal abstract record QueueConfigurationResultBase(QueueConfigurationError Error)
{
    public bool IsSuccess => Error == QueueConfigurationError.None;
    public bool IsFailure => !IsSuccess;
}

internal record QueueConfigurationResult(QueueConfigurationError Error) : QueueConfigurationResultBase(Error)
{
    public static QueueConfigurationResult Success => new(QueueConfigurationError.None);
    public static QueueConfigurationResult Failure(QueueConfigurationError error) => new(error);
}

internal record QueueConfigurationResult<T>(QueueConfigurationError Error, T Response) : QueueConfigurationResultBase(Error)
{
    public static QueueConfigurationResult<T> Success(T response) => new(QueueConfigurationError.None, response);
    public static QueueConfigurationResult<T> Failure(QueueConfigurationError error) => new(error, default);
}
