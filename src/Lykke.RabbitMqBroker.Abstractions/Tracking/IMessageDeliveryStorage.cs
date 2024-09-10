namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Storage interface for message deliveries.
/// </summary>
public interface IMessageDeliveryStorage
{
    Task Add(MessageDelivery messageDelivery);
    Task Update(MessageDelivery messageDelivery);
    Task<MessageDelivery?> Get(MessageDeliveryId id);
}