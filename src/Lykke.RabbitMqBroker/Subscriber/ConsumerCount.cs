using System;

namespace Lykke.RabbitMqBroker.Subscriber;

public record ConsumerCount
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

    public static ConsumerCount Default { get; } = new ConsumerCount { Value = 1 };

    public static implicit operator ConsumerCount(byte value) => new ConsumerCount { Value = value };

    public static implicit operator ConsumerCount(int value) => new ConsumerCount { Value = value };

    public static implicit operator ConsumerCount(string value) =>
        int.TryParse(value, out var intValue)
            ? new ConsumerCount { Value = intValue }
            : throw new ArgumentException("Invalid string value for ConsumerCount");

    public static implicit operator ConsumerCount(decimal value) => new ConsumerCount { Value = (int)value };

    public static implicit operator int(ConsumerCount value) => value.Value;

    public static implicit operator byte(ConsumerCount value) => (byte)value.Value;

    public override string ToString() => Value.ToString();
}