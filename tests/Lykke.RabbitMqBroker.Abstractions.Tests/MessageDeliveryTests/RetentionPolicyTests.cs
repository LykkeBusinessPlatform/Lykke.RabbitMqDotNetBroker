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
        var delivery = Gens.Pending.Sample(1, 1).Single();

        MinutesPassedBy(1);
        var result = delivery.IsOlderThan(GetNow);

        Assert.That(result, Is.False);
    }

    [Test]
    public void IsOlderThan_WhenDispatched_ShouldBeTrue()
    {
        var dispatchedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(GetNow());

        MinutesPassedBy(1);
        var result = dispatchedDelivery.IsOlderThan(GetNow);

        Assert.That(result);
    }

    [Test]
    public void IsOlderThan_WhenFailed_ShouldBeTrue()
    {
        var failedDelivery = new MessageDeliveryWithDefaults()
            .TrySetFailed(
                MessageDeliveryFailure.Create(
                    MessageDeliveryFailureReason.DispatchError,
                    dateTime: GetNow()));

        MinutesPassedBy(1);
        var result = failedDelivery.IsOlderThan(GetNow);

        Assert.That(result);
    }

    [Test]
    public void IsOlderThan_WhenMomentIsBeforeReceived_ShouldBeTrue()
    {
        var dispatchedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(GetNow());

        MinutesPassedBy(1);
        var moment = GetNow();

        MinutesPassedBy(1);
        var receivedDelivery = dispatchedDelivery
            .TrySetReceived(GetNow());

        var result = receivedDelivery.IsOlderThan(moment);

        Assert.That(result);
    }

    [Test]
    public void IsOlderThan_WhenMomentExactlyDispatched_ShouldBeFalse()
    {
        var dispatchedDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(GetNow());

        var result = dispatchedDelivery.IsOlderThan(GetNow);

        Assert.That(result, Is.False);
    }

    private void MinutesPassedBy(int minutes) => _timeProvider.Advance(TimeSpan.FromMinutes(minutes));

    private DateTime GetNow() => _timeProvider.GetUtcNow().DateTime;
}