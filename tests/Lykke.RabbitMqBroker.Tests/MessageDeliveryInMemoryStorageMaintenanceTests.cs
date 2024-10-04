using System;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using NUnit.Framework;

using Microsoft.Extensions.Time.Testing;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageDeliveryMaintenanceTests
{
    private IMessageDeliveryMaintenance _maintenance;
    private IMessageDeliveryStorage _storage;

    [SetUp]
    public void SetUp()
    {
        _maintenance = new MessageDeliveryInMemoryStorage();
        _storage = _maintenance as IMessageDeliveryStorage;
    }

    [Test]
    public async Task Delete_WhenDeleted_ShouldNotBeAbleToGetById()
    {
        var messageDelivery = MessageDelivery.Create();
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
        var messageDeliveries = Enumerable.Range(0, 10).Select(_ => MessageDelivery.Create());
        await Task.WhenAll(messageDeliveries.Select(_storage.AddOrUpdate));

        await _maintenance.Delete(messageDeliveries.Select(x => x.Id));

        await Assert.ThatAsync(() => Task.WhenAll(messageDeliveries.Select(x => _storage.Get(x.Id))), Is.All.EqualTo(MessageDelivery.None));
    }

    [Test]
    public async Task GetChangedSince_WhenNoChanges_ShouldReturnEmpty()
    {
        var messageDelivery = MessageDelivery.Create();
        await _storage.AddOrUpdate(messageDelivery);

        var result = _maintenance.GetChangedSince(DateTime.MinValue);


        Assert.That(result.ToEnumerable(), Is.Empty);
    }

    [Test]
    public async Task GetChangedSince_WhenDispatchedSince_ShouldReturn()
    {
        FakeTimeProvider timeProvider = new(DateTimeOffset.Now);
        var messageDelivery = MessageDelivery.Create().TrySetDispatched(timeProvider.GetLocalNow().DateTime);
        await _storage.AddOrUpdate(messageDelivery);

        var result = _maintenance.GetChangedSince(timeProvider.GetLocalNow().DateTime);

        Assert.That(result.ToEnumerable(), Is.EquivalentTo(new[] { messageDelivery }));
    }

    [Test]
    public async Task GetChangedSince_WhenReceivedSince_ShouldReturn()
    {
        FakeTimeProvider timeProvider = new(DateTimeOffset.Now);
        var dispatchedDelivery = MessageDelivery.Create().TrySetDispatched(timeProvider.GetLocalNow().DateTime);
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var beforeReceivalTime = timeProvider.GetLocalNow().DateTime;
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var receivedDelivery = dispatchedDelivery.TrySetReceived(timeProvider.GetLocalNow().DateTime);
        await _storage.AddOrUpdate(receivedDelivery);

        var result = _maintenance.GetChangedSince(beforeReceivalTime);

        Assert.That(result.ToEnumerable(), Is.EquivalentTo(new[] { receivedDelivery }));
    }

    [Test]
    public async Task GetChangedSince_WhenFailedSince_ShouldReturn()
    {
        FakeTimeProvider timeProvider = new(DateTimeOffset.Now);
        var dispatchedDelivery = MessageDelivery.Create().TrySetDispatched(timeProvider.GetLocalNow().DateTime);
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var beforeFailureTime = timeProvider.GetLocalNow().DateTime;
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var failedDelivery = dispatchedDelivery.TrySetFailed(
            MessageDeliveryFailure.Create(
                reason: MessageDeliveryFailureReason.Uncategorised,
                dateTime: timeProvider.GetLocalNow().DateTime));
        await _storage.AddOrUpdate(failedDelivery);

        var result = _maintenance.GetChangedSince(beforeFailureTime);

        Assert.That(result.ToEnumerable(), Is.EquivalentTo(new[] { failedDelivery }));
    }

    [Test]
    public async Task GetChangedSince_WhenMultipleMixedChangesSince_ShouldReturn()
    {
        FakeTimeProvider timeProvider = new(DateTimeOffset.Now);
        var messagedelivery1 = MessageDelivery.Create().TrySetDispatched(timeProvider.GetLocalNow().DateTime);
        var messagedelivery2 = MessageDelivery.Create();
        var messagedelivery3 = MessageDelivery.Create().TrySetDispatched(timeProvider.GetLocalNow().DateTime);
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var sinceTime = timeProvider.GetLocalNow().DateTime;
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        messagedelivery1 = messagedelivery1.TrySetReceived(sinceTime);
        messagedelivery2 = messagedelivery2.TrySetDispatched(sinceTime).TrySetFailed(
            MessageDeliveryFailure.Create(
                reason: MessageDeliveryFailureReason.Uncategorised,
                dateTime: timeProvider.GetLocalNow().DateTime)
        );
        await _storage.AddOrUpdate(messagedelivery1);
        await _storage.AddOrUpdate(messagedelivery2);
        await _storage.AddOrUpdate(messagedelivery3);

        var result = _maintenance.GetChangedSince(sinceTime);

        Assert.That(result.ToEnumerable(), Is.EquivalentTo(new[] { messagedelivery1, messagedelivery2 }));
    }

    [Test]
    public async Task GetChangedSince_WhenEmptyTimestampsInvolved_ShouldIgnoreThem()
    {
        FakeTimeProvider timeProvider = new(DateTimeOffset.Now);
        var messagedelivery1 = MessageDelivery.Create().TrySetDispatched(timeProvider.GetLocalNow().DateTime);
        var messagedelivery2 = MessageDelivery.Create(); // empty dates
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var sinceTime = timeProvider.GetLocalNow().DateTime;
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        messagedelivery1 = messagedelivery1.TrySetReceived(sinceTime);
        var messagedelivery3 = MessageDelivery.Create(); // empty dates
        await _storage.AddOrUpdate(messagedelivery1);
        await _storage.AddOrUpdate(messagedelivery2);
        await _storage.AddOrUpdate(messagedelivery3);

        var result = _maintenance.GetChangedSince(sinceTime);

        Assert.That(result.ToEnumerable(), Is.EquivalentTo(new[] { messagedelivery1 }));
    }

    [Test]
    public async Task GetChangedSince_WhenOnlyEmptyTimestamps_ShouldReturnEmpty()
    {
        FakeTimeProvider timeProvider = new(DateTimeOffset.Now);
        var sinceTime = timeProvider.GetLocalNow().DateTime;
        timeProvider.Advance(TimeSpan.FromSeconds(1));
        var messagedelivery1 = MessageDelivery.Create(); // empty dates
        var messagedelivery2 = MessageDelivery.Create(); // empty dates
        await _storage.AddOrUpdate(messagedelivery1);
        await _storage.AddOrUpdate(messagedelivery2);

        var result = _maintenance.GetChangedSince(sinceTime);

        Assert.That(result.ToEnumerable(), Is.Empty);
    }
}
