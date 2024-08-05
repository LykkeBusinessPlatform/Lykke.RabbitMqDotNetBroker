using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker
{
    internal interface IListenersRegistryHandlersRunner
    {
        Task Run();
    }
}