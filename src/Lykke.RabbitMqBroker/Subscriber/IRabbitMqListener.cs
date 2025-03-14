namespace Lykke.RabbitMqBroker.Subscriber;

public interface IRabbitMqListener
{
    public bool IsConnected();

    public string MessageTypeName { get; }
}