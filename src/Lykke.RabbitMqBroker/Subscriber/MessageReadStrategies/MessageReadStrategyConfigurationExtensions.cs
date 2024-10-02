// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class MessageReadStrategyConfigurationExtensions
{
    /// <summary>
    /// Attempt to configure a queue together with its DLX and poison
    /// using provided configuration options
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IConfigurationResult<QueueName> StrategyTryConfigure(this Func<IModel> channelFactory, QueueConfigurationOptions options)
    {
        var queueConfigurationResult = QueueConfigurator.Configure(channelFactory, options);

        if (queueConfigurationResult.IsSuccess && ExchangeConfigurator.DlxApplicable(options))
        {
            var dlxConfigurationResult = ExchangeConfigurator.ConfigureDlx(channelFactory, options).Match(
                onSuccess: () => QueueConfigurator.ConfigurePoison(channelFactory, options));

            return dlxConfigurationResult.IsSuccess
                ? queueConfigurationResult
                : ConfigurationResult<QueueName>.Failure(dlxConfigurationResult.Error);
        }

        return queueConfigurationResult;
    }

    /// <summary>
    /// Drop the queue and attempt to configure strategy again
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <param name="dropQueueName">Queue to drop before retry</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    /// Since the whole strategy configuration includes original 
    /// and poison queues configuration in case of failure we need 
    /// to know which queue exactly failed to be configured so 
    /// that we can drop it and retry.
    /// </remarks>
    public static IConfigurationResult<QueueName> StrategyRetryWithQueueRecreation(
        this Func<IModel> channelFactory,
        QueueConfigurationOptions options,
        QueueName dropQueueName)
    {
        return channelFactory.SafeDeleteQueue(dropQueueName.ToString()).Match(
            onSuccess: _ => StrategyTryConfigure(channelFactory, options),
            onFailure: _ => throw new InvalidOperationException($"Failed to delete queue [{options.QueueName}]."));
    }
}
