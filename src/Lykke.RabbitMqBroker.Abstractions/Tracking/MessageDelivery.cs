namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Message delivery tracking entity.
/// </summary>
/// <param name="Id"></param>
/// <param name="DispatchedTimestamp"></param>
/// <param name="ReceivedTimestamp"></param>
/// <param name="Failure"></param>
public readonly struct MessageDelivery
{
    public readonly Guid Id;
    public readonly DateTime? DispatchedTimestamp;
    public readonly DateTime? ReceivedTimestamp;
    public readonly MessageDeliveryFailure? Failure;

    private MessageDelivery(
        Guid id,
        DateTime? dispatchedTimestamp,
        DateTime? receivedTimestamp,
        MessageDeliveryFailure? failure)
    {
        Id = id;
        DispatchedTimestamp = dispatchedTimestamp;
        ReceivedTimestamp = receivedTimestamp;
        Failure = failure;
    }

    public MessageDeliveryStatus Status() => this switch
    {
        { Failure: not null } => MessageDeliveryStatus.Failed,
        { ReceivedTimestamp: not null } => MessageDeliveryStatus.Received,
        { DispatchedTimestamp: not null } => MessageDeliveryStatus.Dispatched,
        _ => MessageDeliveryStatus.Pending
    };

    public static MessageDelivery Create() => new(Guid.NewGuid(), null, null, null);

    public MessageDelivery Dispatched(DateTime dispatchedTimestamp) => Status() switch
    {
        MessageDeliveryStatus.Pending => new(Id, dispatchedTimestamp, ReceivedTimestamp, Failure),
        _ => throw new InvalidOperationException($"Delivery {Id} status is invalid for setting dispatched")
    };

    public MessageDelivery Received(DateTime receivedTimestamp) => Status() switch
    {
        MessageDeliveryStatus.Dispatched => new(Id, DispatchedTimestamp, receivedTimestamp, Failure),
        _ => throw new InvalidOperationException($"Delivery {Id} status is invalid for setting received")
    };

    public MessageDelivery Failed(MessageDeliveryFailure failure) => Status() switch
    {
        MessageDeliveryStatus.Failed => throw new InvalidOperationException($"Delivery {Id} is already failed. Cannot set failed again."),
        _ => new(Id, DispatchedTimestamp, ReceivedTimestamp, failure)
    };
}