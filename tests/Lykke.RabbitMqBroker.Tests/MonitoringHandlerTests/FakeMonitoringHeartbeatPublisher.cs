using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Publisher;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHandlerTests;

internal class FakeMonitoringHeartbeatPublisher : IMessageProducer<MonitoringHeartbeat>
{
    public int PublicationsCount { get; private set; }
    public Task ProduceAsync(MonitoringHeartbeat message)
    {
        PublicationsCount++;
        return Task.CompletedTask;
    }
}