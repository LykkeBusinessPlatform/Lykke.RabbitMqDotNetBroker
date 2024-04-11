// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker
{
    /// <summary>
    /// The purpose of this interface is to abstract the creation of autorecovering connections to RabbitMQ.
    /// </summary>
    public interface IAutorecoveringConnectionFactory
    {
        public IAutorecoveringConnection Create(string connectionString, string name);
    }
}
