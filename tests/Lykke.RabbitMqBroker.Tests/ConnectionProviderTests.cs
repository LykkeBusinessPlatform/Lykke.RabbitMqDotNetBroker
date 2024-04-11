// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Lykke.RabbitMqBroker.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture]
    public class ConnectionProviderTests
    {
        [Test]
        public void Gets_Same_Shared_Connection_For_Same_Connection_String()
        {
            var sut = CreateSut();
            const string connectionString = "connectionString";

            var connection1 = sut.GetOrCreateShared(connectionString);
            var connection2 = sut.GetOrCreateShared(connectionString);

            Assert.AreEqual(connection1, connection2);
        }
        
        [Test]
        public void Gets_Separate_Shared_Connection_For_Different_Connection_Strings()
        {
            var sut = CreateSut();

            var connection1 = sut.GetOrCreateShared("connectionString1");
            var connection2 = sut.GetOrCreateShared("connectionString2");

            Assert.AreNotEqual(connection1, connection2);
        }
        
        [Test]
        public void Gets_Separate_Shared_And_Exclusive_Connections_For_Same_Connection_String()
        {
            var sut = CreateSut();
            const string connectionString = "connectionString";

            var sharedConnection = sut.GetOrCreateShared(connectionString);
            var exclusiveConnection = sut.GetExclusive(connectionString);

            Assert.AreNotEqual(sharedConnection, exclusiveConnection);
        }
        
        [Test]
        public void Gets_Separate_Exclusive_Connections_For_Same_Connection_String()
        {
            var sut = CreateSut();
            const string connectionString = "connectionString";

            var exclusiveConnection1 = sut.GetExclusive(connectionString);
            var exclusiveConnection2 = sut.GetExclusive(connectionString);

            Assert.AreNotEqual(exclusiveConnection1, exclusiveConnection2);
        }
        
        private static IConnectionProvider CreateSut()
        {
            return new ConnectionProvider(
                NullLogger<ConnectionProvider>.Instance,
                new FakeAutorecoveringConnectionFactory());
        }
    }
}
