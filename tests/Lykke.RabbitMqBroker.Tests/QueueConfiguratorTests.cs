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

        Assert.AreEqual(options.QueueName, result.QueueName);
        CollectionAssert.DoesNotContain(channel.DeclaredExchanges, options.ExchangeName);
        CollectionAssert.Contains(channel.DeclaredQueues, result.QueueName);
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

        CollectionAssert.Contains(channel.DeclaredExchanges, options.DeadLetterExchangeName);
        CollectionAssert.Contains(channel.DeclaredQueues, options.QueueName.GetPoisonQueueName());
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

        CollectionAssert.DoesNotContain(channel.DeclaredExchanges, options.DeadLetterExchangeName);
        CollectionAssert.DoesNotContain(channel.DeclaredQueues, options.QueueName.GetPoisonQueueName());
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
        Assert.AreEqual("dlx", args?["x-dead-letter-exchange"]);
    }
}
