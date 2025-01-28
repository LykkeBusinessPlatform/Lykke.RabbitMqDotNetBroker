using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
public class StatusTests
{
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
        Prop.ForAll(
            Gens.MessageDelivery.Pending.ToArbitrary(),
            pending => pending.TrySetDispatched(DateTime.UtcNow).GetStatus() == MessageDeliveryStatus.Dispatched
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void DispatchedMessageDelivery_HasDispatchedStatus()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Dispatched.ToArbitrary(),
            dispatched => dispatched.GetStatus() == MessageDeliveryStatus.Dispatched
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_WhenAlreadyDispatched()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Dispatched.ToArbitrary(),
            dispatched => dispatched.TrySetDispatched(DateTime.UtcNow) == dispatched
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_WhenAlreadyReceived()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Received.ToArbitrary(),
            received => received.TrySetDispatched(DateTime.UtcNow) == received
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetDispatched_Does_Nothing_WhenAlreadyFailed()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Failed.ToArbitrary(),
            failed => failed.TrySetDispatched(DateTime.UtcNow) == failed
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetReceived_ChangesStatusToReceived_When_Dispatched()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Dispatched.ToArbitrary(),
            dispatched => dispatched.TrySetReceived(DateTime.UtcNow).GetStatus() == MessageDeliveryStatus.Received
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetReceived_Does_Nothing_WhenAlreadyReceived()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Received.ToArbitrary(),
            received => received.TrySetReceived(DateTime.UtcNow) == received
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetReceived_Does_Nothing_WhenAlreadyFailed()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Failed.ToArbitrary(),
            failed => failed.TrySetReceived(DateTime.UtcNow) == failed
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetReceived_Does_Nothing_WhenPending()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Pending.ToArbitrary(),
            pending => pending.TrySetReceived(DateTime.UtcNow) == pending
        ).QuickCheckThrowOnFailure();
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
        Prop.ForAll((
            from notFailed in Gens.MessageDelivery.NotFailed
            from failure in Gens.MessageDeliveryFailure
            select (notFailed, failure)
            ).ToArbitrary(),
            inputs =>
            {
                var (notFailed, failure) = inputs;
                var failed = notFailed.TrySetFailed(failure);
                return failed.GetStatus() == MessageDeliveryStatus.Failed && !failed.Failure.IsEmpty;
            }
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetFailed_Cleans_DispatchTimestamp()
    {
        Prop.ForAll((
            from dispatched in Gens.MessageDelivery.Dispatched
            from failure in Gens.MessageDeliveryFailure
            select (dispatched, failure)
            ).ToArbitrary(),
            inputs =>
            {
                var (dispatched, failure) = inputs;
                return dispatched.TrySetFailed(failure).DispatchedTimestamp == null;
            }
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void TrySetFailed_ChangesStatusToFailed_WhenReceived()
    {
        Prop.ForAll((
            from received in Gens.MessageDelivery.Received
            from failure in Gens.MessageDeliveryFailure
            select (received, failure)
            ).ToArbitrary(),
            inputs =>
            {
                var (received, failure) = inputs;
                var failed = received.TrySetFailed(failure);
                return failed.GetStatus() == MessageDeliveryStatus.Failed && !failed.Failure.IsEmpty;
            }
        );
    }

    [Test]
    public void TrySetFailed_Does_Nothing_WhenEmptyFailure()
    {
        var notFailed = Gens.MessageDelivery.NotFailed.Sample(1, 1).First();

        var updatedDelivery = notFailed.TrySetFailed(MessageDeliveryFailure.Empty);

        Assert.That(updatedDelivery, Is.EqualTo(notFailed));
    }

    [Test]
    public void TrySetFailed_Does_Nothing_WhenAlreadyFailed()
    {
        Prop.ForAll((
            from failed in Gens.MessageDelivery.Failed
            from failure in Gens.MessageDeliveryFailure
            select (failed, failure)
            ).ToArbitrary(),
            inputs =>
            {
                var (failed, failure) = inputs;
                return failed.TrySetFailed(failure) == failed;
            });
    }

    [Test]
    public void TrySetFailed_Does_Nothing_ToEmptyDelivery()
    {
        var noDelivery = MessageDelivery.None;

        var updatedDelivery = noDelivery.TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.None));

        Assert.That(updatedDelivery, Is.EqualTo(noDelivery));
    }
}