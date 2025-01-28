using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
internal sealed class AnalysisTests
{
    [Test]
    public void GetLastActionTimestamp_WhenFailed_ShouldBeEqualToFailureTimestamp()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Failed.ToArbitrary(),
            failed => failed.GetLastActionTimestamp() == failed.Failure.Timestamp
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void GetLastActionTimestamp_WhenReceived_ShouldBeEqualToReceivedTimestamp()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Received.ToArbitrary(),
            received => received.GetLastActionTimestamp() == received.ReceivedTimestamp
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void GetLastActionTimestamp_WhenDispatched_ShouldBeEqualToDispatchedTimestamp()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Dispatched.ToArbitrary(),
            dispatched => dispatched.GetLastActionTimestamp() == dispatched.DispatchedTimestamp
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void GetLastActionTimestamp_WhenPending_ShouldBeNull()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Pending.ToArbitrary(),
            pending => pending.GetLastActionTimestamp() == null
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_WhenFailed_ShouldBeFalse()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Failed.ToArbitrary(),
            failed => failed.Delivered() == false
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_WhenDispatched_ShouldBeFalse()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Dispatched.ToArbitrary(),
            dispatched => dispatched.Delivered() == false
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_When_Pending_ShouldBeFalse()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Pending.ToArbitrary(),
            pending => pending.Delivered() == false
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_WhenReceived_ShouldBeTrue()
    {
        Prop.ForAll(
            Gens.MessageDelivery.Received.ToArbitrary(),
            received => received.Delivered() == true
        ).QuickCheckThrowOnFailure();
    }
}