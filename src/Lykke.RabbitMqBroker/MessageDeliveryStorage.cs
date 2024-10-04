using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

public sealed class MessageDeliveryInMemoryStorage : IMessageDeliveryStorage, IMessageDeliveryMaintenance
{
    private readonly ConcurrentDictionary<MessageDeliveryId, MessageDelivery> _storage = new();

    public Task<bool> AddOrUpdate(MessageDelivery messageDelivery) => messageDelivery switch
    {
        { IsNone: true } => Task.FromResult(false),
        _ => Task.FromResult(_storage.AddOrUpdate(messageDelivery.Id, messageDelivery, (_, _) => messageDelivery) is not null)
    };

    public Task Delete(IEnumerable<MessageDeliveryId> ids)
    {
        foreach (var id in ids)
        {
            _storage.TryRemove(id, out _);
        }

        return Task.CompletedTask;
    }

    public Task<MessageDelivery> Get(MessageDeliveryId id) =>
        _storage.TryGetValue(id, out var messageDelivery)
            ? Task.FromResult(messageDelivery)
            : Task.FromResult(MessageDelivery.None);

    public IAsyncEnumerable<MessageDelivery> GetChangedSince(DateTime timestamp)
    {
        return _storage
            .Values
            .Where(d => GetAllTimestamps(d).Max() >= timestamp)
            .ToAsyncEnumerable();

        static DateTime?[] GetAllTimestamps(MessageDelivery messageDelivery) =>
            [
                messageDelivery.DispatchedTimestamp,
                messageDelivery.ReceivedTimestamp,
                messageDelivery.Failure == MessageDeliveryFailure.Empty ? null : messageDelivery.Failure.Timestamp
            ];
    }
}