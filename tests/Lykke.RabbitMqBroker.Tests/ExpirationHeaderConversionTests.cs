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
    public void ToExpirationMilliseconds_ValueExceedsULongMax_ReturnsULongMax()
    {
        double src = (double)2 * ulong.MaxValue;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MaxValue));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueBelowULongMin_ReturnsULongMin()
    {
        double src = ulong.MinValue - 1.0;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MinValue));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueAtULongMax_ReturnsUlong()
    {
        double src = Convert.ToDouble(ulong.MaxValue);

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MaxValue));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueAtULongMin_ReturnsUlong()
    {
        double src = ulong.MinValue;

        ulong result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(ulong.MinValue));
    }
}
