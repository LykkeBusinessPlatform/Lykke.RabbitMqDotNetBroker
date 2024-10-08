using System;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using NUnit.Framework;

using Microsoft.Extensions.Time.Testing;
using Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
[NonParallelizable]
internal sealed class MessageDeliveryMaintenanceTests
{
    private IMessageDeliveryMaintenance _maintenance;
    private IMessageDeliveryStorage _storage;
    private FakeTimeProvider _timeProvider;

    [SetUp]
    public void SetUp()
    {
        _maintenance = new MessageDeliveryInMemoryStorage();
        _storage = _maintenance as IMessageDeliveryStorage;
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
    }

    [Test]
    public async Task Delete_WhenDeleted_ShouldNotBeAbleToGetById()
    {
        var messageDelivery = new MessageDeliveryWithDefaults();
        await _storage.AddOrUpdate(messageDelivery);

        await _maintenance.Delete(messageDelivery.Id);

        await Assert.ThatAsync(() => _storage.Get(messageDelivery.Id), Is.EqualTo(MessageDelivery.None));
    }

    [Test]
    public void Delete_WhenNonExistent_ShouldNotThrow()
    {
        Assert.DoesNotThrowAsync(async () => await _maintenance.Delete(MessageDeliveryId.Create()));
    }

    [Test]
    public async Task Delete_WhenMultipleDeleted_ShouldNotBeAbleToGetById()
    {
        var messageDeliveries = Enumerable.Range(0, 10).Select(_ => new MessageDeliveryWithDefaults());
        await Task.WhenAll(messageDeliveries.Select(_storage.AddOrUpdate));

        await _maintenance.Delete(messageDeliveries.Select(x => x.Id));

        await Assert.ThatAsync(() => Task.WhenAll(messageDeliveries.Select(x => _storage.Get(x.Id))), Is.All.EqualTo(MessageDelivery.None));
    }

    [Test]
    public async Task GetBeforeMoment_WhenBothOlderAndNot_ShouldReturnOnlyOlder()
    {
        var deliveryBeforeMoment = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);
        await _storage.AddOrUpdate(deliveryBeforeMoment);

        _timeProvider.Advance(TimeSpan.FromMinutes(1));
        var moment = _timeProvider.GetUtcNow().DateTime;

        _timeProvider.Advance(TimeSpan.FromMinutes(1));
        var deliveryAfterMoment = new MessageDeliveryWithDefaults()
            .TrySetDispatched(_timeProvider.GetUtcNow().DateTime);
        await _storage.AddOrUpdate(deliveryAfterMoment);

        var result = _maintenance.GetBeforeMoment(moment);

        Assert.That(result.ToEnumerable(), Is.EquivalentTo(new[] { deliveryBeforeMoment }));
    }
}
