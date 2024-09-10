namespace Lykke.RabbitMqBroker.Monitoring;

readonly record struct MonitoringMessageMetadata(NonEmptyString ExchangeName, string RoutingKey, string RouteText);
