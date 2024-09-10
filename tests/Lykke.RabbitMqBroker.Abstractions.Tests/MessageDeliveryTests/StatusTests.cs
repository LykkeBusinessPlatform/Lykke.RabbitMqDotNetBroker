using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
public class StatusTests
{
    [Test]
    public void NewMessageDelivery_HasPendingStatus()
    {
        MessageDelivery delivery = new();

        Assert.That(delivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Pending));
    }

    [Test]
    public void Dispatched_ChangesStatusToDispatched()
    {
        var dispatchedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow);

        Assert.That(dispatchedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Dispatched));
    }

    [Test]
    public void Dispatched_ThrowsException_WhenAlreadyDispatched()
    {
        var dispatchedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => dispatchedDelivery.Dispatched(DateTime.UtcNow));
    }

    [Test]
    public void Dispatched_ThrowsException_WhenReceived()
    {
        var receivedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow)
            .Received(DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => receivedDelivery.Dispatched(DateTime.UtcNow));
    }

    [Test]
    public void Dispatched_ThrowsException_WhenFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);
        var failedDelivery = new MessageDelivery()
            .Failed(failure);

        Assert.Throws<InvalidOperationException>(() => failedDelivery.Dispatched(DateTime.UtcNow));
    }

    [Test]
    public void Received_ChangesStatusToReceived()
    {
        var receivedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow)
            .Received(DateTime.UtcNow);

        Assert.That(receivedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Received));
    }

    [Test]
    public void Received_ThrowsException_WhenAlreadyReceived()
    {
        var receivedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow)
            .Received(DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => receivedDelivery.Received(DateTime.UtcNow));
    }

    [Test]
    public void Received_ThrowsException_WhenNotDispatched()
    {
        MessageDelivery delivery = new();

        Assert.Throws<InvalidOperationException>(() => delivery.Received(DateTime.UtcNow));
    }

    [Test]
    public void Received_ThrowsException_WhenFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);
        var failedDelivery = new MessageDelivery().Failed(failure);

        Assert.Throws<InvalidOperationException>(() => failedDelivery.Received(DateTime.UtcNow));
    }

    [Test]
    public void Failed_ChangesStatusToFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);
        var failedDelivery = new MessageDelivery().Failed(failure);

        Assert.That(failedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
    }

    [Test]
    public void Failed_ChangesStatusToFailed_WhenDispatched()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);
        var failedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow)
            .Failed(failure);

        Assert.That(failedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
    }

    [Test]
    public void Failed_ChangesStatusToFailed_WhenReceived()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);
        var failedDelivery = new MessageDelivery()
            .Dispatched(DateTime.UtcNow)
            .Received(DateTime.UtcNow)
            .Failed(failure);

        Assert.That(failedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
    }

    [Test]
    public void Failed_ThrowsException_WhenAlreadyFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Uncategorised);
        var failedDelivery = new MessageDelivery().Failed(failure);

        Assert.Throws<InvalidOperationException>(() => failedDelivery.Failed(failure));
    }
}