namespace Lykke.RabbitMqBroker
{
    // todo: do we need this interface?
    public interface IListenerRegistration
    {
        ListenerRoute ListenerRoute { get; }
    }
}
