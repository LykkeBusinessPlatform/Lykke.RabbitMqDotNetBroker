using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageHeadersTests;

[TestFixture]
internal sealed class SetDeliveryIdHeaderTests
{
    [Test]
    public void When_Properties_Is_Null_Then_Does_Nothing()
    {
        var properties = default(IBasicProperties);

        properties.SetDeliveryIdHeader(MessageDeliveryId.Create());

        Assert.Pass();
    }

    [Test]
    public void Sets_Header()
    {
        var deliveryId = MessageDeliveryId.Create();
        var properties = new FakeProperties();

        properties.SetDeliveryIdHeader(deliveryId);

        Assert.That(properties.Headers, Is.Not.Null);
        Assert.That(properties.Headers, Does.ContainKey(MonitoringMessageHeaders.DeliveryIdHeader));
        Assert.That(properties.Headers[MonitoringMessageHeaders.DeliveryIdHeader], Is.EqualTo(deliveryId.ToString()));
    }
}
