using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatReceiverTests;

internal sealed class FakeStorage : IMessageDeliveryStorage
{
    private Dictionary<MessageDeliveryId, MessageDeliveryStatus> _received { get; set; } = [];

    /// <summary>
    /// The list of received message delivery IDs.
    /// </summary>
    public IReadOnlyList<MessageDeliveryId> Received => _received
        .Where(x => x.Value == MessageDeliveryStatus.Received)
        .Select(x => x.Key)
        .ToList();

    public Task<bool> AddOrUpdate(MessageDelivery messageDelivery)
    {
        _received.Add(messageDelivery.Id, messageDelivery.GetStatus());
        return Task.FromResult(true);
    }

    /// <summary>
    /// Emulates the method that retrieves the message delivery by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<MessageDelivery> Get(MessageDeliveryId id) =>
        Task.FromResult(MessageDelivery.Create(id, new MessageRouteWithDefaults()).TrySetDispatched(DateTime.UtcNow));

    public IAsyncEnumerable<MessageDelivery> GetLatestForEveryRoute()
    {
        throw new NotImplementedException();
    }
}