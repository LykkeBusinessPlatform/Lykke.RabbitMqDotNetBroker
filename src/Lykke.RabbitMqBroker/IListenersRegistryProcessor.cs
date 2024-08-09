using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker
{
    /// <summary>
    /// The marker interface for the listeners registry processor.
    /// </summary>
    internal interface IListenersRegistryProcessor
    {
        /// <summary>
        /// Processes the listeners.
        /// </summary>
        /// <returns></returns>
        Task ProcessListeners();
    }
}