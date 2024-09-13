using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

interface IMonitoringHeartbeatReceiver
{
    Task Handle(MonitoringHeartbeat heartbeat, MessageDeliveryId deliveryId);
}
