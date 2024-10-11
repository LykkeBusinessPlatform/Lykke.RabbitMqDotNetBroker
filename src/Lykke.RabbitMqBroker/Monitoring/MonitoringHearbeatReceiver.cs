using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class MonitoringHeartbeatReceiver : IMonitoringHeartbeatReceiver
{
    private readonly IMessageDeliveryStorage _messageDeliveryStorage;

    public MonitoringHeartbeatReceiver(IMessageDeliveryStorage messageDeliveryStorage)
    {
        _messageDeliveryStorage = messageDeliveryStorage;
    }

    public Task Handle(ReadOnlyMemory<byte> body, MessageDeliveryId deliveryId) =>
        _messageDeliveryStorage.TrySetReceived(deliveryId);
}