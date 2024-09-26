using System;
using System.Linq;

using Lykke.RabbitMqBroker.Subscriber;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
public class ConsumerCountTests
{
    [TestCase(1)]
    [TestCase(255)]
    [TestCase(100)]
    public void ConsumerCount_ValidValues_ShouldNotThrowException(int value)
    {
        Assert.DoesNotThrow(() => _ = new ConsumerCount { Value = value });
    }

    [TestCase(0)]
    [TestCase(256)]
    [TestCase(-1)]
    public void ConsumerCount_InvalidValues_ShouldThrowArgumentOutOfRangeException(int value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = new ConsumerCount { Value = value };
        });
    }

    [TestCase(1)]
    [TestCase(255)]
    public void ImplicitConversion_FromByte_ShouldSetProperValue(byte value)
    {
        ConsumerCount consumerCount = value;
        Assert.That(value, Is.EqualTo(consumerCount.Value));
    }

    [TestCase(1)]
    [TestCase(255)]
    public void ImplicitConversion_FromInt_ShouldSetProperValue(int value)
    {
        ConsumerCount consumerCount = value;
        Assert.That(value, Is.EqualTo(consumerCount.Value));
    }

    [TestCase("1")]
    [TestCase("255")]
    public void ImplicitConversion_FromString_ShouldSetProperValue(string value)
    {
        ConsumerCount consumerCount = value;
        Assert.That(int.Parse(value), Is.EqualTo(consumerCount.Value));
    }

    [TestCase("invalid")]
    public void ImplicitConversion_FromInvalidString_ShouldThrowArgumentException(string value)
    {
        Assert.Throws<ArgumentException>(() =>
        {
            _ = (ConsumerCount)value;
        });
    }

    [TestCase(1)]
    [TestCase(255)]
    public void ImplicitConversion_FromDecimal_ShouldSetProperValue(decimal value)
    {
        ConsumerCount consumerCount = value;
        Assert.That((int)value, Is.EqualTo(consumerCount.Value));
    }

    [Test]
    public void ImplicitConversion_ToInt_ShouldReturnProperValue()
    {
        ConsumerCount consumerCount = 100;
        int intValue = consumerCount;
        Assert.That(intValue, Is.EqualTo(100));
    }

    [Test]
    public void ImplicitConversion_ToByte_ShouldReturnProperValue()
    {
        ConsumerCount consumerCount = 100;
        byte byteValue = consumerCount;
        Assert.That(byteValue, Is.EqualTo(100));
    }

    [Test]
    public void DefaultProperty_ShouldReturnConsumerCountOfOne()
    {
        var defaultConsumerCount = ConsumerCount.Default;
        Assert.That(defaultConsumerCount.Value, Is.EqualTo(1));
    }

    [TestCase(1)]
    [TestCase(255)]
    [TestCase(100)]
    public void ConsumerCount_ToString_ShouldReturnCorrectString(int value)
    {
        ConsumerCount consumerCount = value;
        string result = consumerCount.ToString();
        Assert.That(value.ToString(), Is.EqualTo(result));
    }

    [Test]
    public void GetEnumerator_ShouldReturnCorrectSequence()
    {
        var consumerCount = new ConsumerCount { Value = 5 };
        var expectedValues = new[] { 1, 2, 3, 4, 5 };
        var actualValues = consumerCount.ToList();

        Assert.That(expectedValues, Is.EqualTo(actualValues));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void CompareTo_ShouldReturnNegative_WhenFirstIsLessThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1.CompareTo(count2), Is.LessThan(0));
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    public void CompareTo_ShouldReturnPositive_WhenFirstIsGreaterThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1.CompareTo(count2), Is.GreaterThan(0));
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(255)]
    public void CompareTo_ShouldReturnZero_WhenValuesAreEqual(int value)
    {
        var count1 = new ConsumerCount { Value = value };
        var count2 = new ConsumerCount { Value = value };
        Assert.That(count1.CompareTo(count2), Is.EqualTo(0));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void LessThanOperator_ShouldReturnTrue_WhenFirstIsLessThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.LessThan(count2));
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    public void GreaterThanOperator_ShouldReturnTrue_WhenFirstIsGreaterThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.GreaterThan(count2));
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(255)]
    public void Equals_ShouldReturnTrue_WhenValuesAreEqual(int value)
    {
        var count1 = new ConsumerCount { Value = value };
        var count2 = new ConsumerCount { Value = value };
        Assert.That(count1, Is.EqualTo(count2));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void Equals_ShouldReturnFalse_WhenValuesAreNotEqual(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.Not.EqualTo(count2));
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualValues()
    {
        var count1 = new ConsumerCount { Value = 100 };
        var count2 = new ConsumerCount { Value = 100 };
        Assert.That(count1.GetHashCode(), Is.EqualTo(count2.GetHashCode()));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    [TestCase(100, 100)]
    public void LessThanOrEqualOperator_ShouldReturnTrue_WhenFirstIsLessThanOrEqualToSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.LessThanOrEqualTo(count2));
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    public void LessThanOrEqualOperator_ShouldReturnFalse_WhenFirstIsGreaterThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.GreaterThan(count2));
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    [TestCase(100, 100)]
    public void GreaterThanOrEqualOperator_ShouldReturnTrue_WhenFirstIsGreaterThanOrEqualToSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.GreaterThanOrEqualTo(count2));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void GreaterThanOrEqualOperator_ShouldReturnFalse_WhenFirstIsLessThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.That(count1, Is.LessThan(count2));
    }
}