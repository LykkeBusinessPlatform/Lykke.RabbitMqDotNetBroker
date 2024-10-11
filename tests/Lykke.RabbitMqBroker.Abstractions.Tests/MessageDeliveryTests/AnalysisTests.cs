using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
[NonParallelizable]
internal sealed class AnalysisTests
{
    private FakeTimeProvider _timeProvider;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
    }

    [Test]
    public void GetLastActionTimestamp_WhenBrokerCustodyNotConfirmed_ShouldBeEqualToFailureTimestamp()
    {
        var delivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        _timeProvider.Advance(TimeSpan.FromMinutes(1));

        delivery = delivery.TrySetFailed(MessageDeliveryFailure.Create(
            MessageDeliveryFailureReason.BrokerCustodyNotConfirmed,
            dateTime: _timeProvider.GetUtcNow().DateTime));

        Assert.That(delivery.GetLastActionTimestamp(), Is.EqualTo(delivery.Failure.Timestamp));
    }

    [Test]
    public void GetLastActionTimestamp_WhenBrokerCustodyConfirmed_ShouldBeEqualToReceivedTimestamp()
    {
        var delivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        _timeProvider.Advance(TimeSpan.FromSeconds(5));

        delivery = delivery.TrySetReceived(_timeProvider.GetUtcNow().DateTime);

        Assert.That(delivery.GetLastActionTimestamp(), Is.EqualTo(delivery.ReceivedTimestamp));
    }

    [Test]
    public void GetLastActionTimestamp_WhenDispatchedOnly_ShouldBeEqualToDispatchedTimestamp()
    {
        var delivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        Assert.That(delivery.GetLastActionTimestamp(), Is.EqualTo(delivery.DispatchedTimestamp));
    }

    [Test]
    public void GetLastActionTimestamp_WhenNotDispatched_ShouldBeEqualToMinValue()
    {
        var delivery = new MessageDeliveryWithDefaults();

        Assert.That(delivery.GetLastActionTimestamp(), Is.EqualTo(DateTime.MinValue));
    }

    [Test]
    public void Delivered_WhenFailed_ShouldBeFalse()
    {
        var delivery = new MessageDeliveryWithDefaults()
            .TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.DispatchError));

        Assert.That(delivery.Delivered(), Is.False);
    }

    [Test]
    public void Delivered_WhenDispatchedOnly_ShouldBeFalse()
    {
        var delivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        Assert.That(delivery.Delivered(), Is.False);
    }

    [Test]
    public void Delivered_When_NotDispatched_ShouldBeFalse()
    {
        var delivery = new MessageDeliveryWithDefaults();

        Assert.That(delivery.Delivered(), Is.False);
    }

    [Test]
    public void Delivered_WhenReceived_ShouldBeTrue()
    {
        var delivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime)
            .TrySetReceived(_timeProvider.GetUtcNow().DateTime);

        Assert.Multiple(() =>
        {
            Assert.That(delivery.Delivered());
            Assert.That(delivery.NotDelivered(), Is.False);
        });
    }
}