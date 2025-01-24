using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
public class StatusTests
{
    [Test]
    public void NewMessageDelivery_HasPendingStatus()
    {
        var delivery = new MessageDeliveryWithDefaults();

        Assert.That(delivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Pending));
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_To_Empty_Delivery()
    {
        var noDelivery = MessageDelivery.None;

        var dispatchedDelivery = noDelivery.TrySetDispatched(DateTime.UtcNow);

        Assert.That(dispatchedDelivery, Is.EqualTo(noDelivery));
    }

    [Test]
    public void TrySetDispatched_ChangesStatusToDispatched_WhenPending()
    {
        var pendingDelivery = new MessageDeliveryWithDefaults();

        var dispatchedDelivery = pendingDelivery.TrySetDispatched(DateTime.UtcNow);

        Assert.That(dispatchedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Dispatched));
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_WhenAlreadyDispatched()
    {
        var dispatchedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow);

        var dispatchedDelivery2 = dispatchedDelivery.TrySetDispatched(DateTime.UtcNow);

        Assert.That(dispatchedDelivery2, Is.EqualTo(dispatchedDelivery));
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_WhenAlreadyReceived()
    {
        var receivedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetReceived(DateTime.UtcNow);

        var updatedDelivery = receivedDelivery.TrySetDispatched(DateTime.UtcNow);

        Assert.That(updatedDelivery, Is.EqualTo(receivedDelivery));
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_WhenAlreadyFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None);
        var failedDelivery = new MessageDeliveryWithDefaults()
            .TrySetFailed(failure);

        var updatedDelivery = failedDelivery.TrySetDispatched(DateTime.UtcNow);

        Assert.That(updatedDelivery, Is.EqualTo(failedDelivery));
    }


    [Test]
    public void TrySetReceived_ChangesStatusToReceived()
    {
        var receivedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetReceived(DateTime.UtcNow);

        Assert.That(receivedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Received));
    }

    [Test]
    public void TrySetReceived_Does_Nothing_WhenAlreadyReceived()
    {
        var receivedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetReceived(DateTime.UtcNow);

        var updatedDelivery = receivedDelivery.TrySetReceived(DateTime.UtcNow);

        Assert.That(updatedDelivery, Is.EqualTo(receivedDelivery));
    }

    [Test]
    public void TrySetReceived_Does_Nothing_WhenAlreadyFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None);
        var failedDelivery = new MessageDeliveryWithDefaults()
            .TrySetFailed(failure);

        var updatedDelivery = failedDelivery.TrySetReceived(DateTime.UtcNow);

        Assert.That(updatedDelivery, Is.EqualTo(failedDelivery));
    }

    [Test]
    public void TrySetReceived_Does_Nothing_WhenNotDispatched()
    {
        var pendingDelivery = new MessageDeliveryWithDefaults();

        var updatedDelivery = pendingDelivery.TrySetReceived(DateTime.UtcNow);

        Assert.That(updatedDelivery, Is.EqualTo(pendingDelivery));
    }

    [Test]
    public void TrySetReceived_Does_Nothing_ToEmptyDelivery()
    {
        var noDelivery = MessageDelivery.None;

        var updatedDelivery = noDelivery.TrySetReceived(DateTime.UtcNow);

        Assert.That(updatedDelivery, Is.EqualTo(noDelivery));
    }

    [Test]
    public void TrySetFailed_ChangesStatusToFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None);
        var failedDelivery = new MessageDeliveryWithDefaults().TrySetFailed(failure);

        Assert.Multiple(() =>
        {
            Assert.That(failedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
            Assert.That(failedDelivery.Failure.IsEmpty, Is.False);
        });
    }

    [Test]
    public void TrySetFailed_ChangesStatusToFailed_WhenDispatched()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None);
        var failedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetFailed(failure);

        Assert.Multiple(() =>
        {
            Assert.That(failedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
            Assert.That(failedDelivery.Failure.IsEmpty, Is.False);
        });
    }

    [Test]
    public void TrySetFailed_ChangesStatusToFailed_WhenReceived()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None);
        var failedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetReceived(DateTime.UtcNow)
            .TrySetFailed(failure);

        Assert.Multiple(() =>
        {
            Assert.That(failedDelivery.GetStatus(), Is.EqualTo(MessageDeliveryStatus.Failed));
            Assert.That(failedDelivery.Failure.IsEmpty, Is.False);
        });
    }

    [Test]
    public void TrySetFailed_Does_Nothing_WhenEmptyFailure()
    {
        var emptyFailure = MessageDeliveryFailure.Empty;
        var delivery = new MessageDeliveryWithDefaults();

        var updatedDelivery = delivery.TrySetFailed(emptyFailure);

        Assert.That(updatedDelivery, Is.EqualTo(delivery));
    }

    [Test]
    public void TrySetFailed_Does_Nothing_WhenAlreadyFailed()
    {
        var failure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None);
        var originallyFailedDelivery = new MessageDeliveryWithDefaults()
            .TrySetFailed(failure);

        var anotherFailure = MessageDeliveryFailure.Create(MessageDeliveryFailureReason.BrokerCustodyNotConfirmed);
        var updatedDelivery = originallyFailedDelivery.TrySetFailed(anotherFailure);

        Assert.That(updatedDelivery, Is.EqualTo(originallyFailedDelivery));
    }

    [Test]
    public void TrySetFailed_Does_Nothing_ToEmptyDelivery()
    {
        var noDelivery = MessageDelivery.None;

        var updatedDelivery = noDelivery.TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None));

        Assert.That(updatedDelivery, Is.EqualTo(noDelivery));
    }
}