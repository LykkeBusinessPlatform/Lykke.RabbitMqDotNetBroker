using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Analysis;

public static class MessageDeliveryAnalysis
{
    public enum MessageDeliveryAnalysisVerdict
    {
        NotDeliveredYet,
        NotDelivered,
        LatelyDelivered,
        DeliveredOnTime
    }

    public static MessageDeliveryAnalysisVerdict Analyze(
        this MessageDelivery message,
        TimeSpan fairDelay,
        DateTime currentTime) => message switch
        {
            _ when message.NotDelivered() => message.YetToBeDelivered(fairDelay, currentTime)
                ? MessageDeliveryAnalysisVerdict.NotDeliveredYet
                : MessageDeliveryAnalysisVerdict.NotDelivered,
            _ when message.DeliveredOnTime(fairDelay) => MessageDeliveryAnalysisVerdict.DeliveredOnTime,
            _ => MessageDeliveryAnalysisVerdict.LatelyDelivered
        };
}
