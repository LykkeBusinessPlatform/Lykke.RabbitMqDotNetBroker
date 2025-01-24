namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Represents the reason of a message delivery failure.
/// </summary>
public enum MessageDeliveryFailureReason
{
    None = 0,

    /// <summary>
    /// Could not publish the message
    /// </summary>
    DispatchError = 5,

    /// <summary>
    /// Message was published but not confirmed by the broker.
    /// </summary>
    BrokerCustodyNotConfirmed = 10,

    /// <summary>
    /// Message was published but not routed to any queue so it was returned
    /// if configured to do so.
    /// </summary>
    Unroutable = 15
}
