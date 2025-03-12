using System;
using System.Collections;
using System.Collections.Generic;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

class NonEmptyListenersRegistry : IListenersRegistry
{
    private readonly List<IListenerRegistration> _registrations;

    public NonEmptyListenersRegistry(List<IListenerRegistration> registrations)
    {
        _registrations = registrations;
    }

    public bool Add(IListenerRegistration registration)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<IListenerRegistration> GetEnumerator()
    {
        foreach (var registration in _registrations)
        {
            yield return registration;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}