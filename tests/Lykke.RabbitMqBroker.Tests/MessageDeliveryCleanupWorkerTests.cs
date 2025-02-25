using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
[NonParallelizable]
internal sealed class MessageDeliveryCleanupWorkerTests
{
    private FakeTimeProvider _timeProvider;
    private MessageDeliveryInMemoryStorage _seededStorage;

    [SetUp]
    public void SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
        _seededStorage = new MessageDeliveryInMemoryStorage();

        SeedStorage();
    }


    [Test]
    public async Task Execute_WipesOut_Deliveries_Below_RetentionPeriod()
    {
        var sut = new MessageDeliveryCleanupWorker(
            _seededStorage,
            _timeProvider,
            retentionPeriod: TimeSpan.FromMinutes(60));

        _timeProvider.Advance(TimeSpan.FromMinutes(61));

        await sut.Execute();

        Assert.That(_seededStorage.IsEmpty);
    }

    [Test]
    public async Task Execute_Preserves_Deliveries_Above_RetentionPeriod()
    {
        var sut = new MessageDeliveryCleanupWorker(
            _seededStorage,
            _timeProvider,
            retentionPeriod: TimeSpan.FromMinutes(60));

        _timeProvider.Advance(TimeSpan.FromMinutes(61));

        var preservedDelivery = new MessageDeliveryWithDefaults().TrySetDispatched(_timeProvider.GetUtcNow().DateTime);
        await _seededStorage.AddOrUpdate(preservedDelivery);

        _timeProvider.Advance(TimeSpan.FromMinutes(59));

        await sut.Execute();

        Assert.That(await _seededStorage.Get(preservedDelivery.Id), Is.EqualTo(preservedDelivery));
    }

    private void SeedStorage()
    {
        var now = _timeProvider.GetUtcNow().DateTime;

        _seededStorage.AddOrUpdate(new MessageDeliveryWithDefaults().TrySetDispatched(now));
        _seededStorage.AddOrUpdate(new MessageDeliveryWithDefaults().TrySetDispatched(now).TrySetReceived(now.AddSeconds(5)));
        _seededStorage.AddOrUpdate(new MessageDeliveryWithDefaults().TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.DispatchError, dateTime: now)));
    }
}
