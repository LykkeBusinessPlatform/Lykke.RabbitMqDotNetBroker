using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageDeliveryInMemoryStorageTests
{
    [Test]
    public async Task Add_WhenAdded_ShouldReturnTrue()
    {
        var storage = new MessageDeliveryInMemoryStorage();
        var messageDelivery = new MessageDelivery(Guid.NewGuid());

        var added = await storage.Add(messageDelivery);

        Assert.That(added);
    }

    [Test]
    public async Task Add_WhenAdded_ShouldBeAbleToGetById()
    {
        var storage = new MessageDeliveryInMemoryStorage();
        var messageDelivery = new MessageDelivery(Guid.NewGuid());

        var added = await storage.Add(messageDelivery);
        Assert.That(added);

        var existingMessageDelivery = await storage.Get(messageDelivery.Id);
        Assert.That(existingMessageDelivery, Is.Not.Null);
        Assert.That(existingMessageDelivery, Is.EqualTo(messageDelivery));
    }

    [Test]
    public async Task Added_WhenAdded_ShouldBeAbleToGetByMessageId()
    {
        var storage = new MessageDeliveryInMemoryStorage();
        var messageId = Guid.NewGuid();
        var messageDelivery = new MessageDelivery(messageId);

        var added = await storage.Add(messageDelivery);
        Assert.That(added);

        var existingMessageDeliveries = await storage.GetByMessageId(messageId);
        Assert.That(existingMessageDeliveries, Is.Not.Null);
        Assert.That(existingMessageDeliveries, Has.One.Items);
        Assert.That(existingMessageDeliveries, Has.One.EqualTo(messageDelivery));
    }

    [Test]
    public async Task Added_WhenAdded_TwoDeliveries_ForSameMessage_ShouldBeAbleToGetByMessageId()
    {
        var storage = new MessageDeliveryInMemoryStorage();
        var messageId = Guid.NewGuid();
        var messageDelivery1 = new MessageDelivery(messageId);
        var messageDelivery2 = new MessageDelivery(messageId);

        var addedFirst = await storage.Add(messageDelivery1);
        Assert.That(addedFirst);

        var addedSecond = await storage.Add(messageDelivery2);
        Assert.That(addedSecond);

        var existingMessageDeliveries = await storage.GetByMessageId(messageId);
        Assert.That(existingMessageDeliveries, Is.Not.Null);
        Assert.That(existingMessageDeliveries, Has.Exactly(2).Items);
        Assert.That(existingMessageDeliveries, Has.One.EqualTo(messageDelivery1));
        Assert.That(existingMessageDeliveries, Has.One.EqualTo(messageDelivery2));
    }
}