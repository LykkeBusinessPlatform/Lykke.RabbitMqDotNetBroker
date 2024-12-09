using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using NUnit.Framework;

using RabbitMQ.Client;

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

        Assert.That(args, Does.ContainKey(Headers.XQueueType));
        Assert.That(args[Headers.XQueueType], Is.EqualTo("quorum"));
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

        Assert.That(args, Does.ContainKey(Headers.XDeadLetterExchange));
        Assert.That(args[Headers.XDeadLetterExchange], Is.EqualTo(deadLetterExchangeName.ToString()));
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
        Assert.That(args, Does.ContainKey(Headers.XOverflow));
        Assert.That(args[Headers.XOverflow], Is.EqualTo("reject-publish"));
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

        args.TryGetValue(Headers.XOverflow, out var overflow);
        Assert.That(overflow, Is.AnyOf(null, "drop-head"));
    }

    [Test]
    public void BuildArguments_WhenTtlIsNotDefault_AddsExpirationArgument()
    {
        var options = new QueueConfigurationOptions(
            QueueName.Create("queue"),
            ExchangeName.Create("exchange"),
            TimeToLive.OneMinute
        );

        var args = options.BuildArguments();

        Assert.That(args, Does.ContainKey(Headers.XExpires));
        Assert.That(
            args[Headers.XExpires],
            Is.EqualTo(TimeToLive.OneMinute.ToExpirationMilliseconds()));
    }

    [Test]
    public void ForClassicQueue_WhenAutoDeleteIsTrue_SetsTtlToInfinite()
    {
        var options = QueueConfigurationOptions.ForClassicQueue(
            QueueName.Create("queue"),
            ExchangeName.Create("exchange"),
            null,
            string.Empty,
            false,
            true,
            RoutingKey.Empty,
            TimeToLive.OneHour
        );

        Assert.That(options.Ttl, Is.EqualTo(TimeToLive.Infinite));
    }
}