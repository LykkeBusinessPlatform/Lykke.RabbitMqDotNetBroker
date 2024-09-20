namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class DeadLetteringConfigurationOptions
{
    public PoisonQueueName QueueName { get; init; }
    public DeadLetterExchangeName ExchangeName { get; init; }
    public string ExchangeType { get; init; }
    public bool Durable { get; init; }
    public bool AutoDelete { get; init; }
    public string RoutingKey { get; init; }

    public static DeadLetteringConfigurationOptions FromQueueConfigurationOptions(QueueConfigurationOptions options)
    {
        return new DeadLetteringConfigurationOptions
        {
            QueueName = options.QueueName.AsPoison(),
            ExchangeName = options.DeadLetterExchangeName,
            ExchangeType = options.DeadLetterExchangeType,
            Durable = options.Durable,
            AutoDelete = options.AutoDelete,
            RoutingKey = options.RoutingKey
        };
    }
}
