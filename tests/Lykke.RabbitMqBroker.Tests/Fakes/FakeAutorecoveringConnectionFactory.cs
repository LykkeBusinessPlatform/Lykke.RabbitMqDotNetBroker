// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.Fakes
{
    internal class FakeAutorecoveringConnectionFactory : IAutorecoveringConnectionFactory
    {
        public IAutorecoveringConnection Create(string connectionString, string displayName)
        {
            return new FakeConnection();
        }
    }
}
