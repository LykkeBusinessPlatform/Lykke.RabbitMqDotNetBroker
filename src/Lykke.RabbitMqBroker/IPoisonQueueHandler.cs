namespace Lykke.RabbitMqBroker;

public interface IPoisonQueueHandler
{
    string TryPutMessagesBack();
}
