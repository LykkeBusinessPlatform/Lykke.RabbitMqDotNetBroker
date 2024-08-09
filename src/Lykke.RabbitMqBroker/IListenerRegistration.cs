namespace Lykke.RabbitMqBroker
{
    public interface IListenerRegistration
    {
        string ExchangeName { get; }
        string QueueName { get; }
        string RoutingKey { get; }
        string MessageRoute { get; }
    }
}
