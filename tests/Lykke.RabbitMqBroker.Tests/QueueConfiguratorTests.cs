using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using NUnit.Framework;

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
            RoutingKey: RoutingKey.Empty
        );

        var result = QueueConfigurator.Configure(() => new ConfiguratorFakeChannel(), options);

        Assert.That(options.QueueName, Is.EqualTo(result.Response));
        Assert.That(ConfiguratorFakeChannel.DeclaredExchanges, Does.Not.Contain(options.ExistingExchangeName));
        Assert.That(ConfiguratorFakeChannel.DeclaredQueues, Does.Contain(options.QueueName.ToString()));
    }

    [Test]
    public void When_Dead_Letter_Exchange_Configured_Original_Queue_Should_Be_Declared_With_Dlx()
    {
        var options = new QueueConfigurationOptions(
            QueueName.Create("q"),
            ExchangeName.Create("x"),
            DeadLetterExchangeName: DeadLetterExchangeName.Create("dlx"),
            RoutingKey: RoutingKey.Empty
        );

        var result = QueueConfigurator.Configure(() => new ConfiguratorFakeChannel(), options);

        ConfiguratorFakeChannel.DeclaredQueuesArguments.TryGetValue(result.Response.ToString(), out var args);
        Assert.That("dlx", Is.EqualTo(args?["x-dead-letter-exchange"]));
    }

    [TearDown]
    public void TearDown()
    {
        ConfiguratorFakeChannel.ResetCounters();
    }
}