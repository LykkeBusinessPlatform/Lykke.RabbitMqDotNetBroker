namespace Lykke.RabbitMqBroker.Subscriber;

internal interface IQueueConfigurationResult
{
}

internal record QueueConfigurationSuccess() : IQueueConfigurationResult;
internal sealed record QueueConfigurationSuccess<T>(T Response) : QueueConfigurationSuccess;
internal sealed record QueueConfigurationPreconditionFailure(string Message) : IQueueConfigurationResult;
