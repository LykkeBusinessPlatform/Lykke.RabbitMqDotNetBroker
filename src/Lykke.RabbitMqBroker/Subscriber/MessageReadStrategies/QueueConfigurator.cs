using System;

using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class QueueConfigurator
{
    /// <summary>
    /// Declares and binds a queue
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IConfigurationResult<QueueName> Configure(
        Func<IModel> channelFactory,
        QueueConfigurationOptions options)
    {
        var args = options.BuildArguments();
        var logger = LoggerFactoryContainer.Instance.CreateLogger(nameof(QueueConfigurator));
        return channelFactory.DeclareQueue(options, args)
            .Match(
                // Even if the declared queue name is defined (from options),
                // strictly speaking, declaration operation returns the declared 
                // queue name, so it should to be honored here
                success => channelFactory.BindQueue(options with { QueueName = QueueName.Create(success.QueueName) })
                    .Match(ConfigurationResult<QueueName>.Success,
                        error => {
                            logger.LogWarning($"Failure during binding of the queue {options.QueueName} to exchange {options.ExistingExchangeName} because of {error.Code}:{error.Message}");
                            return ConfigurationResult<QueueName>.Failure(error);
                        }),
                error =>
                {
                    logger.LogWarning($"Failure during declaration of the queue {options.QueueName} because of {error.Code}:{error.Message}");
                    return ConfigurationResult<QueueName>.Failure(error);
                });
    }

    /// <summary>
    /// Declares and binds a poison queue based on the original queue options
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="originalQueueOptions"></param>
    /// <returns></returns>
    public static IConfigurationResult<PoisonQueueName> ConfigurePoison(
        Func<IModel> channelFactory,
        QueueConfigurationOptions originalQueueOptions)
    {
        var configurationOptions = new QueueConfigurationOptions(
            originalQueueOptions.QueueName.AsPoison(),
            originalQueueOptions.DeadLetterExchangeName,
            originalQueueOptions.Ttl.AsPoison(),
            Durable: true,
            AutoDelete: false,
            QueueType: originalQueueOptions.QueueType,
            RoutingKey: RoutingKey.Empty);

        return Configure(channelFactory, configurationOptions).Match(
            queueName => ConfigurationResult<PoisonQueueName>.Success(queueName.AsPoison()),
            e => ConfigurationResult<PoisonQueueName>.Failure(e));
    }
}
