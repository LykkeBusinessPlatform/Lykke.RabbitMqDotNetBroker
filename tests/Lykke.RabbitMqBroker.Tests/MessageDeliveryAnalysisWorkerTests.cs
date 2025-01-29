using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Gens = Lykke.RabbitMqBroker.TestDataGenerators.MessageDeliveryGens;

using Microsoft.Extensions.Time.Testing;

using NUnit.Framework;
using FsCheck.Fluent;
using System.Linq;

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
            _timeProvider,
            TimeSpan.FromMilliseconds(FairDelayPeriodMs));

        _timeProvider.Advance(TimeSpan.FromMilliseconds(FairDelayPeriodMs + 1));

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
        await _seededStorage.AddOrUpdate(Gens.DispatchedAt(now)
            .Sample(1, 1)
            .First());

        await _seededStorage.AddOrUpdate(Gens.DispatchedAt(now)
            .Sample(1, 1)
            .First()
            .TrySetFailed(Gens.MessageDeliveryFailureAt(now.AddSeconds(1))
                .Sample(1, 1)
                .First()));

        // Delivered too late (1)
        await _seededStorage.AddOrUpdate(Gens.DispatchedAt(now)
            .Sample(1, 1)
            .First()
            .TrySetReceived(now
                .AddSeconds(1)
                .AddMilliseconds(FairDelayPeriodMs)
                .AddMilliseconds(1)));
    }
}
