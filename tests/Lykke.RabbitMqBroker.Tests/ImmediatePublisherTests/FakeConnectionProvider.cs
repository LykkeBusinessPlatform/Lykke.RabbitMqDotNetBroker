using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.ImmediatePublisherTests;

internal sealed class FakeConnectionProvider : IConnectionProvider
{
    public uint ExclusiveConnectionCount { get; private set; } = 0;
    public uint SharedConnectionCount { get; private set; } = 0;
    public FakeConnection LatestConnection { get; private set; } = null;

    public void Dispose()
    {
    }

    public IAutorecoveringConnection GetExclusive(string connectionString, string name = null)
    {
        ExclusiveConnectionCount++;
        var connection = new FakeConnection();
        LatestConnection = connection;
        return connection;
    }

    public IAutorecoveringConnection GetOrCreateShared(string connectionString)
    {
        SharedConnectionCount++;
        var connection = new FakeConnection();
        LatestConnection = connection;
        return connection;
    }
}
