using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal class ListenerRegistrationTests
{
    class Model1 { }
    class Model2 { }

    [Test]
    public void ListenerRegistration_CanBeCreated()
    {
        var registration = new ListenerRegistration<object>("exchange", "queue");
        Assert.That(registration, Is.Not.Null);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ListenerRegistration_ExchangeNameIsRequired(string exchangeName)
    {
        Assert.That(() => new ListenerRegistration<object>(exchangeName, "queue"), Throws.ArgumentException);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    public void ListenerRegistration_QueueNameIsRequired(string queueName)
    {
        Assert.That(() => new ListenerRegistration<object>("exchange", queueName), Throws.ArgumentException);
    }

    [Test]
    public void ListenerRegistration_Equals()
    {
        var registration1 = new ListenerRegistration<object>("exchange", "queue", "routingKey");
        var registration2 = new ListenerRegistration<object>("exchange", "queue", "routingKey");
        Assert.That(registration1, Is.EqualTo(registration2));
    }

    [Test]
    public void ListenerRegistration_Equals_RoutingKey_Matters()
    {
        var registration1 = new ListenerRegistration<object>("exchange", "queue", "routingKey1");
        var registration2 = new ListenerRegistration<object>("exchange", "queue", "routingKey2");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void ListenerRegistration_Equals_ExchangeName_Matters()
    {
        var registration1 = new ListenerRegistration<object>("exchange1", "queue", "routingKey");
        var registration2 = new ListenerRegistration<object>("exchange2", "queue", "routingKey");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }

    [Test]
    public void ListenerRegistration_Equals_QueueName_Matters()
    {
        var registration1 = new ListenerRegistration<object>("exchange", "queue1", "routingKey");
        var registration2 = new ListenerRegistration<object>("exchange", "queue2", "routingKey");
        Assert.That(registration1, Is.Not.EqualTo(registration2));
    }
}