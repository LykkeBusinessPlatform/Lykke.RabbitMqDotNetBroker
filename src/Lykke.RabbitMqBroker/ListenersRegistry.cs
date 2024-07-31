using System;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.RabbitMqBroker
{
    public class ListenersRegistry : IListenersRegistry
    {
        private readonly HashSet<IListenerRegistration> _registrations = new();

        public bool Add(IListenerRegistration registration)
        {
            if (registration == null)
            {
                throw new ArgumentNullException(nameof(registration));
            }

            return _registrations.Add(registration);
        }

        public IEnumerator<IListenerRegistration> GetEnumerator()
        {
            return _registrations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}