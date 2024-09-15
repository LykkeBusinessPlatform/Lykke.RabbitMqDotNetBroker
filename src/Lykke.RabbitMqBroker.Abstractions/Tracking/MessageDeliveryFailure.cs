namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record MessageDeliveryFailure(MessageDeliveryFailureReason Reason, string Description, DateTime Timestamp)
{
    public static readonly MessageDeliveryFailure Empty = new(MessageDeliveryFailureReason.Uncategorised, string.Empty, DateTime.MinValue);
    public bool IsEmpty => this == Empty;
    public static MessageDeliveryFailure Create(MessageDeliveryFailureReason reason, string description = "") =>
        new(reason, description, DateTime.UtcNow);
    public static MessageDeliveryFailure FromException(Exception exception, MessageDeliveryFailureReason reason = MessageDeliveryFailureReason.Uncategorised) =>
        new(reason, exception.Message, DateTime.UtcNow);
}