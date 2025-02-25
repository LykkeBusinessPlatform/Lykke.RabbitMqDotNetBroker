namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public interface IMessageDeliveryMaintenance
{
    IAsyncEnumerable<MessageDelivery> GetOlderThan(DateTime moment);
    Task Delete(MessageDeliveryId id) => Delete([id]);
    Task Delete(IEnumerable<MessageDeliveryId> ids);
}
