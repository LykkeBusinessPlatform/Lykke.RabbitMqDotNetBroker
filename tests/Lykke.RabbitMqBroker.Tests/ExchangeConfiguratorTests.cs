using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
internal sealed class ExchangeConfiguratorTests
{
    [Test]
    public void Configure_DeclaresExchange()
    {
        var options = new ExchangeConfigurationOptions(
            ExchangeName.Create("x"),
            ExchangeType: "direct",
            Durable: true,
            AutoDelete: false
        );

        var result = ExchangeConfigurator.Configure(() => new ConfiguratorFakeChannel(), options);

        Assert.That(result.IsSuccess);
        Assert.That(ConfiguratorFakeChannel.DeclaredExchanges, Does.Contain(options.ExchangeName.ToString()));
    }

    [Test]
    public void ConfigureDlx_DeclaresDlxExchange()
    {
        var originalQueueOptions = new QueueConfigurationOptions(
            QueueName.Create("q"),
            ExchangeName.Create("x"),
            DeadLetterExchangeName: DeadLetterExchangeName.Create("dlx"),
            RoutingKey: RoutingKey.Empty
        );

        var result = ExchangeConfigurator.ConfigureDlx(() => new ConfiguratorFakeChannel(), originalQueueOptions);

        Assert.That(result.IsSuccess);
        Assert.That(ConfiguratorFakeChannel.DeclaredExchanges, Does.Contain(originalQueueOptions.DeadLetterExchangeName.ToString()));
    }
}