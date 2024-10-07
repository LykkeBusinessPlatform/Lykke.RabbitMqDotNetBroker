namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record NonEmptyString
{
    public string Value { get; }

    public NonEmptyString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or empty.", nameof(value));
        }
        Value = value.Trim();
    }

    public static NonEmptyString Create(string value) => new(value);

    public static implicit operator string(NonEmptyString nonEmptyString) => nonEmptyString.Value;
}
