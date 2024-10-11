using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker;

/// <summary>
/// The marker interface for the message delivery analysis worker.
/// </summary>
internal interface IMessageDeliveryAnalysisWorker
{
    Task Execute();
}
