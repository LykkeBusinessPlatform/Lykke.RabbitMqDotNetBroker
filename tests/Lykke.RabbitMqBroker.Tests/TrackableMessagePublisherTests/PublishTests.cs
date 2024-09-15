using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Publisher;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests.TrackableMessagePublisherTests;


[TestFixture]
internal class PublishTests
{
    private IMessageDeliveryStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _storage = new MessageDeliveryInMemoryStorage();
    }

    [Test]
    public async Task Publish_When_Message_Not_Confirmed_Then_Delivery_Fails()
    {
        var publisher = new TrackableMessagePublisher<MessageModel>(
            new UnconfirmedMessagePurePublisher(),
            _storage);

        var deliveryId = await publisher.Publish(new ReadOnlyMemory<byte>(), null, null, null);

        var delivery = await _storage.Get(deliveryId);

        Assert.Multiple(() =>
        {
            Assert.That(delivery?.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
            Assert.That(delivery?.Failure?.Reason, Is.EqualTo(MessageDeliveryFailureReason.BrokerCustodyNotConfirmed));
        });
    }

    [Test]
    public async Task Publish_When_Unexpected_Exception_Then_Delivery_Fails()
    {
        var publisher = new TrackableMessagePublisher<MessageModel>(
            new UnexpectedExceptionPurePublisher(),
            _storage);

        var deliveryId = await publisher.Publish(new ReadOnlyMemory<byte>(), null, null, null);

        var delivery = await _storage.Get(deliveryId);

        Assert.Multiple(() =>
        {
            Assert.That(delivery?.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
            Assert.That(delivery?.Failure?.Reason, Is.EqualTo(MessageDeliveryFailureReason.DispatchError));
        });
    }

    [Test]
    public async Task Publish_WhenSucceeded_Then_Delivery_Succeeds()
    {
        var publisher = new TrackableMessagePublisher<MessageModel>(
            new SuccessMessagePurePublisher(),
            _storage);

        var deliveryId = await publisher.Publish(new ReadOnlyMemory<byte>(), null, null, null);

        var delivery = await _storage.Get(deliveryId);

        Assert.Multiple(() =>
        {
            Assert.That(delivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Dispatched));
            Assert.That(delivery.Failure.IsEmpty);
        });
    }
}