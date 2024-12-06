using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class QueueDeclarationArgumentsBuilderTests
{
    [Test]
    public void UseQuorumQueue_AddsQuorumQueueArgument()
    {
        var argsBuilder = new QueueDeclarationArgumentsBuilder();

        argsBuilder.UseQuorumQueue();

        var args = argsBuilder.Build();

        Assert.That(args, Does.ContainKey(Headers.XQueueType));
        Assert.That(args[Headers.XQueueType], Is.EqualTo("quorum"));
    }

    [Test]
    public void UseClassicQueue_RemovesQuorumQueueArgument()
    {
        var argsBuilder = new QueueDeclarationArgumentsBuilder();

        argsBuilder
            .UseQuorumQueue()
            .UseClassicQueue();

        var args = argsBuilder.Build();

        Assert.That(args, Is.Null);
    }

    [Test]
    public void AddDeadLetterExchange_AddsDeadLetterExchangeArgument()
    {
        var argsBuilder = new QueueDeclarationArgumentsBuilder();

        var dlxExchangeName = DeadLetterExchangeName.Create("exchange");
        argsBuilder.AddDeadLetterExchange(dlxExchangeName);

        var args = argsBuilder.Build();

        Assert.That(args, Does.ContainKey(Headers.XDeadLetterExchange));
        Assert.That(args[Headers.XDeadLetterExchange], Is.EqualTo(dlxExchangeName.ToString()));
    }
}
