using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Analysis;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using static Lykke.RabbitMqBroker.Abstractions.Analysis.MessageDeliveryAnalysis.MessageDeliveryAnalysisVerdict;
using Gens = Lykke.RabbitMqBroker.TestDataGenerators.MessageDeliveryGens;

using Microsoft.Extensions.Time.Testing;

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
    public void Analyze_FairDelayNotExpired_NotDelivered_Returns_Verdict_NotDeliveredYet()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelaySeconds in Gen.Choose(1, 60)
            from timePassedSeconds in Gen.Choose(0, fairDelaySeconds - 1)
            let initialTime = dispatched.DispatchedTimestamp.Value
            let fairDelay = TimeSpan.FromSeconds(fairDelaySeconds)
            let timePassed = TimeSpan.FromSeconds(timePassedSeconds)
            let timeProvider = new FakeTimeProvider(initialTime)
            select (dispatched, fairDelay, timePassed, timeProvider)
        ).ToArbitrary(),
        inputs =>
        {
            var (dispatched, fairDelay, timePassed, timeProvider) = inputs;
            timeProvider.Advance(timePassed);
            return dispatched.Analyze(fairDelay, timeProvider) == NotDeliveredYet;

        }).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_FairDelayNotExpired_Delivered_Returns_Verdict_DeliveredOnTime()
    {
        Prop.ForAll((
            from received in Gens.Received
            let dispatchedTime = received.DispatchedTimestamp.Value
            let receivedTime = received.ReceivedTimestamp.Value
            let initialTime = dispatchedTime
            let timePassed = receivedTime - dispatchedTime
            let fairDelay = timePassed + TimeSpan.FromSeconds(1)
            let timeProvider = new FakeTimeProvider(initialTime)
            select (received, fairDelay, timePassed, timeProvider)
        ).ToArbitrary(),
        inputs =>
        {
            var (received, fairDelay, timePassed, timeProvider) = inputs;
            timeProvider.Advance(timePassed);
            return received.Analyze(fairDelay, timeProvider) == DeliveredOnTime;
        }).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_Expired_NotDelivered_Returns_Verdict_NotDelivered()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelaySeconds in Gen.Choose(1, 60)
            let timePassedSeconds = fairDelaySeconds + 1
            let initialTime = dispatched.DispatchedTimestamp.Value
            let fairDelay = TimeSpan.FromSeconds(fairDelaySeconds)
            let timePassed = TimeSpan.FromSeconds(timePassedSeconds)
            let timeProvider = new FakeTimeProvider(initialTime)
            select (dispatched, fairDelay, timePassed, timeProvider)
        ).ToArbitrary(),
        inputs =>
        {
            var (dispatched, fairDelay, timePassed, timeProvider) = inputs;
            timeProvider.Advance(timePassed);
            return dispatched.Analyze(fairDelay, timeProvider) == NotDelivered;
        }).QuickCheckThrowOnFailure();
    }

    [Test]
    public void Analyze_Expired_Delivered_Returns_Verdict_LatelyDelivered()
    {
        Prop.ForAll((
            from received in Gens.Received
            let dispatchedTime = received.DispatchedTimestamp.Value
            let receivedTime = received.ReceivedTimestamp.Value
            let initialTime = dispatchedTime
            let timePassed = receivedTime - dispatchedTime
            let fairDelay = timePassed - TimeSpan.FromSeconds(1)
            let timeProvider = new FakeTimeProvider(initialTime)
            select (received, fairDelay, timePassed, timeProvider)
        ).ToArbitrary(),
        inputs =>
        {
            var (received, fairDelay, timePassed, timeProvider) = inputs;
            timeProvider.Advance(timePassed);
            return received.Analyze(fairDelay, timeProvider) == LatelyDelivered;
        }).QuickCheckThrowOnFailure();
    }
}