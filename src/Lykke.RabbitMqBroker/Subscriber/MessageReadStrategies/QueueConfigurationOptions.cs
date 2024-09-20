namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class QueueConfigurationOptions
{
    public QueueName QueueName { get; init; }
    public string ExchangeName { get; init; }
    public string DeadLetterExchangeName { get; init; }
    public string DeadLetterExchangeType { get; init; }
    public bool Durable { get; init; }
    public bool AutoDelete { get; init; }
    public string RoutingKey { get; init; }

    /// <summary>
    /// The type of the queue. Depending on the type, 
    /// some aforementioned options might be or might be not supported.
    /// In case they are not supported, they will be ignored.
    /// This has to be redesigned.
    /// In particular: Durable, AutoDelete, DeadLetterExchangeName, DeadLetterExchangeType
    /// </summary>
    public QueueType QueueType { get; init; }

    public bool ShouldConfigureDeadLettering() => !string.IsNullOrWhiteSpace(DeadLetterExchangeName);
}
