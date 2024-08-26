using System.Runtime;

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
        var options = new QueueConfigurationOptions
        {
            ExchangeName = "x",
            QueueName = "q"
        };

        var channel = new QueueConfiguratorFakeChannel();
        var result = QueueConfigurator.Configure(channel, options);

        Assert.That(options.QueueName, Is.EqualTo(result.QueueName));
        Assert.That(channel.DeclaredExchanges, Does.Not.Contain(options.ExchangeName));
        Assert.That(channel.DeclaredQueues, Does.Contain(options.QueueName));
    }

    [Test]
    public void When_Dead_Letter_Exchange_Name_Is_Not_Empty_Should_Configure_Dead_Lettering()
    {
        var options = new QueueConfigurationOptions
        {
            ExchangeName = "x",
            DeadLetterExchangeName = "dlx",
            QueueName = "q"
        };

        var channel = new QueueConfiguratorFakeChannel();
        QueueConfigurator.Configure(channel, options);

        Assert.That(channel.DeclaredExchanges, Does.Contain(options.DeadLetterExchangeName));
        Assert.That(channel.DeclaredQueues, Does.Contain(options.QueueName.GetPoisonQueueName()));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public void When_Dead_Letter_Exchange_Name_Is_Empty_Should_Not_Configure_Dead_Lettering(string empty_dlx)
    {
        var options = new QueueConfigurationOptions
        {
            ExchangeName = "x",
            DeadLetterExchangeName = empty_dlx,
            QueueName = "q"
        };

        var channel = new QueueConfiguratorFakeChannel();
        QueueConfigurator.Configure(channel, options);

        Assert.That(channel.DeclaredExchanges, Does.Not.Contain(options.DeadLetterExchangeName));
        Assert.That(channel.DeclaredQueues, Does.Not.Contain(options.QueueName.GetPoisonQueueName()));
    }

    [Test]
    public void When_Dead_Letter_Exchange_Configured_Original_Queue_Should_Be_Declared_With_Dlx()
    {
        var options = new QueueConfigurationOptions
        {
            ExchangeName = "x",
            DeadLetterExchangeName = "dlx",
            QueueName = "q"
        };

        var channel = new QueueConfiguratorFakeChannel();
        var result = QueueConfigurator.Configure(channel, options);

        channel.DeclaredQueuesArguments.TryGetValue(result.QueueName, out var args);
        Assert.That("dlx", Is.EqualTo(args?["x-dead-letter-exchange"]));
    }
}
