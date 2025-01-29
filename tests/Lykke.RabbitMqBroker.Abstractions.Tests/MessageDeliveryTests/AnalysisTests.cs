using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Analysis;
using Lykke.RabbitMqBroker.Abstractions.Tracking;

using static Lykke.RabbitMqBroker.Abstractions.Analysis.MessageDeliveryAnalysis.MessageDeliveryAnalysisVerdict;

using Gens = Lykke.RabbitMqBroker.TestDataGenerators.MessageDeliveryGens;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
internal sealed class AnalysisTests
{
    [Test]
    public void GetLastActionTimestamp_WhenFailed_ShouldBeEqualToFailureTimestamp()
    {
        Prop.ForAll(
            Gens.Failed.ToArbitrary(),
            failed => failed.GetLastActionTimestamp() == failed.Failure.Timestamp
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void GetLastActionTimestamp_WhenReceived_ShouldBeEqualToReceivedTimestamp()
    {
        Prop.ForAll(
            Gens.Received.ToArbitrary(),
            received => received.GetLastActionTimestamp() == received.ReceivedTimestamp
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void GetLastActionTimestamp_WhenDispatched_ShouldBeEqualToDispatchedTimestamp()
    {
        Prop.ForAll(
            Gens.Dispatched.ToArbitrary(),
            dispatched => dispatched.GetLastActionTimestamp() == dispatched.DispatchedTimestamp
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void GetLastActionTimestamp_WhenPending_ShouldBeNull()
    {
        Prop.ForAll(
            Gens.Pending.ToArbitrary(),
            pending => pending.GetLastActionTimestamp() == null
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_WhenFailed_ShouldBeFalse()
    {
        Prop.ForAll(
            Gens.Failed.ToArbitrary(),
            failed => failed.Delivered() == false
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_WhenDispatched_ShouldBeFalse()
    {
        Prop.ForAll(
            Gens.Dispatched.ToArbitrary(),
            dispatched => dispatched.Delivered() == false
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_When_Pending_ShouldBeFalse()
    {
        Prop.ForAll(
            Gens.Pending.ToArbitrary(),
            pending => pending.Delivered() == false
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Delivered_WhenReceived_ShouldBeTrue()
    {
        Prop.ForAll(
            Gens.Received.ToArbitrary(),
            received => received.Delivered() == true
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void DeliveredOnTime_NotDelivered_Always_Returns_False()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelayInSeconds in Gen.Choose(1, 60)
            let fairDelay = TimeSpan.FromSeconds(fairDelayInSeconds)
            select dispatched.DeliveredOnTime(fairDelay)
        ).ToArbitrary(),
        deliveredOnTime => !deliveredOnTime
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void DeliveredOnTime_Received_Before_FairDelay_Expires_Returns_True()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from timeOnTheWayInSeconds in Gen.Choose(1, 10)
            from fairDelayInSeconds in Gen.Choose(11, 60)
            let fairDelay = TimeSpan.FromSeconds(fairDelayInSeconds)
            let timeOnTheWay = TimeSpan.FromSeconds(timeOnTheWayInSeconds)
            let receivedTimestamp = dispatched.DispatchedTimestamp.Value + timeOnTheWay
            select dispatched.TrySetReceived(receivedTimestamp).DeliveredOnTime(fairDelay)
        ).ToArbitrary(),
        deliveredOnTime => deliveredOnTime
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void DeliveredOnTime_Received_After_FairDelay_Expires_Returns_False()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from timeOnTheWayInSeconds in Gen.Choose(30, 60)
            from fairDelayInSeconds in Gen.Choose(1, 29)
            let fairDelay = TimeSpan.FromSeconds(fairDelayInSeconds)
            let timeOnTheWay = TimeSpan.FromSeconds(timeOnTheWayInSeconds)
            let receivedTimestamp = dispatched.DispatchedTimestamp.Value + timeOnTheWay
            select dispatched.TrySetReceived(receivedTimestamp).DeliveredOnTime(fairDelay)
        ).ToArbitrary(),
        deliveredOnTime => !deliveredOnTime
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_FairDelayNotExpired_NotDelivered_Returns_Verdict_NotDeliveredYet()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelaySeconds in Gen.Choose(1, 60)
            from timePassedSeconds in Gen.Choose(0, fairDelaySeconds - 1)
            let initialTime = dispatched.DispatchedTimestamp.Value
            let fairDelay = TimeSpan.FromSeconds(fairDelaySeconds)
            let timePassed = TimeSpan.FromSeconds(timePassedSeconds)
            let currentTime = initialTime + timePassed
            select dispatched.Analyze(fairDelay, currentTime)
        ).ToArbitrary(),
        verdict => verdict == NotDeliveredYet
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_FairDelayNotExpired_Delivered_Returns_Verdict_DeliveredOnTime()
    {
        Prop.ForAll((
            from received in Gens.Received
            let dispatchedTime = received.DispatchedTimestamp.Value
            let receivedTime = received.ReceivedTimestamp.Value
            let initialTime = dispatchedTime
            let currentTime = receivedTime
            let timePassed = currentTime - initialTime
            let fairDelay = timePassed + TimeSpan.FromSeconds(1)
            select received.Analyze(fairDelay, currentTime)
        ).ToArbitrary(),
        verdict => verdict == DeliveredOnTime
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_FairDelayExpired_NotDelivered_Returns_Verdict_NotDelivered()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelaySeconds in Gen.Choose(1, 60)
            let timePassedSeconds = fairDelaySeconds + 1
            let initialTime = dispatched.DispatchedTimestamp.Value
            let fairDelay = TimeSpan.FromSeconds(fairDelaySeconds)
            let timePassed = TimeSpan.FromSeconds(timePassedSeconds)
            let currentTime = initialTime + timePassed
            select dispatched.Analyze(fairDelay, currentTime)
        ).ToArbitrary(),
        verdict => verdict == NotDelivered
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_FairDelayExpired_Delivered_Returns_Verdict_LatelyDelivered()
    {
        Prop.ForAll((
            from received in Gens.Received
            let dispatchedTime = received.DispatchedTimestamp.Value
            let receivedTime = received.ReceivedTimestamp.Value
            let initialTime = dispatchedTime
            let timePassed = receivedTime - dispatchedTime
            let fairDelay = timePassed - TimeSpan.FromSeconds(1)
            let currentTime = receivedTime
            select received.Analyze(fairDelay, currentTime)
        ).ToArbitrary(),
        verdict => verdict == LatelyDelivered
        ).QuickCheckThrowOnFailure();
    }
}