using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

public static class MessageRouteExtensions
{
    public static MessageRoute ToMessageRoute(this ListenerRoute route) =>
        MessageRoute.Create(
            new NonEmptyString(route.ExchangeName.ToString()),
            new NonEmptyString(route.QueueName.ToString()),
            route.RoutingKey.ToString());
}
