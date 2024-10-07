namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public record MessageDelivery(MessageDeliveryId Id, DateTime? DispatchedTimestamp, DateTime? ReceivedTimestamp, MessageDeliveryFailure Failure, MessageRoute Route)
{
    internal static MessageDelivery Create(MessageDeliveryId id, MessageRoute route) => new(id, null, null, MessageDeliveryFailure.Empty, route);
    public static MessageDelivery Create(MessageRoute route) => Create(MessageDeliveryId.Create(), route);
    public static readonly MessageDelivery None = new(MessageDeliveryId.Empty, null, null, MessageDeliveryFailure.Empty, MessageRoute.None);
    public bool IsNone => this == None;
}