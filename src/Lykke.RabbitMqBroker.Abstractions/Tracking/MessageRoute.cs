namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public record MessageRoute(NonEmptyString ExchangeName, NonEmptyString QueueName, string RoutingKey)
{
    public static MessageRoute Create(NonEmptyString exchangeName, NonEmptyString queueName, string routingKey) =>
        new(exchangeName, queueName, routingKey);
    public static readonly MessageRoute None = new(new("empty"), new("empty"), string.Empty);
}
