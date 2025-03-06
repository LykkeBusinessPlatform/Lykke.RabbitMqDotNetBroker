using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Subscriber;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageHeadersTests;

[TestFixture]
internal sealed class SetDestnationQueueTests
{
    [Test]
    public void When_Properties_Is_Null_Then_Does_Nothing()
    {
        var properties = default(IBasicProperties);

        properties.SetDestinationQueueHeader(QueueName.Create("queue"));

        Assert.Pass();
    }

    [Test]
    public void Sets_Header()
    {
        var queueName = QueueName.Create("queue");
        var properties = new FakeProperties();

        properties.SetDestinationQueueHeader(queueName);

        Assert.That(properties.Headers, Is.Not.Null);
        Assert.That(properties.Headers, Does.ContainKey(MonitoringMessageHeaders.DestinationQueueHeader));
        Assert.That(properties.Headers[MonitoringMessageHeaders.DestinationQueueHeader], Is.EqualTo(queueName.ToString()));
    }
}