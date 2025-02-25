using System;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Analysis;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using static Lykke.RabbitMqBroker.Abstractions.Analysis.MessageDeliveryAnalysis;

namespace Lykke.RabbitMqBroker;

internal sealed class MessageDeliveryAnalysisWorker(
    IMessageDeliveryStorage storage,
    IMonitoringMessageNotifier notifier,
    TimeProvider timeProvider,
    TimeSpan? fairDelayPeriod = null)
    : IMessageDeliveryAnalysisWorker
{
    private readonly TimeSpan _fairDelayPeriod = fairDelayPeriod ?? DefaultFairDelayPeriod;

    private readonly static TimeSpan DefaultFairDelayPeriod = TimeSpan.FromSeconds(10);

    public Task Execute() => storage
        .GetLatestForEveryRoute()
        .ForEachAwaitAsync(m => m.Analyze(_fairDelayPeriod, timeProvider.GetUtcNow().DateTime) switch
            {
                MessageDeliveryAnalysisVerdict.NotDelivered => notifier.NotifyNotDelivered(m),
                MessageDeliveryAnalysisVerdict.LatelyDelivered => notifier.NotifyLateDelivery(m),
                _ => Task.CompletedTask
            }
        );
}