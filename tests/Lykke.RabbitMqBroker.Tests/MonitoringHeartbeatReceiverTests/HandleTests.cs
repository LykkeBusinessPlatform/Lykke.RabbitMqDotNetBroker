using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatReceiverTests;

[TestFixture]
internal sealed class HandleTests
{
    private FakeStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _storage = new FakeStorage();
    }

    [Test]
    public async Task Handle_MarksDeliveryAsReceived()
    {
        var receiver = new MonitoringHeartbeatReceiver(_storage);
        var deliveryId = MessageDeliveryId.Create();

        await receiver.Handle(Array.Empty<byte>(), deliveryId);

        Assert.That(_storage.Received, Contains.Item(deliveryId));
    }
}