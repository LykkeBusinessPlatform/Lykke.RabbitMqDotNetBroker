using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class ExchangeConfigurator
{
    /// <summary>
    /// Declares an exchange
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IConfigurationResult Configure(
        Func<IModel> channelFactory,
        ExchangeConfigurationOptions options)
    {
        return channelFactory.DeclareExchange(options);
    }

    /// <summary>
    /// Declares a dead-letter exchange based on the original queue options
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="originalQueueOptions"></param>
    /// <returns></returns>
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