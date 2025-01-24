using System;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Time.Testing;

using NUnit.Framework;

using static Lykke.RabbitMqBroker.MessageDeliveryAnalysisExtensions.MessageDeliveryAnalysisVerdict;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class MessageDeliveryAnalysisTests
{
    private static readonly TimeSpan FairDelayPeriod = TimeSpan.FromSeconds(10);

    [Test]
    public void Analyze_FairDelayNotExpired_NotDelivered_Returns_Verdict_NotDeliveredYet()
    {
        var message = new MessageDeliveryWithDefaults().TrySetDispatched(DateTime.UtcNow);

        var result = message.Analyze(
            FairDelayPeriod,
            new FakeTimeProvider(DateTime.UtcNow));

        Assert.That(result, Is.EqualTo(NotDeliveredYet));
    }

    [Test]
    public void Analyze_FairDelayNotExpired_Delivered_Returns_Verdict_DeliveredOnTime()
    {
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetReceived(DateTime.UtcNow.AddSeconds(FairDelayPeriod.TotalSeconds - 1));

        var result = message.Analyze(
            FairDelayPeriod,
            new FakeTimeProvider(DateTime.UtcNow));

        Assert.That(result, Is.EqualTo(DeliveredOnTime));
    }

    [Test]
    public void Analyze_FairDelayExpired_NotDelivered_Returns_Verdict_NotDelivered()
    {
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow);

        var result = message.Analyze(
            FairDelayPeriod,
            new FakeTimeProvider(DateTime.UtcNow.AddSeconds(FairDelayPeriod.TotalSeconds + 1)));

        Assert.That(result, Is.EqualTo(NotDelivered));
    }

    [Test]
    public void Analyze_FairDelayExpired_Delivered_Returns_Verdict_LatelyDelivered()
    {
        var message = new MessageDeliveryWithDefaults()
            .TrySetDispatched(DateTime.UtcNow)
            .TrySetReceived(DateTime.UtcNow.AddSeconds(FairDelayPeriod.TotalSeconds + 1));

        var result = message.Analyze(
            FairDelayPeriod,
            new FakeTimeProvider(DateTime.UtcNow.AddSeconds(FairDelayPeriod.TotalSeconds + 2)));

        Assert.That(result, Is.EqualTo(LatelyDelivered));
    }
}
