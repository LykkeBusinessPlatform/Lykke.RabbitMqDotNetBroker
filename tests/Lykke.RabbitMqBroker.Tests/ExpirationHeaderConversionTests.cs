using System;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class ExpirationHeaderConversionTests
{
    [Test]
    public void ToExpirationMilliseconds_ValidValueWithoutFraction_ReturnsLong()
    {
        double src = 12345;

        long result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(12345L));
    }

    [Test]
    public void ToExpirationMilliseconds_ValidValueWithFraction_ReturnsFlooredLong()
    {
        double src = 12345.67;

        long result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(12345L));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueExceedsLongMax_ReturnsLongMax()
    {
        double src = (double)2 * long.MaxValue;

        long result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(long.MaxValue));
    }

    [TestCase(0d)]
    [TestCase(-1d)]
    public void ToExpirationMilliseconds_ValueBelowZero_ReturnsZero(double value)
    {
        double src = value;

        long result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(0L));
    }

    [Test]
    public void ToExpirationMilliseconds_ValueAtLongMax_ReturnsLongMax()
    {
        double src = Convert.ToDouble(long.MaxValue);

        long result = src.ToExpirationMilliseconds();

        Assert.That(result, Is.EqualTo(long.MaxValue));
    }
}
