using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Tests.MonitoringMessageSenderTests;
using MonitoringListenerRegistrationHandler = Lykke.RabbitMqBroker.Monitoring.ListenerRegistrationMonitoringHandler;

using NUnit.Framework;
using Lykke.RabbitMqBroker.Subscriber;

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

        await sut.Handle(new ListenerRegistration<MessageModel>(ListenerRoute.Create(new ExchangeName("exchange"), new QueueName("queue"))));

        Assert.That(_fakeMonitoringMessagePublisher.PublicationsCount, Is.EqualTo(1));
    }

    private MonitoringListenerRegistrationHandler CreateSut()
    {
        return new MonitoringListenerRegistrationHandler(_fakeMonitoringMessagePublisher);
    }
}