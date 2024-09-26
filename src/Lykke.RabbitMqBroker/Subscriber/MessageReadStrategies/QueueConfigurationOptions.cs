namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

/// <param name="QueueName"> The name of the queue to be used when declaring it. </param>
/// <param name="ExistingExchangeName"> The name of the exchange to be used when binding the queue.
/// The exchange itself should already exist. </param>
/// <param name="DeadLetterExchangeName"> The name of the exchange to be used as a dead-letter exchange.
/// The exchange will be created if it does not exist. </param>
/// <param name="DeadLetterExchangeType"> The type of the exchange to be used as a dead-letter exchange. </param>
/// <param name="Durable"> Whether the queue should be durable. Quorum queues are always durable
/// and this property if set to false will be ignored. </param>
/// <param name="AutoDelete"> Whether the queue should be auto-deleted. Quorum queues are never auto-deleted
/// and this property if set to true will be ignored. </param>
/// <param name="RoutingKey"> The routing key to be used when binding the queue to the existing exchange. </param>
/// <param name="QueueType"> The type of the queue to be declared.
/// Supported types are classic and quorum. </param>
internal sealed record QueueConfigurationOptions(
    QueueName QueueName,
    ExchangeName ExistingExchangeName,
    DeadLetterExchangeName DeadLetterExchangeName = null,
    string DeadLetterExchangeType = "",
    bool Durable = false,
    bool AutoDelete = true,
    RoutingKey RoutingKey = null,
    QueueType QueueType = QueueType.Classic)
{
    /// <summary>
    /// Creates a new instance of the <see cref="QueueConfigurationOptions"/> class
    /// for a classic queue.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="deadLetterExchangeName"></param>
    /// <param name="deadLetterExchangeType"></param>
    /// <param name="durable"></param>
    /// <param name="autoDelete"></param>
    /// <param name="routingKey"></param>
    /// <returns></returns>
    public static QueueConfigurationOptions ForClassicQueue(
        QueueName queueName,
        ExchangeName exchangeName,
        DeadLetterExchangeName deadLetterExchangeName,
        string deadLetterExchangeType,
        bool durable,
        bool autoDelete,
        RoutingKey routingKey) => new(
            queueName,
            exchangeName,
            deadLetterExchangeName,
            deadLetterExchangeType,
            durable,
            autoDelete,
            routingKey,
            QueueType.Classic
        );

    /// <summary>
    /// Creates a new instance of the <see cref="QueueConfigurationOptions"/> class
    /// for a quorum queue.
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="exchangeName"></param>
    /// <param name="deadLetterExchangeName"></param>
    /// <param name="deadLetterExchangeType"></param>
    /// <param name="routingKey"></param>
    /// <returns></returns>
    public static QueueConfigurationOptions ForQuorumQueue(
        QueueName queueName,
        ExchangeName exchangeName,
        DeadLetterExchangeName deadLetterExchangeName,
        string deadLetterExchangeType,
        RoutingKey routingKey) => new(
            queueName,
            exchangeName,
            deadLetterExchangeName,
            deadLetterExchangeType,
            true,
            false,
            routingKey,
            QueueType.Quorum
        );
}
