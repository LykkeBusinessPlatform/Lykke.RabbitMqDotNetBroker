using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringChannelProviderTests;

internal sealed class FakeConnectionProvider : IConnectionProvider
{
    public int ExclusiveConnectionsCount { get; private set; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public IAutorecoveringConnection GetExclusive(string connectionString, string name = null)
    {
        ExclusiveConnectionsCount++;
        return new FakeConnection();
    }

    public IAutorecoveringConnection GetOrCreateShared(string connectionString)
    {
        return new FakeConnection();
    }
}
