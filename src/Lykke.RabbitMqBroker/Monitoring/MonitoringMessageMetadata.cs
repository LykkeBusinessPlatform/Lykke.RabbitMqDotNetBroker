namespace Lykke.RabbitMqBroker.Monitoring;

readonly record struct MonitoringMessageMetadata(string ExchangeName, string RoutingKey, string RouteText);
