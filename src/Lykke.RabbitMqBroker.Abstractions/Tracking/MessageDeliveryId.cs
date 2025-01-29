namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public readonly record struct MessageDeliveryId(Guid Value)
{
    public static readonly MessageDeliveryId Empty = new(Guid.Empty);
    public static MessageDeliveryId Create() => new(Guid.NewGuid());
    public static MessageDeliveryId Parse(string value) => Guid.TryParse(value, out var guid) ? new(guid) : Empty;
    public bool IsEmpty => this == Empty;
    public override string ToString() => Value.ToString();
}
