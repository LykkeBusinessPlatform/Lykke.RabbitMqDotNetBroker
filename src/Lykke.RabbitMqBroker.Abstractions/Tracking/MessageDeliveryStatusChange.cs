namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public static class MessageDeliveryStatusChange
{
    /// <summary>
    /// Tries to set the dispatched timestamp of the message delivery.
    /// If status is not pending, the method does nothing but just 
    /// returns the delivery.
    /// </summary>
    /// <param name="delivery"></param>
    /// <param name="dispatchedTimestamp"></param>
    /// <returns></returns>
    public static MessageDelivery TrySetDispatched(this MessageDelivery delivery, DateTime dispatchedTimestamp) => delivery switch
    {
        { IsNone: true } => delivery,
        _ => delivery.GetStatus() switch
        {
            MessageDeliveryStatus.Pending => delivery with { DispatchedTimestamp = dispatchedTimestamp },
            _ => delivery
        }
    };


    /// <summary>
    /// Tries to set the received timestamp of the message delivery.
    /// If status is not dispatched, the method does nothing but just
    /// returns the delivery.
    /// </summary>
    /// <param name="delivery"></param>
    /// <param name="receivedTimestamp"></param>
    /// <returns></returns>
    public static MessageDelivery TrySetReceived(this MessageDelivery delivery, DateTime receivedTimestamp) => delivery switch
    {
        { IsNone: true } => delivery,
        _ => delivery.GetStatus() switch
        {
            MessageDeliveryStatus.Dispatched => delivery with { ReceivedTimestamp = receivedTimestamp },
            _ => delivery
        }
    };

    /// <summary>
    /// Tries to set the failure of the message delivery.
    /// If status is already failed, the method does nothing but just
    /// returns the delivery.
    /// </summary>
    /// <param name="delivery"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public static MessageDelivery TrySetFailed(this MessageDelivery delivery, MessageDeliveryFailure failure) => delivery switch
    {
        { IsNone: true } => delivery,
        _ => delivery.GetStatus() switch
        {
            MessageDeliveryStatus.Failed => delivery,
            _ => failure switch
            {
                { IsEmpty: true } => delivery,
                _ => delivery with { Failure = failure }
            }
        }
    };

    /// <summary>
    /// Gets the status of the message delivery
    /// based on the presence of timestamps and failure.
    /// </summary>
    /// <param name="delivery"></param>
    /// <returns></returns>
    public static MessageDeliveryStatus GetStatus(this MessageDelivery delivery) => delivery switch
    {
        { Failure.IsEmpty: false } => MessageDeliveryStatus.Failed,
        { ReceivedTimestamp: not null } => MessageDeliveryStatus.Received,
        { DispatchedTimestamp: not null } => MessageDeliveryStatus.Dispatched,
        _ => MessageDeliveryStatus.Pending
    };
}