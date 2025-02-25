using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

[DebuggerDisplay("{ToString()}")]
public record MessageRoute(NonEmptyString ExchangeName, NonEmptyString QueueName, string RoutingKey)
{
    public override string ToString()
    {
        var baseRouteText = $"[Exchange: {ExchangeName}] -> [Queue: {QueueName}]";
        return string.IsNullOrWhiteSpace(RoutingKey)
            ? baseRouteText
            : $"{baseRouteText} with RoutingKey: {RoutingKey}";
    }

    public static MessageRoute Create(NonEmptyString exchangeName, NonEmptyString queueName, string routingKey) =>
        new(exchangeName, queueName, routingKey);
    public static readonly MessageRoute None = new(new("empty"), new("empty"), string.Empty);
}
