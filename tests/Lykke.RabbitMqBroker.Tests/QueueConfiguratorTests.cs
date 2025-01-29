using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using NUnit.Framework;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
public class QueueConfiguratorTests
{
    [Test]
    public void Configurator_Should_Declare_Queue()
    {
        var options = new QueueConfigurationOptions(
            QueueName.Create("q"),
            ExchangeName.Create("x"),
            TimeToLive.Infinite,
            RoutingKey: RoutingKey.Empty
        );

        var result = QueueConfigurator.Configure(() => new PrimitivesConfiguratorFakeChannel(), options);

        Assert.Multiple(() =>
        {
            Assert.That(options.QueueName, Is.EqualTo(result.Response));
            Assert.That(PrimitivesConfiguratorFakeChannel.DeclaredExchanges, Does.Not.Contain(options.ExistingExchangeName.ToString()));
            Assert.That(PrimitivesConfiguratorFakeChannel.DeclaredQueues, Does.Contain(options.QueueName.ToString()));
        });
    }

    [Test]
    public void When_Dead_Letter_Exchange_Configured_Original_Queue_Should_Be_Declared_With_Dlx()
    {
        var options = new QueueConfigurationOptions(
            QueueName.Create("q"),
            ExchangeName.Create("x"),
            TimeToLive.Infinite,
            DeadLetterExchangeName.Create("dlx"),
            RoutingKey: RoutingKey.Empty
        );

        var result = QueueConfigurator.Configure(() => new PrimitivesConfiguratorFakeChannel(), options);

        PrimitivesConfiguratorFakeChannel.DeclaredQueuesArguments.TryGetValue(result.Response.ToString(), out var args);
        Assert.That(args?[Headers.XDeadLetterExchange], Is.EqualTo("dlx"));
    }

    [Test]
    public void ConfigurePoison_Ttl_Should_Be_Greater_Than_Original()
    {
        var originalQueueTtl = TimeToLive.OneMinute;
        var options = new QueueConfigurationOptions(
            QueueName.Create("q"),
            ExchangeName.Create("x"),
            originalQueueTtl,
            DeadLetterExchangeName.Create("dlx"),
            RoutingKey: RoutingKey.Empty
        );

        var result = QueueConfigurator.ConfigurePoison(() => new PrimitivesConfiguratorFakeChannel(), options);

        PrimitivesConfiguratorFakeChannel.DeclaredQueuesArguments.TryGetValue(result.Response.ToString(), out var args);
        Assert.That(options.QueueName.AsPoison(), Is.EqualTo(result.Response));
        Assert.That(
            (long)args[Headers.XExpires],
            Is.GreaterThan(originalQueueTtl.ToExpirationMilliseconds()));
    }

    [TearDown]
    public void TearDown()
    {
        PrimitivesConfiguratorFakeChannel.ResetCounters();
    }
}