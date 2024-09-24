// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class MessageReadStrategyConfigurationExtensions
{
    public static IConfigurationResult<QueueName> StrategyTryConfigure(this Func<IModel> channelFactory, QueueConfigurationOptions options)
    {
        var queueConfigurationResult = QueueConfigurator.Configure(channelFactory, options);

        if (queueConfigurationResult.IsSuccess && options.DeadLetterExchangeName is not null)
        {
            var dlxConfigurationResult = ExchangeConfigurator.ConfigureDlx(channelFactory, options).Match(
                onSuccess: () => QueueConfigurator.ConfigurePoison(channelFactory, options));

            return dlxConfigurationResult.IsSuccess
                ? queueConfigurationResult
                : ConfigurationResult<QueueName>.Failure(dlxConfigurationResult.Error);
        }

        return queueConfigurationResult;
    }

    public static IConfigurationResult<QueueName> StrategyRetryWithQueueRecreation(this Func<IModel> channelFactory, QueueConfigurationOptions options)
    {
        return channelFactory.SafeDeleteQueue(options.QueueName.ToString()).Match(
            onSuccess: _ => StrategyTryConfigure(channelFactory, options),
            onFailure: _ => throw new InvalidOperationException($"Failed to delete queue [{options.QueueName}]."));
    }
}
