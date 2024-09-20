namespace Lykke.RabbitMqBroker.Subscriber;

internal static class QueueNameExtensions
{
    public static PoisonQueueName AsPoison(this QueueName queueName) => PoisonQueueName.FromQueueName(queueName);
}
