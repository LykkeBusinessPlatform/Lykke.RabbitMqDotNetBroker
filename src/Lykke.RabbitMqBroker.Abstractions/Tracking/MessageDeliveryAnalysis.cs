namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public static class MessageDeliveryAnalysis
{
    public static DateTime GetLastActionTimestamp(this MessageDelivery messageDelivery) =>
        messageDelivery switch
        {
            // once failed, message processing is considered finished
            // so that the failure timestamp is the last modified timestamp
            { Failure.IsEmpty: false } => messageDelivery.Failure.Timestamp,
            _ => messageDelivery.ReceivedTimestamp ?? messageDelivery.DispatchedTimestamp ?? DateTime.MinValue
        };

    public static bool Delivered(this MessageDelivery messageDelivery) =>
        messageDelivery switch
        {
            { ReceivedTimestamp: not null } => true,
            _ => false
        };

    public static bool NotDelivered(this MessageDelivery messageDelivery) =>
        !Delivered(messageDelivery);
}