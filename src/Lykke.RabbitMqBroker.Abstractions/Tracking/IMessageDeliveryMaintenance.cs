namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

public interface IMessageDeliveryMaintenance
{
    IAsyncEnumerable<MessageDelivery> GetBeforeMoment(DateTime moment);
    Task Delete(MessageDeliveryId id) => Delete([id]);
    Task Delete(IEnumerable<MessageDeliveryId> ids);
}
