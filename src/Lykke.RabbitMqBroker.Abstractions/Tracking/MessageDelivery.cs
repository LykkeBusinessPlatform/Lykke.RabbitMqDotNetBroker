namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record MessageDelivery(MessageDeliveryId Id, DateTime? DispatchedTimestamp, DateTime? ReceivedTimestamp, MessageDeliveryFailure Failure)
{
    public static MessageDelivery Create() => new(MessageDeliveryId.Create(), null, null, MessageDeliveryFailure.Empty);
    public static readonly MessageDelivery None = new(MessageDeliveryId.Empty, null, null, MessageDeliveryFailure.Empty);
    public bool IsNone => this == None;
}