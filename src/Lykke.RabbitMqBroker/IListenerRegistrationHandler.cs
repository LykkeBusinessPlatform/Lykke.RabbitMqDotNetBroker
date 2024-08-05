using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker
{
    public interface IListenerRegistrationHandler
    {
        string Name { get; }
        Task Handle(IListenerRegistration registration);
    }
}