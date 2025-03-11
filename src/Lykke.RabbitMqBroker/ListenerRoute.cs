using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

public record ListenerRoute(ExchangeName ExchangeName, QueueName QueueName, RoutingKey RoutingKey)
{
    public static ListenerRoute Create(ExchangeName exchangeName, QueueName queueName, RoutingKey routingKey) =>
        new(exchangeName, queueName, routingKey);

    public static ListenerRoute Create(ExchangeName exchangeName, QueueName queueName) =>
        new(exchangeName, queueName, RoutingKey.Empty);

    public static implicit operator MessageRoute(ListenerRoute route) =>
        MessageRoute.Create(
            new NonEmptyString(route.ExchangeName.ToString()),
            new NonEmptyString(route.QueueName.ToString()),
            route.RoutingKey.ToString());
}
