namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record MessageDelivery(MessageDeliveryId Id, DateTime? DispatchedTimestamp, DateTime? ReceivedTimestamp, MessageDeliveryFailure Failure)
{
    internal static MessageDelivery Create(MessageDeliveryId id) => new(id, null, null, MessageDeliveryFailure.Empty);
    public static MessageDelivery Create() => Create(MessageDeliveryId.Create());
    public static readonly MessageDelivery None = new(MessageDeliveryId.Empty, null, null, MessageDeliveryFailure.Empty);
    public bool IsNone => this == None;
}