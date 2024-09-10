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
    public async Task Add_WhenAdded_ShouldBeAbleToGetById()
    {
        MessageDelivery messageDelivery = new();

        await _storage.Add(messageDelivery);

        var existingMessageDelivery = await _storage.Get(messageDelivery.Id);
        Assert.That(existingMessageDelivery, Is.Not.Null);
        Assert.That(existingMessageDelivery, Is.EqualTo(messageDelivery));
    }

    [Test]
    public async Task Add_WhenKeyAlreadyExists_ShouldThrowInvalidOperationException()
    {
        MessageDelivery messageDelivery = new();

        await _storage.Add(messageDelivery);

        Assert.That(async () => await _storage.Add(messageDelivery), Throws.InvalidOperationException);
    }

    [Test]
    public async Task Update_WhenUpdated_ShouldBeAbleToGetById()
    {
        MessageDelivery original = new();
        await _storage.Add(original);
        var updated = original.Dispatched(DateTime.UtcNow);
        await _storage.Update(updated);

        var fromStorage = await _storage.Get(updated.Id);
        Assert.That(fromStorage, Is.Not.Null);
        Assert.That(fromStorage, Is.EqualTo(updated));
        Assert.That(fromStorage, Is.Not.EqualTo(original));
    }
}