using System.Collections.Generic;
using System.Text;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageHeadersTests;

[TestFixture]
internal sealed class GetDeliveryIdTests
{
    [Test]
    public void When_Properties_Is_Null_Then_Returns_Empty()
    {
        var properties = default(IBasicProperties);

        Assert.That(properties.GetDeliveryId, Is.EqualTo(MessageDeliveryId.Empty));
    }

    [Test]
    public void When_ThereIs_No_DeliveryIdHeader_Then_Returns_Empty()
    {
        var properties = new FakeProperties();
        properties.Headers = new Dictionary<string, object>();

        Assert.That(properties.GetDeliveryId, Is.EqualTo(MessageDeliveryId.Empty));
    }

    [Test]
    public void When_DeliveryIdHeader_Is_Byte_Array_Then_Returns_Parsed_DeliveryId()
    {
        var deliveryId = MessageDeliveryId.Create();
        FakeProperties properties = new()
        {
            Headers = new Dictionary<string, object>
            {
                { MonitoringMessageHeaders.DeliveryIdHeader, Encoding.UTF8.GetBytes(deliveryId.ToString()) }
            }
        };

        Assert.That(properties.GetDeliveryId, Is.EqualTo(deliveryId));
    }

    [Test]
    public void When_DeliveryIdHeader_Is_String_Then_Returns_Parsed_DeliveryId()
    {
        var deliveryId = MessageDeliveryId.Create();
        FakeProperties properties = new()
        {
            Headers = new Dictionary<string, object>
            {
                { MonitoringMessageHeaders.DeliveryIdHeader, deliveryId.ToString() }
            }
        };

        Assert.That(properties.GetDeliveryId, Is.EqualTo(deliveryId));
    }

    [Test]
    public void When_DeliveryIdHeader_Is_Unknown_Type_Then_Returns_Empty()
    {
        FakeProperties properties = new()
        {
            Headers = new Dictionary<string, object>
            {
                { MonitoringMessageHeaders.DeliveryIdHeader, new object() }
            }
        };

        Assert.That(properties.GetDeliveryId, Is.EqualTo(MessageDeliveryId.Empty));
    }
}