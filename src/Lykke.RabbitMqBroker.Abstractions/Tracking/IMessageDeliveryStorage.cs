namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Storage interface for message deliveries.
/// </summary>
public interface IMessageDeliveryStorage
{
    Task<bool> AddOrUpdate(MessageDelivery messageDelivery);
    Task<MessageDelivery> Get(MessageDeliveryId id);
    IAsyncEnumerable<MessageDelivery> GetLatestForEveryRoute();
}
