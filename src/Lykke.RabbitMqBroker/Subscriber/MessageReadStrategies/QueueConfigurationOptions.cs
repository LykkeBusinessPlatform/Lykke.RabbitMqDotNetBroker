namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class QueueConfigurationOptions
{
    public string QueueName { get; init; }
    public string ExchangeName { get; init; }
    public string DeadLetterExchangeName { get; init; }
    public string DeadLetterExchangeType { get; init; }
    public bool Durable { get; init; }
    public bool AutoDelete { get; init; }
    public string RoutingKey { get; init; }
    
    public bool ShouldConfigureDeadLettering() => !string.IsNullOrEmpty(DeadLetterExchangeName);
}
