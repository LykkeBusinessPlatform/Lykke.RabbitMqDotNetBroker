using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class DealLetterExchangeArgumentsBuilderTests
{
    [Test]
    public void WithAtLeastOnceStrategy_AddsAtLeastOnceStrategyArguments()
    {
        var argsBuilder = new QueueDeclarationArgumentsBuilder()
            .AddDeadLetterExchange(DeadLetterExchangeName.Create("exchange"))
            .WithAtLeastOnceStrategy();

        var args = argsBuilder.Build();

        Assert.That(args, Does.ContainKey("x-dead-letter-strategy"));
        Assert.That(args["x-dead-letter-strategy"], Is.EqualTo("at-least-once"));

        Assert.That(args, Does.ContainKey(Headers.XOverflow));
        Assert.That(args[Headers.XOverflow], Is.EqualTo("reject-publish"));
    }

    [Test]
    public void WithAtMostOnceStrategy_RemovesAtLeastOnceStrategyArguments()
    {
        var argsBuilder = new QueueDeclarationArgumentsBuilder()
            .AddDeadLetterExchange(DeadLetterExchangeName.Create("exchange"))
            .WithAtMostOnceStrategy();

        var args = argsBuilder.Build();

        Assert.That(args, Does.Not.ContainKey("x-dead-letter-strategy")); // using default
        Assert.That(args, Does.Not.ContainKey(Headers.XOverflow)); // using default
    }
}