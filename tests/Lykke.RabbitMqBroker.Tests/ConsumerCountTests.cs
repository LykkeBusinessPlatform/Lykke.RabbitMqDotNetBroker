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
        Assert.DoesNotThrow(() =>
        {
            _ = new ConsumerCount { Value = value };
        });
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
        Assert.AreEqual(value, consumerCount.Value);
    }

    [TestCase(1)]
    [TestCase(255)]
    public void ImplicitConversion_FromInt_ShouldSetProperValue(int value)
    {
        ConsumerCount consumerCount = value;
        Assert.AreEqual(value, consumerCount.Value);
    }

    [TestCase("1")]
    [TestCase("255")]
    public void ImplicitConversion_FromString_ShouldSetProperValue(string value)
    {
        ConsumerCount consumerCount = value;
        Assert.AreEqual(int.Parse(value), consumerCount.Value);
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
        Assert.AreEqual((int)value, consumerCount.Value);
    }

    [Test]
    public void ImplicitConversion_ToInt_ShouldReturnProperValue()
    {
        ConsumerCount consumerCount = 100;
        int intValue = consumerCount;
        Assert.AreEqual(100, intValue);
    }

    [Test]
    public void ImplicitConversion_ToByte_ShouldReturnProperValue()
    {
        ConsumerCount consumerCount = 100;
        byte byteValue = consumerCount;
        Assert.AreEqual(100, byteValue);
    }

    [Test]
    public void DefaultProperty_ShouldReturnConsumerCountOfOne()
    {
        var defaultConsumerCount = ConsumerCount.Default;
        Assert.AreEqual(1, defaultConsumerCount.Value);
    }
    
    [TestCase(1)]
    [TestCase(255)]
    [TestCase(100)]
    public void ConsumerCount_ToString_ShouldReturnCorrectString(int value)
    {
        ConsumerCount consumerCount = value;
        string result = consumerCount.ToString();
        Assert.AreEqual(value.ToString(), result);
    }
    
    [Test]
    public void GetEnumerator_ShouldReturnCorrectSequence()
    {
        var consumerCount = new ConsumerCount { Value = 5 };
        var expectedValues = new[] { 1, 2, 3, 4, 5 };
        var actualValues = consumerCount.ToList();

        CollectionAssert.AreEqual(expectedValues, actualValues);
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void CompareTo_ShouldReturnNegative_WhenFirstIsLessThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.Less(count1.CompareTo(count2), 0);
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    public void CompareTo_ShouldReturnPositive_WhenFirstIsGreaterThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.Greater(count1.CompareTo(count2), 0);
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(255)]
    public void CompareTo_ShouldReturnZero_WhenValuesAreEqual(int value)
    {
        var count1 = new ConsumerCount { Value = value };
        var count2 = new ConsumerCount { Value = value };
        Assert.AreEqual(0, count1.CompareTo(count2));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void LessThanOperator_ShouldReturnTrue_WhenFirstIsLessThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsTrue(count1 < count2);
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    public void GreaterThanOperator_ShouldReturnTrue_WhenFirstIsGreaterThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsTrue(count1 > count2);
    }

    [TestCase(1)]
    [TestCase(100)]
    [TestCase(255)]
    public void Equals_ShouldReturnTrue_WhenValuesAreEqual(int value)
    {
        var count1 = new ConsumerCount { Value = value };
        var count2 = new ConsumerCount { Value = value };
        Assert.IsTrue(count1.Equals(count2));
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void Equals_ShouldReturnFalse_WhenValuesAreNotEqual(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsFalse(count1.Equals(count2));
    }

    [Test]
    public void GetHashCode_ShouldReturnSameHashCode_ForEqualValues()
    {
        var count1 = new ConsumerCount { Value = 100 };
        var count2 = new ConsumerCount { Value = 100 };
        Assert.AreEqual(count1.GetHashCode(), count2.GetHashCode());
    }
    
    [TestCase(1, 2)]
    [TestCase(100, 255)]
    [TestCase(100, 100)]
    public void LessThanOrEqualOperator_ShouldReturnTrue_WhenFirstIsLessThanOrEqualToSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsTrue(count1 <= count2);
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    public void LessThanOrEqualOperator_ShouldReturnFalse_WhenFirstIsGreaterThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsFalse(count1 <= count2);
    }

    [TestCase(2, 1)]
    [TestCase(255, 100)]
    [TestCase(100, 100)]
    public void GreaterThanOrEqualOperator_ShouldReturnTrue_WhenFirstIsGreaterThanOrEqualToSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsTrue(count1 >= count2);
    }

    [TestCase(1, 2)]
    [TestCase(100, 255)]
    public void GreaterThanOrEqualOperator_ShouldReturnFalse_WhenFirstIsLessThanSecond(int value1, int value2)
    {
        var count1 = new ConsumerCount { Value = value1 };
        var count2 = new ConsumerCount { Value = value2 };
        Assert.IsFalse(count1 >= count2);
    }
}