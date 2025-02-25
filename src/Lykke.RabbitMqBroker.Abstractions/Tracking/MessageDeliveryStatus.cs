namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Represents the status of a message delivery.
/// </summary>
public enum MessageDeliveryStatus
{
    /// <summary>
    /// Message was initialized but not dispatched yet.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Message was dispatched and confirmed by the broker.
    /// </summary>
    Dispatched = 10,

    /// <summary>
    /// Message was received by the consumer.
    /// </summary>
    Received = 20,

    /// <summary>
    /// Failure occurred during the message lifecycle.
    /// </summary>
    Failed = 30
}
