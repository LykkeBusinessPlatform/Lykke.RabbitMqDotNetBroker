namespace Lykke.RabbitMqBroker.Subscriber;

public interface IRabbitMqListener
{
    public bool IsOpen();

    public string MessageTypeName { get; }
}