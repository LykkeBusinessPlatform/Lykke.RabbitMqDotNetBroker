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
        TimeProvider timeProvider) => (message.Expired(fairDelay, timeProvider), message.Delivered()) switch
        {
            (true, true) => MessageDeliveryAnalysisVerdict.LatelyDelivered,
            (false, true) => MessageDeliveryAnalysisVerdict.DeliveredOnTime,
            (true, false) => MessageDeliveryAnalysisVerdict.NotDelivered,
            (false, false) => MessageDeliveryAnalysisVerdict.NotDeliveredYet
        };
}
