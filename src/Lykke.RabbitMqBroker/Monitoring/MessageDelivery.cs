using System;

using Lykke.RabbitMqBroker.Abstractions;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class MessageDelivery(Guid messageId) : IMonitoringMessageDelivery
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid MessageId { get; } = messageId;
    public DateTime? DispatchedTimestamp { get; private set; }
    public DateTime? ReceivedTimestamp { get; private set; }
    public MonitoringMessageDeliveryStatus Status { get; private set; } = MonitoringMessageDeliveryStatus.Pending;
    public MonitoringMessageDeliveryFailureReason? FailureReason { get; private set; }

    public void SetDispatched()
    {
        DispatchedTimestamp = DateTime.UtcNow;
        Status = MonitoringMessageDeliveryStatus.Dispatched;
    }

    public void SetBrokerCustodyConfirmed()
    {
        Status = MonitoringMessageDeliveryStatus.BrokerCustodyConfirmed;
    }

    public void SetReceived()
    {
        ReceivedTimestamp = DateTime.UtcNow;
        Status = MonitoringMessageDeliveryStatus.Received;
    }

    public void SetFailed(MonitoringMessageDeliveryFailureReason reason)
    {
        FailureReason = reason;
        Status = MonitoringMessageDeliveryStatus.Failed;
    }
}