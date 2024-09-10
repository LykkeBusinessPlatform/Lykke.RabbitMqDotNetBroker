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
    public MessageDeliveryId Id { get; }
    public DateTime? DispatchedTimestamp { get; init; }
    public DateTime? ReceivedTimestamp { get; init; }
    public MessageDeliveryFailure? Failure { get; init; }

    private MessageDelivery(
        MessageDeliveryId id,
        DateTime? dispatchedTimestamp,
        DateTime? receivedTimestamp,
        MessageDeliveryFailure? failure)
    {
        Id = id;
        DispatchedTimestamp = dispatchedTimestamp;
        ReceivedTimestamp = receivedTimestamp;
        Failure = failure;
    }

    public MessageDelivery() : this(new MessageDeliveryId(), null, null, null) { }

    public MessageDelivery(MessageDelivery other) : this(other.Id, other.DispatchedTimestamp, other.ReceivedTimestamp, other.Failure) { }

    public MessageDelivery Dispatched(DateTime dispatchedTimestamp) => this.GetStatus() switch
    {
        MessageDeliveryStatus.Pending => new(this) { DispatchedTimestamp = dispatchedTimestamp },
        _ => throw new InvalidOperationException($"Delivery {Id} status is invalid for setting dispatched")
    };

    public MessageDelivery Received(DateTime receivedTimestamp) => this.GetStatus() switch
    {
        MessageDeliveryStatus.Dispatched => new(this) { ReceivedTimestamp = receivedTimestamp },
        _ => throw new InvalidOperationException($"Delivery {Id} status is invalid for setting received")
    };

    public MessageDelivery Failed(MessageDeliveryFailure failure) => this.GetStatus() switch
    {
        MessageDeliveryStatus.Failed => throw new InvalidOperationException($"Delivery {Id} is already failed. Cannot set failed again."),
        _ => new(this) { Failure = failure }
    };
}