namespace Lykke.RabbitMqBroker.Abstractions;

public interface IMonitoringMessageDeliveryStorage
{
    Task<bool> Add(IMonitoringMessageDelivery messageDelivery);
    Task<IMonitoringMessageDelivery?> Get(Guid id);
    Task<IEnumerable<IMonitoringMessageDelivery>> GetByMessageId(Guid messageId);
}