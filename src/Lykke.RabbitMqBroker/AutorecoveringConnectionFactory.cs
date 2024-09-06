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
        public IAutorecoveringConnection Create(string connectionString, string displayName)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString, UriKind.Absolute),
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(60),
                ContinuationTimeout = TimeSpan.FromSeconds(30),
                ClientProvidedName = displayName
            };

            // todo: improve here since according to documentation:
            // "If initial client connection to a RabbitMQ node fails, 
            // automatic connection recovery won't kick in. 
            // Applications developers are responsible for retrying such connections".
            // https://www.rabbitmq.com/client-libraries/dotnet-api-guide#recovery-triggers
            return factory.CreateConnection() as IAutorecoveringConnection;
        }
    }
}
