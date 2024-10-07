using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;
using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Publisher;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.TrackableMessagePublisherTests;

[TestFixture]
internal class OnReturnedTests
{
    private IMessageDeliveryStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _storage = new MessageDeliveryInMemoryStorage();
    }

    [Test]
    public async Task Publish_When_Message_Unrouted_Then_Delivery_Fails()
    {
        var publisher = new TrackableMessagePublisher<MessageModel>(
            new UnroutedMessagePurePublisher(),
            _storage);

        var deliveryId = await publisher.Publish(new ReadOnlyMemory<byte>(), new MessageRouteWithDefaults(), null);

        // wait for emulated I/O operation to complete
        await Task.Delay(UnroutedMessagePurePublisher.IoEmulationDelayMs * 3);

        var delivery = await _storage.Get(deliveryId);

        Assert.Multiple(() =>
        {
            Assert.That(delivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
            Assert.That(delivery.Failure.Reason, Is.EqualTo(MessageDeliveryFailureReason.Unroutable));
        });
    }
}