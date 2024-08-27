namespace Lykke.RabbitMqBroker.Abstractions;

public enum MonitoringMessageDeliveryFailureReason
{
    DispatchError = 0,
    BrokerCustodyNotConfirmed = 10
}
