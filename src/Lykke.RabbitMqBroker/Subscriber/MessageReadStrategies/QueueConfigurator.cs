using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class QueueConfigurator
{
    public static IConfigurationResult<QueueName> Configure(
        Func<IModel> channelFactory,
        QueueConfigurationOptions options)
    {
        var args = options.BuildArguments();
        return channelFactory.DeclareQueue(options, args)
            .Match(
                // Even if the declared queue name is defined (from options),
                // strictly speaking, declaration operation returns the declared 
                // queue name, so it should to be honored here
                success => channelFactory.BindQueue(options with { QueueName = QueueName.Create(success.QueueName) }),
                ConfigurationResult<QueueName>.Failure);
    }

    public static IConfigurationResult<PoisonQueueName> ConfigurePoison(
        Func<IModel> channelFactory,
        QueueConfigurationOptions originalQueueOptions)
    {
        var configurationOptions = new QueueConfigurationOptions(
            originalQueueOptions.QueueName.AsPoison(),
            originalQueueOptions.DeadLetterExchangeName,
            Durable: true,
            AutoDelete: false,
            QueueType: originalQueueOptions.QueueType,
            RoutingKey: RoutingKey.Empty);

        return Configure(channelFactory, configurationOptions).Match(
            queueName => ConfigurationResult<PoisonQueueName>.Success(queueName.AsPoison()),
            e => ConfigurationResult<PoisonQueueName>.Failure(e));
    }
}
