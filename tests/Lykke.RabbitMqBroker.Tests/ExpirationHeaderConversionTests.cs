using System;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class ExpirationHeaderConversionTests
{
    [Test]
    public void ToExpirationMilliseconds_ValidValueWithoutFraction_ReturnsUint()
    {
        double src = 12345;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(12345u));
    }

    [Test]
    public void ToExpirationMilliseconds_ValidValueWithFraction_ReturnsFlooredUint()
    {
        double src = 12345.67;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(12345u));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueExceedsUIntMax_ThrowsArgumentOutOfRangeException()
    {
        double src = ulong.MaxValue + 1.0;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => src.ToExpirationMilliseconds());

        Assert.That(ex.Message, Does.Contain("too large"));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueBelowUIntMin_ThrowsArgumentOutOfRangeException()
    {
        double src = ulong.MinValue - 1.0;

        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => src.ToExpirationMilliseconds());

        Assert.That(ex.Message, Does.Contain("too small"));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueAtUIntMax_ReturnsUint()
    {
        double src = ulong.MaxValue;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MaxValue));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueAtUIntMin_ReturnsUint()
    {
        double src = ulong.MinValue;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MinValue));
    }
}
