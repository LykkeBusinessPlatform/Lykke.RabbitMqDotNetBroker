using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal class ListenersRegistryTests
{
    class MessageModel { }

    [Test]
    public void Add_RegistrationIsNull_ThrowsArgumentNullException()
    {
        var registry = new ListenersRegistry();

        Assert.That(() => registry.Add(null), Throws.ArgumentNullException);
    }

    [Test]
    public void Add_RegistrationIsNotNull_AddsRegistration()
    {
        var registry = new ListenersRegistry();
        var registration = new ListenerRegistration<MessageModel>("exchange", "queue");

        var added = registry.Add(registration);

        Assert.That(registry, Contains.Item(registration));
        Assert.That(added);
    }

    [Test]
    public void AddDuplicateRegistration_ReturnsFalse()
    {
        var registry = new ListenersRegistry();
        var registration = new ListenerRegistration<MessageModel>("exchange", "queue");

        var firstAdd = registry.Add(registration);
        var secondAdd = registry.Add(registration);

        Assert.That(firstAdd);
        Assert.That(secondAdd, Is.False);
    }

    [Test]
    public void AddMultipleRegistrations_AddsAll()
    {
        var registry = new ListenersRegistry();
        var registration1 = new ListenerRegistration<MessageModel>("exchange1", "queue1");
        var registration2 = new ListenerRegistration<MessageModel>("exchange1", "queue2");

        registry.Add(registration1);
        registry.Add(registration2);

        Assert.That(registry, Contains.Item(registration1));
        Assert.That(registry, Contains.Item(registration2));
    }
}