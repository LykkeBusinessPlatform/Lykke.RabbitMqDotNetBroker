using System;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;
using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
[NonParallelizable]
internal sealed class MessageDeliveryInMemoryStorageTests
{
    private FakeTimeProvider _timeProvider;
    private IMessageDeliveryStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
        _storage = new MessageDeliveryInMemoryStorage();
    }

    [Test]
    public async Task AddOrUpdate_WhenAdded_ShouldBeAbleToGetById()
    {
        var messageDelivery = new MessageDeliveryWithDefaults();

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
        var messageDelivery = new MessageDeliveryWithDefaults();
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

    [Test]
    public async Task GetLastForEveryRoute_Gets_From_EveryRoute()
    {
        await SeedStorage();

        var latestDeliveries = _storage.GetLatestForEveryRoute().ToEnumerable();
        var theirRoutes = latestDeliveries.Select(d => d.Route);

        Assert.Multiple(() =>
        {
            Assert.That(latestDeliveries.Count(), Is.EqualTo(2));
            Assert.That(theirRoutes.Distinct().Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetLastForEveryRoute_Gets_Latest_Message()
    {
        var messageDelivery = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        _timeProvider.Advance(TimeSpan.FromMinutes(1));

        var anotherMessageDeliverySameRoute = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime)
            .TrySetReceived(_timeProvider.GetUtcNow().DateTime);

        await _storage.AddOrUpdate(messageDelivery);
        await _storage.AddOrUpdate(anotherMessageDeliverySameRoute);

        var latestDeliveries = _storage.GetLatestForEveryRoute().ToEnumerable();

        Assert.That(latestDeliveries.Single(), Is.EqualTo(anotherMessageDeliverySameRoute));
    }

    private async Task SeedStorage()
    {
        // route 1 messages
        var route1MessageDelivery = MessageDelivery.Create(
            MessageRoute.Create(
                NonEmptyString.Create("exchange1"),
                NonEmptyString.Create("queue1"),
                "routingKey1"))
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime)
            .TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.DispatchError, dateTime: _timeProvider.GetUtcNow().DateTime));

        _timeProvider.Advance(TimeSpan.FromMinutes(1));

        var route1AnotherMessageDelivery = MessageDelivery.Create(
            MessageRoute.Create(
                NonEmptyString.Create("exchange1"),
                NonEmptyString.Create("queue1"),
                "routingKey1"))
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime)
            .TrySetReceived(_timeProvider.GetUtcNow().DateTime);

        // route 2 messages
        var route2MessageDelivery = MessageDelivery.Create(
            MessageRoute.Create(
                NonEmptyString.Create("exchange2"),
                NonEmptyString.Create("queue2"),
                "routingKey2"))
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);

        await _storage.AddOrUpdate(route1MessageDelivery);
        await _storage.AddOrUpdate(route1AnotherMessageDelivery);
        await _storage.AddOrUpdate(route2MessageDelivery);
    }
}