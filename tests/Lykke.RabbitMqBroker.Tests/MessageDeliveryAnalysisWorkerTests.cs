using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
[NonParallelizable]
internal sealed class MessageDeliveryAnalysisWorkerTests
{
    class FakeMonitoringIssueNotifier : IMonitoringIssueNotifier
    {
        public int Notified { get; private set; }
        public Task Notify(MessageDelivery messageDelivery)
        {
            Notified++;
            return Task.CompletedTask;
        }
    }

    private FakeTimeProvider _timeProvider;
    private MessageDeliveryInMemoryStorage _seededStorage;
    private FakeMonitoringIssueNotifier _monitoringIssueNotifier;

    [SetUp]
    public async Task SetUp()
    {
        _timeProvider = new FakeTimeProvider(DateTime.UtcNow);
        _seededStorage = new MessageDeliveryInMemoryStorage();
        _monitoringIssueNotifier = new FakeMonitoringIssueNotifier();

        await SeedStorage();
    }

    [Test]
    public async Task Execute_Notifies_About_All_NotDelivered()
    {
        var sut = new MessageDeliveryAnalysisWorker(_seededStorage, _monitoringIssueNotifier);

        await sut.Execute();

        Assert.That(_monitoringIssueNotifier.Notified, Is.EqualTo(2));
    }

    private async Task SeedStorage()
    {
        var now = _timeProvider.GetUtcNow().DateTime;

        await _seededStorage.AddOrUpdate(new MessageDeliveryWithDefaults().TrySetDispatched(now));
        await _seededStorage
            .AddOrUpdate(MessageDelivery.Create(
                MessageRoute.Create(
                    NonEmptyString.Create("e"),
                    NonEmptyString.Create("q"),
                    "r"))
            .TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.DispatchError, dateTime: now)));
    }
}
