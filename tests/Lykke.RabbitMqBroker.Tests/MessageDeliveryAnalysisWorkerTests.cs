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
        public int NotifiedAboutNotDeliveredCounter { get; private set; }
        public int NotifiedAboutLatelyDeliveredCounter { get; private set; }
        public Task NotifyNotDelivered(MessageDelivery messageDelivery)
        {
            NotifiedAboutNotDeliveredCounter++;
            return Task.CompletedTask;
        }

        public Task NotifyLateDelivery(MessageDelivery messageDelivery)
        {
            NotifiedAboutLatelyDeliveredCounter++;
            return Task.CompletedTask;
        }
    }

    private FakeTimeProvider _timeProvider;
    private MessageDeliveryInMemoryStorage _seededStorage;
    private FakeMonitoringIssueNotifier _monitoringIssueNotifier;

    private const int FairDelayPeriodMs = 10_000;

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
        var sut = new MessageDeliveryAnalysisWorker(
            _seededStorage,
            _monitoringIssueNotifier,
            _timeProvider);

        await sut.Execute();

        Assert.That(_monitoringIssueNotifier.NotifiedAboutNotDeliveredCounter, Is.EqualTo(2));
    }

    [Test]
    public async Task Execute_Notifies_About_Late_Deliveries()
    {
        var sut = new MessageDeliveryAnalysisWorker(
            _seededStorage,
            _monitoringIssueNotifier,
            _timeProvider,
            TimeSpan.FromMilliseconds(FairDelayPeriodMs));

        await sut.Execute();

        Assert.That(_monitoringIssueNotifier.NotifiedAboutLatelyDeliveredCounter, Is.EqualTo(1));
    }

    private async Task SeedStorage()
    {
        var now = _timeProvider.GetUtcNow().DateTime;

        // Not delivered messages (2)
        await _seededStorage
            .AddOrUpdate(new MessageDeliveryWithDefaults()
            .TrySetDispatched(now));

        await _seededStorage
            .AddOrUpdate(MessageDelivery.Create(
                MessageRoute.Create(
                    NonEmptyString.Create("e1"),
                    NonEmptyString.Create("q1"),
                    "r1"))
            .TrySetDispatched(now.AddSeconds(1))
            .TrySetFailed(MessageDeliveryFailure.Create(MessageDeliveryFailureReason.Unroutable, dateTime: now.AddSeconds(2))));

        // Delivered too late (1)
        await _seededStorage
            .AddOrUpdate(MessageDelivery.Create(
                MessageRoute.Create(
                    NonEmptyString.Create("e2"),
                    NonEmptyString.Create("q2"),
                    "r2"))
            .TrySetDispatched(now
                .AddSeconds(1))
            .TrySetReceived(now
                .AddSeconds(1)
                .AddMilliseconds(FairDelayPeriodMs)
                .AddMilliseconds(1)));
    }
}
