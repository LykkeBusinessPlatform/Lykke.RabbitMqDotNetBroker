using JetBrains.Annotations;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class QueueConfigurationOptionsTests
{
    [Test]
    public void BuildArguments_WhenQueueTypeIsQuorum_AddsQuorumQueueArgument()
    {
        var options = new QueueConfigurationOptions(
            QueueName.Create("queue"),
            ExchangeName.Create("exchange"),
            TimeToLive.Infinite,
            QueueType: QueueType.Quorum
        );

        var args = options.BuildArguments();

        Assert.That(args, Does.ContainKey("x-queue-type"));
        Assert.That(args["x-queue-type"], Is.EqualTo("quorum"));
    }

    [Test]
    public void BuildArguments_WhenDeadLetterExchangeNameIsNotNull_AddsDeadLetterExchangeArgument()
    {
        var deadLetterExchangeName = DeadLetterExchangeName.Create("dlx");
        var options = new QueueConfigurationOptions(
            QueueName.Create("queue"),
            ExchangeName.Create("exchange"),
            TimeToLive.Infinite,
            deadLetterExchangeName
        );

        var args = options.BuildArguments();

        Assert.That(args, Does.ContainKey("x-dead-letter-exchange"));
        Assert.That(args["x-dead-letter-exchange"], Is.EqualTo(deadLetterExchangeName.ToString()));
    }

    [Test]
    public void BuildArguments_WhenDeadLetterExchangeNameIsNotNull_And_QuorumQueue_AddsAtLeastOnceStrategy()
    {
        var options = new QueueConfigurationOptions(
            QueueName.Create("queue"),
            ExchangeName.Create("exchange"),
            TimeToLive.Infinite,
            DeadLetterExchangeName.Create("dlx"),
            QueueType: QueueType.Quorum
        );

        var args = options.BuildArguments();

        Assert.That(args, Does.ContainKey("x-dead-letter-strategy"));
        Assert.That(args["x-dead-letter-strategy"], Is.EqualTo("at-least-once"));
        Assert.That(args, Does.ContainKey("overflow"));
        Assert.That(args["overflow"], Is.EqualTo("reject-publish"));
    }

    [Test]
    public void BuildArguments_WhenDeadLetterExchangeNameIsNotNull_And_ClassicQueue_AddsAtMostOnceStrategy()
    {
        // at-most-once is the default one
        // so we either expect the key to be removed or the value to be "at-most-once"

        var options = new QueueConfigurationOptions(
            QueueName.Create("queue"),
            ExchangeName.Create("exchange"),
            TimeToLive.Infinite,
            DeadLetterExchangeName.Create("dlx"),
            QueueType: QueueType.Classic
        );

        var args = options.BuildArguments();

        args.TryGetValue("x-dead-letter-strategy", out var strategy);
        Assert.That(strategy, Is.AnyOf(null, "at-most-once"));

        args.TryGetValue("overflow", out var overflow);
        Assert.That(overflow, Is.AnyOf(null, "drop-head"));
    }
}