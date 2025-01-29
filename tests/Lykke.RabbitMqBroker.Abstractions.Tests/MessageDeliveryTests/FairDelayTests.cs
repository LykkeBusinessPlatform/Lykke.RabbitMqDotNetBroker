using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Gens = Lykke.RabbitMqBroker.TestDataGenerators.MessageDeliveryGens;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
public class FairDelayTests
{
    [Test]
    public void When_Pending_Always_Returns_False() // fair delay is not applicable to pending messages
    {
        Prop.ForAll((
            from pending in Gens.Pending
            from fairDelay in ArbMap.Default.GeneratorFor<TimeSpan>()
            select pending.Expired(fairDelay, DateTime.UtcNow)
            ).ToArbitrary(),
            expired => !expired
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void When_Dispatched_And_TimePassed_Returns_True()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelaySeconds in Gen.Choose(1, 60)
            from timePassedSeconds in Gen.Choose(61, 86400)
            let initialTime = dispatched.DispatchedTimestamp.Value
            let fairDelay = TimeSpan.FromSeconds(fairDelaySeconds)
            let timePassed = TimeSpan.FromSeconds(timePassedSeconds)
            let currentTime = initialTime + timePassed
            select dispatched.Expired(fairDelay, currentTime)
            ).ToArbitrary(),
            expired => expired
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void When_Dispatched_And_TimeNotPassed_Returns_False()
    {
        Prop.ForAll((
            from dispatched in Gens.Dispatched
            from fairDelaySeconds in Gen.Choose(1, 60)
            from timePassedSeconds in Gen.Choose(0, fairDelaySeconds - 1)
            let initialTime = dispatched.DispatchedTimestamp.Value
            let fairDelay = TimeSpan.FromSeconds(fairDelaySeconds)
            let timePassed = TimeSpan.FromSeconds(timePassedSeconds)
            let currentTime = initialTime + timePassed
            select dispatched.Expired(fairDelay, currentTime)
            ).ToArbitrary(),
            expired => !expired
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void When_Received_And_TimePassed_Returns_True()
    {
        Prop.ForAll((
            from received in Gens.Received
            let initialTime = received.DispatchedTimestamp.Value
            let timeOnTheWay = received.ReceivedTimestamp.Value - received.DispatchedTimestamp.Value
            let fairDelay = timeOnTheWay.Subtract(TimeSpan.FromSeconds(1)) // emulate always late delivery
            let currentTime = initialTime + timeOnTheWay
            select received.Expired(fairDelay, currentTime)
            ).ToArbitrary(),
            expired => expired
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void When_Received_And_TimeNotPassed_Returns_False()
    {
        Prop.ForAll((
            from received in Gens.Received
            let initialTime = received.DispatchedTimestamp.Value
            let timeOnTheWay = received.ReceivedTimestamp.Value - received.DispatchedTimestamp.Value
            let fairDelay = timeOnTheWay.Add(TimeSpan.FromSeconds(1)) // emulate always early delivery
            let currentTime = initialTime + timeOnTheWay
            select received.Expired(fairDelay, currentTime)
            ).ToArbitrary(),
            expired => !expired
        ).QuickCheckThrowOnFailure();
    }
}