using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker.Monitoring;

readonly record struct MonitoringHeartbeatMetadata(ExchangeName ExchangeName, RoutingKey RoutingKey, string RouteText);
