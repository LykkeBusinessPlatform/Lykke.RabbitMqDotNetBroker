using System;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class ExpirationHeaderConversionTests
{
    [Test]
    public void ToExpirationMilliseconds_ValidValueWithoutFraction_ReturnsUlong()
    {
        double src = 12345;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(12345ul));
    }

    [Test]
    public void ToExpirationMilliseconds_ValidValueWithFraction_ReturnsFlooredUlong()
    {
        double src = 12345.67;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(12345ul));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueExceedsULongMax_ThrowsArgumentOutOfRangeException()
    {
        double src = (double)2 * ulong.MaxValue;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => src.ToExpirationMilliseconds());

        Assert.That(ex.Message, Does.Contain("too large"));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueBelowULongMin_ThrowsArgumentOutOfRangeException()
    {
        double src = ulong.MinValue - 1.0;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => src.ToExpirationMilliseconds());

        Assert.That(ex.Message, Does.Contain("too small"));
    }

    [Test]
    [Repeat(100)]
    public void ToExpirationMilliseconds_ValueAtULongMax_ReturnsUlong()
    {
        double src = ulong.MaxValue;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.InRange(ulong.MaxValue - 1, ulong.MaxValue));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueAtULongMin_ReturnsUlong()
    {
        double src = ulong.MinValue;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MinValue));
    }
}
