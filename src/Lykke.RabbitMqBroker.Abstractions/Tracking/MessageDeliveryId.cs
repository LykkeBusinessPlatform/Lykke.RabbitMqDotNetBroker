namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public readonly struct MessageDeliveryId
{
    public readonly Guid Value { get; init; }
    public MessageDeliveryId() => Value = Guid.NewGuid();
    private MessageDeliveryId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value cannot be empty", nameof(value));

        Value = value;
    }
    public override string ToString() => Value.ToString();
    public static MessageDeliveryId FromGuid(Guid value) => new(value);
    public static MessageDeliveryId FromGuid(string value) => new(Guid.Parse(value));
}
