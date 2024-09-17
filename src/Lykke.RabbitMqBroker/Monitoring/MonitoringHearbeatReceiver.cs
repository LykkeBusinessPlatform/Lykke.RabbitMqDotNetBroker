using System;
using System.Text;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

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

    public async Task Handle(ReadOnlyMemory<byte> body, MessageDeliveryId deliveryId)
    {
        var heartbeat = JsonConvert.DeserializeObject<MonitoringHeartbeat>(Encoding.UTF8.GetString(body.Span));

        _logger.LogDebug("Received heartbeat message: {Heartbeat}", heartbeat);

        var updated = await _messageDeliveryStorage.TrySetReceived(deliveryId);

        _logger.LogDebug("DeliveryId {DeliveryId} marked as received: {Updated}", deliveryId, updated);
    }
}