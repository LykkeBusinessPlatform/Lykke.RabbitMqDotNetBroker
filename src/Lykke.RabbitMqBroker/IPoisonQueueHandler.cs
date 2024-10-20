using System.Threading;

namespace Lykke.RabbitMqBroker;

public interface IPoisonQueueHandler
{
    string TryPutMessagesBack(CancellationToken cancellationToken = default);
}
