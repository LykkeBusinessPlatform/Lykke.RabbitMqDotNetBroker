using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// The marker interface for the message delivery cleanup worker.
/// </summary>
internal interface IMessageDeliveryCleanupWorker
{
    Task Execute();
}
