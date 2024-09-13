using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Tests.MonitoringMessageSenderTests;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHandlerTests;

[TestFixture]
internal class HandleTests
{
    private FakeMonitoringHeartbeatPublisher _fakeMonitoringMessagePublisher;

    [SetUp]
    public void SetUp()
    {
        _fakeMonitoringMessagePublisher = new FakeMonitoringHeartbeatPublisher();
    }

    [Test]
    public async Task Handle_Publishes_Single_MonitoringMessage()
    {
        var sut = CreateSut();

        await sut.Handle(ListenerRegistration<MessageModel>.Create("exchange", "queue"));

        Assert.That(_fakeMonitoringMessagePublisher.PublicationsCount, Is.EqualTo(1));
    }

    private MonitoringHandler CreateSut()
    {
        return new MonitoringHandler(_fakeMonitoringMessagePublisher);
    }
}