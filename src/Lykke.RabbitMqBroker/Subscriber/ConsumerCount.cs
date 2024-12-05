using System;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.RabbitMqBroker.Subscriber;
public record ConsumerCount : IEnumerable<int>, IComparable<ConsumerCount>
{
    private readonly byte _value = 1;

    public int Value
    {
        get => _value;
        init
        {
            if (value is < 1 or > 255)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Consumer count must be in the range [1, 255]");
            }

            _value = (byte)value;
        }
    }

    public static ConsumerCount Default { get; } = new() { Value = 1 };

    public static implicit operator ConsumerCount(byte value) => new() { Value = value };

    public static implicit operator ConsumerCount(int value) => new() { Value = value };

    public static implicit operator ConsumerCount(string value) =>
        int.TryParse(value, out var intValue)
            ? new ConsumerCount { Value = intValue }
            : throw new ArgumentException("Invalid string value for ConsumerCount");

    public static implicit operator ConsumerCount(decimal value) => new() { Value = (int)value };

    public static implicit operator int(ConsumerCount value) => value.Value;

    public static implicit operator byte(ConsumerCount value) => (byte)value.Value;

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = 1; i <= Value; i++)
        {
            yield return i;
        }
    }

    public override string ToString() => Value.ToString();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int CompareTo(ConsumerCount other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        return Value.CompareTo(other.Value);
    }

    public virtual bool Equals(ConsumerCount other) => other is not null && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator <(ConsumerCount left, ConsumerCount right) =>
        left is not null && left.CompareTo(right) < 0;

    public static bool operator <=(ConsumerCount left, ConsumerCount right) =>
        left is not null && left.CompareTo(right) <= 0;

    public static bool operator >(ConsumerCount left, ConsumerCount right) =>
        left is not null && left.CompareTo(right) > 0;

    public static bool operator >=(ConsumerCount left, ConsumerCount right) =>
        left is not null && left.CompareTo(right) >= 0;
}