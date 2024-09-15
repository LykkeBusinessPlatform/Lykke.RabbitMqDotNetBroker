using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class MonitoringHeartbeatReceiver : IMonitoringHeartbeatReceiver
{
    private readonly IMessageDeliveryStorage _messageDeliveryStorage;
    private readonly ILogger<MonitoringHeartbeatReceiver> _logger;

    public MonitoringHeartbeatReceiver(IMessageDeliveryStorage messageDeliveryStorage, ILogger<MonitoringHeartbeatReceiver> logger)
    {
        _messageDeliveryStorage = messageDeliveryStorage;
        _logger = logger;
    }

    public async Task Handle(MonitoringHeartbeat heartbeat, MessageDeliveryId deliveryId)
    {
        _logger.LogDebug("Received heartbeat message: {heartbeat}", heartbeat);


        if (!deliveryId.IsEmpty)
        {
            await _messageDeliveryStorage.TrySetReceived(deliveryId);
            _logger.LogDebug("DeliveryId {deliveryId} marked as received", deliveryId);
        }
    }
}