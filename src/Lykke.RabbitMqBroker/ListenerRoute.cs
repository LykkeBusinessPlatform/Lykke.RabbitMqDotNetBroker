using System.Diagnostics;

using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

[DebuggerDisplay("{ToString()}")]
public record ListenerRoute(ExchangeName ExchangeName, QueueName QueueName, RoutingKey RoutingKey)
{
    public override string ToString()
    {
        var baseRoute = $"[Exchange: {ExchangeName}] -> [Queue: {QueueName}]";
        return RoutingKey.IsEmpty
            ? baseRoute
            : $"{baseRoute} with RoutingKey: {RoutingKey}";
    }

    public static ListenerRoute Create(ExchangeName exchangeName, QueueName queueName, RoutingKey routingKey) =>
        new(exchangeName, queueName, routingKey);

    public static ListenerRoute Create(ExchangeName exchangeName, QueueName queueName) =>
        new(exchangeName, queueName, RoutingKey.Empty);
}
