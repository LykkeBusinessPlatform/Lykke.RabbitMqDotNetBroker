namespace Lykke.RabbitMqBroker.Monitoring;

public static class MonitoringMessagePublisherSettingsFactory
{
    public static RabbitMqSubscriptionSettings Create(
        string connectionString) => new()
        {
            ConnectionString = connectionString,
            ExchangeName = "to be overridden per message",
            RoutingKey = "to be overridden per message",
        };
}