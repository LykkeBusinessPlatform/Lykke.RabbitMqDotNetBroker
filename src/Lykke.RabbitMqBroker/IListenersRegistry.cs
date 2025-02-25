using System.Collections.Generic;

namespace Lykke.RabbitMqBroker
{
    public interface IListenersRegistry : IEnumerable<IListenerRegistration>
    {
        bool Add(IListenerRegistration registration);
    }
}