namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public static class MessageDeliveryRetentionPolicy
{
    /// <summary>
    /// Checks if the message delivery is older than the specified moment.
    /// The message is considered older if it was dispatched before the moment
    /// or if it failed to be dispatched before the moment.
    /// </summary>
    /// <param name="messageDelivery"></param>
    /// <param name="moment"></param>
    /// <returns></returns>
    public static bool IsOlderThan(this MessageDelivery messageDelivery, DateTime moment) =>
        messageDelivery switch
        {
            { DispatchedTimestamp: { } } => messageDelivery.DispatchedTimestamp < moment,
            { Failure.IsEmpty: false } => messageDelivery.Failure.Timestamp < moment,
            _ => false
        };

    public static bool IsOlderThan(this MessageDelivery messageDelivery, Func<DateTime> momentFunc) =>
        messageDelivery.IsOlderThan(momentFunc());
}
