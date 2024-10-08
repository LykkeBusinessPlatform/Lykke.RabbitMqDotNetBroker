// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker;

public interface IConnectionProvider : IDisposable
{
    /// <summary>
    /// Gets a shared connection to the RabbitMQ broker.
    /// If there is no connection for the specified connection string, a new connection is created.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns></returns>
    public IAutorecoveringConnection GetOrCreateShared(string connectionString);

    /// <summary>
    /// Creates a new connection to the RabbitMQ broker.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="name">Client connection name, visible in RabbitMQ dashboard</param>
    /// <returns></returns>
    public IAutorecoveringConnection GetExclusive(string connectionString, string name = null);
}