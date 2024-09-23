using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class ExchangeConfigurator
{
    public static IConfigurationResult Configure(
        Func<IModel> channelFactory,
        ExchangeConfigurationOptions options)
    {
        return channelFactory.DeclareExchange(options);
    }

    public static IConfigurationResult ConfigureDlx(
        Func<IModel> channelFactory,
        QueueConfigurationOptions originalQueueOptions)
    {
        var configurationOptions = new ExchangeConfigurationOptions(
            originalQueueOptions.DeadLetterExchangeName,
            originalQueueOptions.DeadLetterExchangeType,
            Durable: true,
            AutoDelete: false);

        return Configure(channelFactory, configurationOptions);
    }
}