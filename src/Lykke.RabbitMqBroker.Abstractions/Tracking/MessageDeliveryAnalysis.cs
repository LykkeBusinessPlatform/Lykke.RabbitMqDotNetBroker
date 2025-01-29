namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public static class MessageDeliveryAnalysis
{
    public static DateTime? GetLastActionTimestamp(this MessageDelivery messageDelivery) =>
        messageDelivery switch
        {
            // once failed, message processing is considered finished
            // so that the failure timestamp is the last modified timestamp
            { Failure.IsEmpty: false } => messageDelivery.Failure.Timestamp,
            _ => messageDelivery.ReceivedTimestamp ?? messageDelivery.DispatchedTimestamp ?? null
        };

    public static bool Delivered(this MessageDelivery messageDelivery) =>
        messageDelivery switch
        {
            { Failure.IsEmpty: false } => false,
            { ReceivedTimestamp: not null } => true,
            _ => false
        };

    public static bool NotDelivered(this MessageDelivery messageDelivery) =>
        !Delivered(messageDelivery);

    public static bool DeliveredOnTime(this MessageDelivery messageDelivery, TimeSpan fairDelay) =>
        messageDelivery switch
        {
            _ when messageDelivery.NotDelivered() => false,
            _ => !PeriodPassed(
                messageDelivery.DispatchedTimestamp.Value,
                messageDelivery.ReceivedTimestamp.Value,
                fairDelay)
        };

    public static bool YetToBeDelivered(this MessageDelivery messageDelivery, TimeSpan fairDelay, DateTime currentTime) =>
        messageDelivery switch
        {
            { Failure.IsEmpty: false } => false,
            _ => !PeriodPassed(
                messageDelivery.DispatchedTimestamp.Value,
                currentTime,
                fairDelay)
        };

    private static bool PeriodPassed(DateTime start, DateTime end, TimeSpan period) => end - start > period;
}