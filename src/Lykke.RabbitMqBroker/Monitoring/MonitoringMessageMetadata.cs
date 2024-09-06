using System;

namespace Lykke.RabbitMqBroker.Monitoring;

public record MonitoringMessageMetadata(Guid Id, string ExchangeName, string RoutingKey, string RouteText)
{
    public static MonitoringMessageMetadata Create(
        string exchangeName,
        string routingKey,
        string routeText) =>
        new(Guid.NewGuid(), exchangeName, routingKey, routeText);
}
