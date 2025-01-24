using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

internal sealed class MessageDeliveryAnalysisWorker : IMessageDeliveryAnalysisWorker
{
    private readonly IMessageDeliveryStorage _storage;
    private readonly IMonitoringIssueNotifier _notifier;
    private readonly TimeSpan _fairDelayPeriod;
    private readonly TimeProvider _timeProvider;

    private readonly static TimeSpan DefaultFairDelayPeriod = TimeSpan.FromSeconds(10);

    public MessageDeliveryAnalysisWorker(
        IMessageDeliveryStorage storage,
        IMonitoringIssueNotifier notifier,
        TimeProvider timeProvider,
        TimeSpan? fairDelayPeriod = null)
    {
        _storage = storage;
        _notifier = notifier;
        _timeProvider = timeProvider;
        _fairDelayPeriod = fairDelayPeriod ?? DefaultFairDelayPeriod;
    }

    public async Task Execute()
    {
        await foreach (var message in _storage.GetLatestForEveryRoute())
        {
            if (!message.FairDelayExpired(_fairDelayPeriod, _timeProvider))
            {
                continue;
            }

            if (message.Delivered())
            {
                await _notifier.NotifyLateDelivery(message);
                continue;
            }

            await _notifier.NotifyNotDelivered(message);
        }
    }
}
