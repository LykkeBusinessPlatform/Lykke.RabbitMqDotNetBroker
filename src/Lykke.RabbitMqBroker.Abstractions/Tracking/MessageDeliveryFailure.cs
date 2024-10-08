namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public sealed record MessageDeliveryFailure(MessageDeliveryFailureReason Reason, string Description, DateTime Timestamp)
{
    public static readonly MessageDeliveryFailure Empty = new(MessageDeliveryFailureReason.Uncategorised, string.Empty, DateTime.MinValue);
    public bool IsEmpty => this == Empty;
    public static MessageDeliveryFailure Create(MessageDeliveryFailureReason reason, string description = "", DateTime? dateTime = null) =>
        new(reason, description, dateTime ?? TimeProvider.System.GetLocalNow().DateTime);
    public static MessageDeliveryFailure FromException(Exception exception, MessageDeliveryFailureReason reason = MessageDeliveryFailureReason.Uncategorised, DateTime? dateTime = null) =>
        new(reason, exception.Message, dateTime ?? TimeProvider.System.GetLocalNow().DateTime);
}