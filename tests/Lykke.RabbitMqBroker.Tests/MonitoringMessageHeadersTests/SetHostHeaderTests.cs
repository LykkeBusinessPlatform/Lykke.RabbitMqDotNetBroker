using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageHeadersTests;

[TestFixture]
internal sealed class SetHostHeaderTests
{
    [Test]
    public void When_Properties_Is_Null_Then_Does_Nothing()
    {
        var properties = default(IBasicProperties);

        properties.SetHostHeader("host");

        Assert.Pass();
    }

    [Test]
    public void Sets_Header()
    {
        const string host = "host";
        var properties = new FakeProperties();

        properties.SetHostHeader(host);

        Assert.That(properties.Headers, Is.Not.Null);
        Assert.That(properties.Headers, Does.ContainKey(MonitoringMessageHeaders.HostHeader));
        Assert.That(properties.Headers[MonitoringMessageHeaders.HostHeader], Is.EqualTo(host));
    }
}