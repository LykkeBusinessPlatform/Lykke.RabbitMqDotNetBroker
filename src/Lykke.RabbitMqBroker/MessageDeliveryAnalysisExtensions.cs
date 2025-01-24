using System;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

internal static class MessageDeliveryAnalysisExtensions
{
    internal enum MessageDeliveryAnalysisVerdict
    {
        NotDeliveredYet,
        NotDelivered,
        LatelyDelivered,
        DeliveredOnTime
    }

    internal static MessageDeliveryAnalysisVerdict Analyze(
        this MessageDelivery message,
        TimeSpan fairDelay,
        TimeProvider timeProvider) => (message.FairDelayExpired(fairDelay, timeProvider), message.Delivered()) switch
        {
            (true, true) => MessageDeliveryAnalysisVerdict.LatelyDelivered,
            (false, true) => MessageDeliveryAnalysisVerdict.DeliveredOnTime,
            (true, false) => MessageDeliveryAnalysisVerdict.NotDelivered,
            (false, false) => MessageDeliveryAnalysisVerdict.NotDeliveredYet
        };
}
