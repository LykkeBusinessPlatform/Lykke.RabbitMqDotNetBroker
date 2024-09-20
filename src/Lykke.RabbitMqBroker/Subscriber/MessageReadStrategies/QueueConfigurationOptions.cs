namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class QueueConfigurationOptions
{
    public QueueName QueueName { get; init; }
    public ExchangeName ExchangeName { get; init; }
    public DeadLetterExchangeName DeadLetterExchangeName { get; init; }
    public string DeadLetterExchangeType { get; init; }
    public bool Durable { get; init; }
    public bool AutoDelete { get; init; }
    public RoutingKey RoutingKey { get; init; }
    public QueueType QueueType { get; init; }
    internal QueueConfigurationOptions() { }
    public static QueueConfigurationOptions ForClassicQueue(
        QueueName queueName,
        ExchangeName exchangeName,
        DeadLetterExchangeName deadLetterExchangeName,
        string deadLetterExchangeType,
        bool durable,
        bool autoDelete,
        RoutingKey routingKey) => new()
        {
            QueueName = queueName,
            ExchangeName = exchangeName,
            DeadLetterExchangeName = deadLetterExchangeName,
            DeadLetterExchangeType = deadLetterExchangeType,
            Durable = durable,
            AutoDelete = autoDelete,
            RoutingKey = routingKey,
            QueueType = QueueType.Classic
        };
    public static QueueConfigurationOptions ForQuorumQueue(
        QueueName queueName,
        ExchangeName exchangeName,
        DeadLetterExchangeName deadLetterExchangeName,
        string deadLetterExchangeType,
        RoutingKey routingKey) => new()
        {
            QueueName = queueName,
            ExchangeName = exchangeName,
            DeadLetterExchangeName = deadLetterExchangeName,
            DeadLetterExchangeType = deadLetterExchangeType,
            Durable = true,
            AutoDelete = false,
            RoutingKey = routingKey,
            QueueType = QueueType.Quorum
        };
}
