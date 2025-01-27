using System;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// Retention policy implementation for message deliveries.
/// Deletes all message deliveries that are older than the
/// specified retention period or 24 hours by default.
/// </summary>
internal sealed class MessageDeliveryCleanupWorker : IMessageDeliveryCleanupWorker
{
    private readonly IMessageDeliveryMaintenance _messageStorage;
    private readonly TimeSpan _retentionPeriod;
    private readonly TimeProvider _timeProvider;

    private readonly static TimeSpan DefaultRetentionPeriod = TimeSpan.FromHours(24);

    public MessageDeliveryCleanupWorker(
        IMessageDeliveryMaintenance messageDeliveryMaintenance,
        TimeProvider timeProvider,
        TimeSpan? retentionPeriod = null)
    {
        _messageStorage = messageDeliveryMaintenance ?? throw new ArgumentNullException(nameof(messageDeliveryMaintenance));
        _retentionPeriod = retentionPeriod ?? DefaultRetentionPeriod;
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    public Task Execute() =>
        _messageStorage
            .GetBeforeMoment(RetentionMoment.From(_retentionPeriod, _timeProvider))
            .ForEachAwaitAsync(m => _messageStorage.Delete([m.Id]));
}
