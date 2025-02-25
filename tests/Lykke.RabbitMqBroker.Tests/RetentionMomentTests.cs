using System;

using Microsoft.Extensions.Time.Testing;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
[NonParallelizable]
internal sealed class RetentionMomentTests
{
    private FakeTimeProvider _timeProvider;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
    }

    [Test]
    public void From_WhenRetentionPeriodIsZeroOrNegative_ShouldThrowArgumentOutOfRangeException()
    {
        var invalidRetentionPeriod = TimeSpan.Zero;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            RetentionMoment.From(invalidRetentionPeriod, _timeProvider));
    }

    [Test]
    public void From_WhenTimeProviderIsNull_ShouldThrowArgumentNullException()
    {
        var retentionPeriod = TimeSpan.FromDays(10);

        Assert.Throws<ArgumentNullException>(() =>
            RetentionMoment.From(retentionPeriod, null));
    }

    [Test]
    public void From_WhenRetentionMomentGoesBelowMinValue_ShouldReturnMinValue()
    {
        var retentionMoment = RetentionMoment.From(GetTimeSpanExceedingCurrentDate(), _timeProvider);

        Assert.That((DateTime)retentionMoment, Is.EqualTo(DateTime.MinValue));
    }

    private TimeSpan GetTimeSpanExceedingCurrentDate()
    {
        var longerTicks = _timeProvider.GetUtcNow().AddYears(1).Ticks;
        return TimeSpan.FromTicks(longerTicks);
    }
}