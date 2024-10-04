namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public interface IMessageDeliveryMaintenance
{
    IAsyncEnumerable<MessageDelivery> GetChangedSince(DateTime timestamp);
    Task Delete(MessageDeliveryId id) => Delete([id]);
    Task Delete(IEnumerable<MessageDeliveryId> ids);
}
