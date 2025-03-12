using System;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

class EmptyListenersRegistry : IListenersRegistry
{
    public bool Add(IListenerRegistration registration)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IListenerRegistration> GetEnumerator()
    {
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}