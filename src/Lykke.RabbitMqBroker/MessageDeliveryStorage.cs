using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

public sealed class MessageDeliveryInMemoryStorage : IMessageDeliveryStorage
{
    private readonly ConcurrentDictionary<MessageDeliveryId, MessageDelivery> _storage = new();
    public Task Add(MessageDelivery messageDelivery)
    {
        var added = _storage.TryAdd(messageDelivery.Id, messageDelivery);
        if (!added)
        {
            throw new InvalidOperationException($"Message delivery with id {messageDelivery.Id} could not be added to the storage");
        }

        return Task.CompletedTask;
    }

    public Task<MessageDelivery?> Get(MessageDeliveryId id) =>
        _storage.TryGetValue(id, out var messageDelivery)
            ? Task.FromResult<MessageDelivery?>(messageDelivery)
            : Task.FromResult<MessageDelivery?>(null);

    public Task Update(MessageDelivery messageDelivery)
    {
        if (!_storage.ContainsKey(messageDelivery.Id))
        {
            throw new InvalidOperationException($"Message delivery with id {messageDelivery.Id} not found in the storage");
        }

        _storage[messageDelivery.Id] = messageDelivery;

        return Task.CompletedTask;
    }
}