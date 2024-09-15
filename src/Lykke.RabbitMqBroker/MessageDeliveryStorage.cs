using System.Collections.Concurrent;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

public sealed class MessageDeliveryInMemoryStorage : IMessageDeliveryStorage
{
    private readonly ConcurrentDictionary<MessageDeliveryId, MessageDelivery> _storage = new();

    public Task<bool> AddOrUpdate(MessageDelivery messageDelivery) => messageDelivery switch
    {
        { IsNone: true } => Task.FromResult(false),
        _ => Task.FromResult(_storage.AddOrUpdate(messageDelivery.Id, messageDelivery, (_, _) => messageDelivery) is not null)
    };

    public Task<MessageDelivery> Get(MessageDeliveryId id) =>
        _storage.TryGetValue(id, out var messageDelivery)
            ? Task.FromResult(messageDelivery)
            : Task.FromResult(MessageDelivery.None);
}