using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker
{
    /// <summary>
    /// The marker interface for the listeners registry worker.
    /// </summary>
    internal interface IListenersRegistryWorker
    {
        /// <summary>
        /// Processes the listeners.
        /// </summary>
        /// <returns></returns>
        Task Execute();
    }
}