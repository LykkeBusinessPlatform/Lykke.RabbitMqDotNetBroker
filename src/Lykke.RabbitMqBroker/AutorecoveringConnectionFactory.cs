// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker
{
    /// <summary>
    /// Factory for creating connections with autorecovery and topology recovery enabled.
    /// </summary>
    public sealed class AutorecoveringConnectionFactory : IAutorecoveringConnectionFactory
    {
        public IAutorecoveringConnection Create(string connectionString, string name)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString, UriKind.Absolute),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(60),
                ContinuationTimeout = TimeSpan.FromSeconds(30),
                ClientProvidedName = name
            };

            return factory.CreateConnection() as IAutorecoveringConnection;
        }
    }
}
