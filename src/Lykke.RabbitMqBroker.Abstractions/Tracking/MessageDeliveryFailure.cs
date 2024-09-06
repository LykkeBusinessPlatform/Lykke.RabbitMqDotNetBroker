namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record MessageDeliveryFailure(
    MessageDeliveryFailureReason Reason,
    string? Description,
    DateTime Timestamp)
{
    public static MessageDeliveryFailure Create(
        MessageDeliveryFailureReason reason,
        string? description = null) => new(reason, description, DateTime.UtcNow);

    public static MessageDeliveryFailure FromException(
        Exception exception,
        MessageDeliveryFailureReason? reason = null) =>
        new(reason ?? MessageDeliveryFailureReason.Uncategorised, exception.Message, DateTime.UtcNow);
}