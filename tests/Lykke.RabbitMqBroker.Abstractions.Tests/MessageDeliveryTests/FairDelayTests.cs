using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

[TestFixture]
public class FairDelayTests
{
    private static readonly TimeSpan DefaultFairDelay = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan MoreThanDefaultFairDelay = DefaultFairDelay.Add(TimeSpan.FromSeconds(1));
    private static readonly TimeSpan LessThanDefaultFairDelay = DefaultFairDelay.Subtract(TimeSpan.FromSeconds(1));

    private FakeTimeProvider _timeProvider;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
    }

    [Test]
    public void When_DispatchTimestamp_IsEmpty_Returns_False()
    {
        var message = new MessageDeliveryWithNoTimestamps();

        var expired = message.FairDelayExpired(DefaultFairDelay, _timeProvider);

        Assert.That(expired, Is.False);
    }

    [Test]
    public void When_NotReceivedYet_Uses_CurrentTime_Expired()
    {
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        _timeProvider.Advance(MoreThanDefaultFairDelay);

        var expired = message.FairDelayExpired(DefaultFairDelay, _timeProvider);

        Assert.That(expired, Is.True);
    }

    [Test]
    public void When_NotReceivedYet_Uses_CurrentTime_NotExpired()
    {
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        _timeProvider.Advance(LessThanDefaultFairDelay);

        var expired = message.FairDelayExpired(DefaultFairDelay, _timeProvider);

        Assert.That(expired, Is.False);
    }


    [Test]
    public void When_Received_Uses_ReceivedTimestamp_Expired()
    {
        var now = _timeProvider.GetUtcNow().DateTime;
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(now)
            .TrySetReceived(now.Add(MoreThanDefaultFairDelay));

        var expired = message.FairDelayExpired(DefaultFairDelay, _timeProvider);

        Assert.That(expired, Is.True);
    }

    [Test]
    public void When_Received_Uses_ReceivedTimestamp_NotExpired()
    {
        var now = _timeProvider.GetUtcNow().DateTime;
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(now)
            .TrySetReceived(now.Add(LessThanDefaultFairDelay));

        var expired = message.FairDelayExpired(DefaultFairDelay, _timeProvider);

        Assert.That(expired, Is.False);
    }
}