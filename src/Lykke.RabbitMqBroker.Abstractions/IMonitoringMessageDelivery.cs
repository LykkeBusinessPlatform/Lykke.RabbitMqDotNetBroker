namespace Lykke.RabbitMqBroker.Abstractions;

public interface IMonitoringMessageDelivery
{
    Guid Id { get; }
    Guid MessageId { get; }
    DateTime? DispatchedTimestamp { get; }
    DateTime? ReceivedTimestamp { get; }
    MonitoringMessageDeliveryStatus Status { get; }
    MonitoringMessageDeliveryFailureReason? FailureReason { get; }
}