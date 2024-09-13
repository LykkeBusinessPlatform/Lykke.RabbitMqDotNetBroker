using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal class ListenerRegistrationTests
{
    class Model1 { }
    class Model2 { }

    [Test]
    public void CanBeCreated()
    {
        var registration = ListenerRegistration<object>.Create("exchange", "queue");
        Assert.That(registration, Is.Not.Null);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ExchangeNameIsRequired(string exchangeName)
    {
        Assert.That(() => ListenerRegistration<object>.Create(exchangeName, "queue"), Throws.ArgumentException);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void QueueNameIsRequired(string queueName)
    {
        Assert.That(() => ListenerRegistration<object>.Create("exchange", queueName), Throws.ArgumentException);
    }

    [Test]
    public void Equals()
    {
        var registration1 = ListenerRegistration<object>.Create("exchange", "queue", "routingKey");
        var registration2 = ListenerRegistration<object>.Create("exchange", "queue", "routingKey");
        Assert.That(registration1, Is.EqualTo(registration2));
    }

    [Test]
    public void Equals_RoutingKey_Matters()
    {
        var registration1 = ListenerRegistration<object>.Create("exchange", "queue", "routingKey1");
        var registration2 = ListenerRegistration<object>.Create("exchange", "queue", "routingKey2");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void Equals_ExchangeName_Matters()
    {
        var registration1 = ListenerRegistration<object>.Create("exchange1", "queue", "routingKey");
        var registration2 = ListenerRegistration<object>.Create("exchange2", "queue", "routingKey");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void Equals_QueueName_Matters()
    {
        var registration1 = ListenerRegistration<object>.Create("exchange", "queue1", "routingKey");
        var registration2 = ListenerRegistration<object>.Create("exchange", "queue2", "routingKey");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void ToString_Contains_RoutingKey_WhenSet()
    {
        var registration = ListenerRegistration<object>.Create("exchange", "queue", "routingKey");
        Assert.That(registration.ToString(), Contains.Substring("routingKey"));
    }
}