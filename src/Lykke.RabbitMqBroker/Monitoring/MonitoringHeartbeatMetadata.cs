namespace Lykke.RabbitMqBroker.Monitoring;

readonly record struct MonitoringHeartbeatMetadata(string ExchangeName, string RoutingKey, string RouteText);
