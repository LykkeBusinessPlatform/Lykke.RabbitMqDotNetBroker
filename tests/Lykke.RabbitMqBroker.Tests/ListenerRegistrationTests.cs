using Lykke.RabbitMqBroker.Subscriber;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal class ListenerRegistrationTests
{
    class Model1 { }
    class Model2 { }

    private static ListenerRegistration<T> CreateRegistration<T>(string exchangeName, string queueName, string routingKey = null)
    {
        var route = routingKey != null
            ? ListenerRoute.Create(new ExchangeName(exchangeName), new QueueName(queueName), new RoutingKey(routingKey))
            : ListenerRoute.Create(new ExchangeName(exchangeName), new QueueName(queueName));
        return new ListenerRegistration<T>(route);
    }

    [Test]
    public void CanBeCreated()
    {
        var registration = CreateRegistration<object>("exchange", "queue");
        Assert.That(registration, Is.Not.Null);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ExchangeNameIsRequired(string exchangeName)
    {
        Assert.That(() => CreateRegistration<object>(exchangeName, "queue"), Throws.ArgumentException);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void QueueNameIsRequired(string queueName)
    {
        Assert.That(() => CreateRegistration<object>("exchange", queueName), Throws.ArgumentException);
    }

    [Test]
    public void Equals()
    {
        var registration1 = CreateRegistration<object>("exchange", "queue", "routingKey");
        var registration2 = CreateRegistration<object>("exchange", "queue", "routingKey");
        Assert.That(registration1, Is.EqualTo(registration2));
    }

    [Test]
    public void Equals_RoutingKey_Matters()
    {
        var registration1 = CreateRegistration<object>("exchange", "queue", "routingKey1");
        var registration2 = CreateRegistration<object>("exchange", "queue", "routingKey2");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void Equals_ExchangeName_Matters()
    {
        var registration1 = CreateRegistration<object>("exchange1", "queue", "routingKey");
        var registration2 = CreateRegistration<object>("exchange2", "queue", "routingKey");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void Equals_QueueName_Matters()
    {
        var registration1 = CreateRegistration<object>("exchange", "queue1", "routingKey");
        var registration2 = CreateRegistration<object>("exchange", "queue2", "routingKey");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void ToString_Contains_RoutingKey_WhenSet()
    {
        var registration = CreateRegistration<object>("exchange", "queue", "routingKey");
        Assert.That(registration.ToString(), Contains.Substring("routingKey"));
    }
}