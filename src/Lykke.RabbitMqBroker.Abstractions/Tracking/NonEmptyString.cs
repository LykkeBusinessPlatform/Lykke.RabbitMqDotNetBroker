namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record NonEmptyString
{
    public string Value { get; }

    public NonEmptyString(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value.Trim();
    }

    public override string ToString() => Value;

    public static NonEmptyString Create(string value) => new(value);

    public static implicit operator string(NonEmptyString nonEmptyString) => nonEmptyString.Value;
}
