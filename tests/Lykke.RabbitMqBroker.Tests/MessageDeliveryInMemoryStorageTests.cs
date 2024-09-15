using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageDeliveryInMemoryStorageTests
{
    private IMessageDeliveryStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _storage = new MessageDeliveryInMemoryStorage();
    }

    [Test]
    public async Task AddOrUpdate_WhenAdded_ShouldBeAbleToGetById()
    {
        var messageDelivery = MessageDelivery.Create();

        await _storage.AddOrUpdate(messageDelivery);

        var addedDelivery = await _storage.Get(messageDelivery.Id);
        Assert.Multiple(() =>
        {
            Assert.That(addedDelivery.IsNone, Is.False);
            Assert.That(addedDelivery, Is.EqualTo(messageDelivery));
        });
    }

    [Test]
    public async Task AddOrUpdate_Does_Not_Add_None()
    {
        var added = await _storage.AddOrUpdate(MessageDelivery.None);

        Assert.That(added, Is.False);
    }

    [Test]
    public async Task AddOrUpdate_WhenUpdated_ShouldBeAbleToGetById()
    {
        var messageDelivery = MessageDelivery.Create();
        await _storage.AddOrUpdate(messageDelivery);

        var updatedDelivery = messageDelivery.TrySetDispatched(DateTime.UtcNow);
        var updated = await _storage.AddOrUpdate(updatedDelivery);

        var fromStorage = await _storage.Get(messageDelivery.Id);

        Assert.Multiple(() =>
        {
            Assert.That(updated);
            Assert.That(fromStorage, Is.EqualTo(updatedDelivery));
        });
    }
}