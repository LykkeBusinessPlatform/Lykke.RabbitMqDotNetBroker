using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Tests.MonitoringMessageSenderTests;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHandlerTests;

[TestFixture]
internal sealed class HandleTests
{
    private FakeMonitoringMessagePublisher _fakeMonitoringMessagePublisher;

    [SetUp]
    public void SetUp()
    {
        _fakeMonitoringMessagePublisher = new FakeMonitoringMessagePublisher();
    }

    [Test]
    public async Task Handle_Publishes_Single_MonitoringMessage()
    {
        var sut = CreateSut();

        await sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue"));

        Assert.That(_fakeMonitoringMessagePublisher.PublicationsCount, Is.EqualTo(1));
    }

    private MonitoringHandler CreateSut()
    {
        return new MonitoringHandler(_fakeMonitoringMessagePublisher);
    }
}