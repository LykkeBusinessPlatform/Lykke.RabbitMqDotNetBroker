using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

using Gens = Lykke.RabbitMqBroker.TestDataGenerators.MessageDeliveryGens;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
[NonParallelizable]
internal sealed class MessageDeliveryRetentionPolicyTests
{
    private FakeTimeProvider _timeProvider;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
    }

    [Test]
    public void IsOlderThan_WhenPending_ShouldBeFalse()
    {
        Prop.ForAll((
            from pending in Gens.Pending
            from moment in Gen.Constant(_timeProvider.GetUtcNow().DateTime)
            let currentTime = moment.AddMinutes(1)
            select pending.IsOlderThan(currentTime)
            ).ToArbitrary(),
            isOlderThan => !isOlderThan
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void IsOlderThan_WhenDispatched_ShouldBeTrue()
    {
        Prop.ForAll((
            from moment in Gen.Constant(_timeProvider.GetUtcNow().DateTime)
            from dispatched in Gens.DispatchedAt(moment)
            let currentTime = moment.AddMinutes(1)
            select dispatched.IsOlderThan(currentTime)
            ).ToArbitrary(),
            isOlderThan => isOlderThan
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void IsOlderThan_WhenFailed_ShouldBeTrue()
    {
        Prop.ForAll((
            from failed in Gens.Failed
            let moment = failed.Failure.Timestamp
            let currentTime = moment.AddMinutes(1)
            select failed.IsOlderThan(currentTime)
            ).ToArbitrary(),
            isOlderThan => isOlderThan
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void IsOlderThan_WhenMomentIsBeforeReceived_ShouldBeTrue()
    {
        Prop.ForAll((
            from moment in Gen.Constant(_timeProvider.GetUtcNow().DateTime)
            from dispatched in Gens.DispatchedAt(moment)
            let someTimeAfterDispatch = moment.AddMinutes(1)
            let yetMoreTimeAfterDispatch = someTimeAfterDispatch.AddMinutes(1)
            let received = dispatched.TrySetReceived(yetMoreTimeAfterDispatch)
            select received.IsOlderThan(someTimeAfterDispatch)
            ).ToArbitrary(),
            isOlderThan => isOlderThan
        ).QuickCheckThrowOnFailure();
    }

    [Test]
    public void IsOlderThan_WhenMomentExactlyDispatched_ShouldBeFalse()
    {
        Prop.ForAll((
            from moment in Gen.Constant(_timeProvider.GetUtcNow().DateTime)
            from dispatched in Gens.DispatchedAt(moment)
            select dispatched.IsOlderThan(moment)
            ).ToArbitrary(),
            isOlderThan => !isOlderThan
        ).QuickCheckThrowOnFailure();
    }
}