using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageDeliveryInMemoryStorageTests
{
    [Test]
    public async Task Add_WhenAdded_ShouldBeAbleToGetById()
    {
        var storage = new MessageDeliveryInMemoryStorage();
        MessageDelivery messageDelivery = new();

        await storage.Add(messageDelivery);

        var existingMessageDelivery = await storage.Get(messageDelivery.Id);
        Assert.That(existingMessageDelivery, Is.Not.Null);
        Assert.That(existingMessageDelivery, Is.EqualTo(messageDelivery));
    }
}