using System;

namespace Lykke.RabbitMqBroker;

readonly record struct NonEmptyString(string Value)
{
    public readonly string Value { get; init; } =
        !string.IsNullOrWhiteSpace(Value)
            ? Value.Trim()
            : throw new ArgumentException("Value cannot be null or whitespace", nameof(Value));

    public NonEmptyString() : this(string.Empty) { }

    public static implicit operator string(NonEmptyString value) => value.Value;
    public static implicit operator NonEmptyString(string value) => new(value);
}
