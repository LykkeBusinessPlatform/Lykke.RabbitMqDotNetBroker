namespace Lykke.RabbitMqBroker.Abstractions;

public enum MonitoringMessageDeliveryStatus
{
    Pending = 0,
    Dispatched = 10,
    BrokerCustodyConfirmed = 20,
    Received = 30,
    Failed = 40
}
