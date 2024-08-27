using System;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageSenderTests;

[TestFixture]
internal sealed class HandleTests
{
    private FakeMonitoringMessageChannelProvider _channelProvider;

    [SetUp]
    public void SetUp()
    {
        _channelProvider = new FakeMonitoringMessageChannelProvider();
    }

    [Test]
    public void Handle_Publishes_Single_MonitoringMessage()
    {
        var sut = CreateSut();

        sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue"));

        Assert.That(_channelProvider.LatestChannel.PublicationsCount, Is.EqualTo(1));
    }

    [Test]
    public void Handle_Waits_For_Confirmation()
    {
        var sut = CreateSut();

        sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue"));

        Assert.That(_channelProvider.LatestChannel.WaitForConfirmsOrDieCount, Is.EqualTo(1));
    }

    [Test]
    public void Handle_Disposes_Channel()
    {
        var sut = CreateSut();

        sut.Handle(new ListenerRegistration<MessageModel>("exchange", "queue"));

        Assert.That(_channelProvider.LatestChannel.Disposed);
    }

    private MonitoringMessageSender CreateSut()
    {
        return new MonitoringMessageSender(
            _channelProvider,
            new MonitoringMessagePropertiesFactory(
                new RabbitMqMonitoringConfiguration
                {
                    DeliveryMode = 1
                }),
            TimeSpan.FromSeconds(1));
    }
}