namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

internal static class MessageDeliveryStatusFeature
{
    public static MessageDeliveryStatus GetStatus(this MessageDelivery delivery) => delivery switch
    {
        { Failure: not null } => MessageDeliveryStatus.Failed,
        { ReceivedTimestamp: not null } => MessageDeliveryStatus.Received,
        { DispatchedTimestamp: not null } => MessageDeliveryStatus.Dispatched,
        _ => MessageDeliveryStatus.Pending
    };
}
