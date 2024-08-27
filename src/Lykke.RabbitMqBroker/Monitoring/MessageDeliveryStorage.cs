using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class MessageDeliveryInMemoryStorage : IMonitoringMessageDeliveryStorage
{
    private readonly ConcurrentDictionary<Guid, IMonitoringMessageDelivery> _storage = new();
    private readonly ConcurrentDictionary<Guid, List<IMonitoringMessageDelivery>> _messageIdIndex = new();
    public Task<bool> Add(IMonitoringMessageDelivery messageDelivery)
    {
        var added = _storage.TryAdd(messageDelivery.Id, messageDelivery);

        if (added)
        {
            _messageIdIndex.AddOrUpdate(
                messageDelivery.MessageId,
                [messageDelivery],
                (_, list) =>
                {
                    list.Add(messageDelivery);
                    return list;
                });
        }

        return Task.FromResult(added);
    }

    public Task<IMonitoringMessageDelivery> Get(Guid id) =>
        _storage.TryGetValue(id, out var messageDelivery)
            ? Task.FromResult(messageDelivery)
            : Task.FromResult<IMonitoringMessageDelivery>(null);

    public Task<IEnumerable<IMonitoringMessageDelivery>> GetByMessageId(Guid messageId) =>
        _messageIdIndex.TryGetValue(messageId, out var messageDeliveries)
            ? Task.FromResult<IEnumerable<IMonitoringMessageDelivery>>(messageDeliveries)
            : Task.FromResult<IEnumerable<IMonitoringMessageDelivery>>([]);
}