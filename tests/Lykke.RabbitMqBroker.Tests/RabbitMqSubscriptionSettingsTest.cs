using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture]
    internal class RabbitMqSubscriptionSettingsTest
    {
        [Test]
        public void PublisherSettingsDefaultNamespaceTest()
        {
            var connString = "test.ConnString";
            var endPoint = "test.Endpoint";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForPublisher(connString, endPoint);

            Assert.That(connString, Is.EqualTo(shortPubSettings.ConnectionString));
            Assert.That(shortPubSettings.DeadLetterExchangeName, Is.Null);
            Assert.That($"{RabbitMqSubscriptionSettings.LykkeNameSpace}.{endPoint}", Is.EqualTo(shortPubSettings.ExchangeName));
            Assert.That(shortPubSettings.IsDurable, Is.False);
            Assert.That(shortPubSettings.QueueName, Is.Null);
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionDelay, Is.EqualTo(shortPubSettings.ReconnectionDelay));
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionsCountToAlarm, Is.EqualTo(shortPubSettings.ReconnectionsCountToAlarm));
            Assert.That(string.Empty, Is.EqualTo(shortPubSettings.RoutingKey));
        }

        [Test]
        public void PublisherSettingsCustomNamespaceTest()
        {
            var connString = "test.ConnString";
            var endPoint = "test.Endpoint";
            var namespace1 = "test.Namespace";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForPublisher(
                connString,
                namespace1,
                endPoint);

            Assert.That(connString, Is.EqualTo(shortPubSettings.ConnectionString));
            Assert.That(shortPubSettings.DeadLetterExchangeName, Is.Null);
            Assert.That($"{namespace1}.{endPoint}", Is.EqualTo(shortPubSettings.ExchangeName));
            Assert.That(shortPubSettings.IsDurable, Is.False);
            Assert.That(shortPubSettings.QueueName, Is.Null);
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionDelay, Is.EqualTo(shortPubSettings.ReconnectionDelay));
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionsCountToAlarm, Is.EqualTo(shortPubSettings.ReconnectionsCountToAlarm));
            Assert.That(string.Empty, Is.EqualTo(shortPubSettings.RoutingKey));
        }

        [Test]
        public void PublisherSettingsCustomLykkeEndpointTest()
        {
            var endPoint = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Endpoint";
            var namespace1 = RabbitMqSubscriptionSettings.LykkeNameSpace;

            var shortPubSettings = RabbitMqSubscriptionSettings.ForPublisher(
                "test.ConnString",
                namespace1,
                endPoint);

            Assert.That(endPoint, Is.EqualTo(shortPubSettings.ExchangeName));
        }

        [Test]
        public void PublisherSettingsCustomLykkeStartEndpointTest()
        {
            var endPoint = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Endpoint";
            var namespace1 = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Namespace";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForPublisher(
                "test.ConnString",
                namespace1,
                endPoint);

            Assert.That($"{namespace1}.{endPoint}", Is.EqualTo(shortPubSettings.ExchangeName));
        }

        [Test]
        public void SubscriberSettingsDefaultNamespacesTest()
        {
            var connString = "test.ConnString";
            var source = "test.Source";
            var endPoint = "test.Endpoint";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForSubscriber(
                connString,
                source,
                endPoint);

            Assert.That(connString, Is.EqualTo(shortPubSettings.ConnectionString));
            Assert.That($"{RabbitMqSubscriptionSettings.LykkeNameSpace}.{endPoint}.{source}.dlx", Is.EqualTo(shortPubSettings.DeadLetterExchangeName));
            Assert.That($"{RabbitMqSubscriptionSettings.LykkeNameSpace}.{source}", Is.EqualTo(shortPubSettings.ExchangeName));
            Assert.That(shortPubSettings.IsDurable, Is.False);
            Assert.That($"{RabbitMqSubscriptionSettings.LykkeNameSpace}.{source}.{endPoint}", Is.EqualTo(shortPubSettings.QueueName));
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionDelay, Is.EqualTo(shortPubSettings.ReconnectionDelay));
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionsCountToAlarm, Is.EqualTo(shortPubSettings.ReconnectionsCountToAlarm));
            Assert.That(string.Empty, Is.EqualTo(shortPubSettings.RoutingKey));
        }

        [Test]
        public void SubscriberSettingsCustomNamespacesTest()
        {
            var connString = "test.ConnString";
            var sourceNamespace = "test.SourceNamespace";
            var source = "test.Source";
            var endpointNamespace = "test.EndpointNamespace";
            var endPoint = "test.Endpoint";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForSubscriber(
                connString,
                sourceNamespace,
                source,
                endpointNamespace,
                endPoint);

            Assert.That(connString, Is.EqualTo(shortPubSettings.ConnectionString));
            Assert.That($"{endpointNamespace}.{endPoint}.{source}.dlx", Is.EqualTo(shortPubSettings.DeadLetterExchangeName));
            Assert.That($"{sourceNamespace}.{source}", Is.EqualTo(shortPubSettings.ExchangeName));
            Assert.That(shortPubSettings.IsDurable, Is.False);
            Assert.That($"{sourceNamespace}.{source}.{endPoint}", Is.EqualTo(shortPubSettings.QueueName));
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionDelay, Is.EqualTo(shortPubSettings.ReconnectionDelay));
            Assert.That(RabbitMqSubscriptionSettings.DefaultReconnectionsCountToAlarm, Is.EqualTo(shortPubSettings.ReconnectionsCountToAlarm));
            Assert.That(string.Empty, Is.EqualTo(shortPubSettings.RoutingKey));
        }

        [Test]
        public void SubscriberSettingsCustomLykkeNamespacesTest()
        {
            var sourceNamespace = RabbitMqSubscriptionSettings.LykkeNameSpace;
            var source = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Source";
            var endpointNamespace = RabbitMqSubscriptionSettings.LykkeNameSpace;
            var endPoint = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Endpoint";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForSubscriber(
                "test.ConnString",
                sourceNamespace,
                source,
                endpointNamespace,
                endPoint);

            Assert.That($"{endPoint}.{source}.dlx", Is.EqualTo(shortPubSettings.DeadLetterExchangeName));
            Assert.That($"{source}", Is.EqualTo(shortPubSettings.ExchangeName));
            Assert.That($"{source}.{endPoint}", Is.EqualTo(shortPubSettings.QueueName));
        }

        [Test]
        public void SubscriberSettingsCustomLykkeStartNamespacesTest()
        {
            var sourceNamespace = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.SourceNamespace";
            var source = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Source";
            var endpointNamespace = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.EndpointNamespace";
            var endPoint = $"{RabbitMqSubscriptionSettings.LykkeNameSpace}.Endpoint";

            var shortPubSettings = RabbitMqSubscriptionSettings.ForSubscriber(
                "test.ConnString",
                sourceNamespace,
                source,
                endpointNamespace,
                endPoint);

            Assert.That($"{endpointNamespace}.{endPoint}.{source}.dlx", Is.EqualTo(shortPubSettings.DeadLetterExchangeName));
            Assert.That($"{sourceNamespace}.{source}", Is.EqualTo(shortPubSettings.ExchangeName));
            Assert.That($"{sourceNamespace}.{source}.{endPoint}", Is.EqualTo(shortPubSettings.QueueName));
        }
    }
}
