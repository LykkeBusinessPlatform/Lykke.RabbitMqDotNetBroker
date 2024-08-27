using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageSenderTests;

class FakeMonitoringMessageChannelProvider : IMonitoringMessageChannelProvider
{
    public FakeChannel LatestChannel { get; private set; }

    public IModel Create()
    {
        LatestChannel = new FakeChannel();
        return LatestChannel;
    }
}