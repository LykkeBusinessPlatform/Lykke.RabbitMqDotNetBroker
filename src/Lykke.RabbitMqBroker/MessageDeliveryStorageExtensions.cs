using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;


public static class MessageDeliveryStorageExtensions
{
    public static async Task<MessageDeliveryId> Add(this IMessageDeliveryStorage storage, MessageRoute route)
    {
        var delivery = MessageDelivery.Create(route);
        await storage.AddOrUpdate(delivery);

        return delivery.Id;
    }

    public static async Task<bool> TrySetDispatched(this IMessageDeliveryStorage storage, MessageDeliveryId id, DateTime? dispatchedTimestamp = null)
    {
        var originalDelivery = await storage.Get(id);
        var updatedDelivery = originalDelivery.TrySetDispatched(dispatchedTimestamp ?? DateTime.UtcNow);

        var updatedIndeed = updatedDelivery != originalDelivery;
        if (updatedIndeed)
            return await storage.AddOrUpdate(updatedDelivery);

        return false;
    }

    public static async Task<bool> TrySetReceived(this IMessageDeliveryStorage storage, MessageDeliveryId id, DateTime? receivedTimestamp = null)
    {
        var originalDelivery = await storage.Get(id);
        var updatedDelivery = originalDelivery.TrySetReceived(receivedTimestamp ?? DateTime.UtcNow);

        var updatedIndeed = updatedDelivery != originalDelivery;
        if (updatedIndeed)
            return await storage.AddOrUpdate(updatedDelivery);

        return false;
    }

    public static async Task<bool> TrySetFailed(this IMessageDeliveryStorage storage, MessageDeliveryId id, MessageDeliveryFailure failure)
    {
        var originalDelivery = await storage.Get(id);
        var updatedDelivery = originalDelivery.TrySetFailed(failure);

        var updatedIndeed = updatedDelivery != originalDelivery;
        if (updatedIndeed)
            return await storage.AddOrUpdate(updatedDelivery);

        return false;
    }
}