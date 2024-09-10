using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Publisher;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHandlerTests;

internal class FakeMonitoringMessagePublisher : IMessageProducer<MonitoringMessage>
{
    public int PublicationsCount { get; private set; }
    public Task ProduceAsync(MonitoringMessage message)
    {
        PublicationsCount++;
        return Task.CompletedTask;
    }
}