using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;


public static class MessageDeliveryStorageExtensions
{
    public static async Task<MessageDeliveryId> Add(this IMessageDeliveryStorage storage)
    {
        MessageDelivery delivery = new();
        await storage.Add(delivery);

        return delivery.Id;
    }

    public static async Task SetDispatched(
        this IMessageDeliveryStorage storage,
        MessageDeliveryId id,
        DateTime? dispatchedTimestamp = null)
    {
        if (await storage.Get(id) is not MessageDelivery delivery)
        {
            throw new InvalidOperationException($"Delivery with id {id} not found");
        }

        await storage.Update(delivery.Dispatched(dispatchedTimestamp ?? DateTime.UtcNow));
    }

    public static async Task SetReceived(
        this IMessageDeliveryStorage storage,
        MessageDeliveryId id,
        DateTime? receivedTimestamp = null)
    {
        if (await storage.Get(id) is not MessageDelivery delivery)
        {
            throw new InvalidOperationException($"Delivery with id {id} not found");
        }

        await storage.Update(delivery.Received(receivedTimestamp ?? DateTime.UtcNow));
    }

    public static async Task SetFailed(
        this IMessageDeliveryStorage storage,
        MessageDeliveryId id,
        MessageDeliveryFailure failure)
    {
        if (await storage.Get(id) is not MessageDelivery delivery)
        {
            throw new InvalidOperationException($"Delivery with id {id} not found");
        }

        await storage.Update(delivery.Failed(failure));
    }
}