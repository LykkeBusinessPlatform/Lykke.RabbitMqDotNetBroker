using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class StrategyConfigurator
{
    /// <summary>
    /// Configures the whole strategy including queue, DLX and poison queue
    /// according to the provided options.
    /// In case there is a configuration mismatch with existing queue or
    /// poison queue, the method will attempt to safe delete the queue 
    /// in question and retry the configuration.
    /// Safe deletion means that the queue will be deleted only if it's
    /// empty and has no consumers.
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IConfigurationResult<QueueName> Configure(
        Func<IModel> channelFactory,
        QueueConfigurationOptions options)
    {
        var result = QueueConfigurator.Configure(channelFactory, options);
        if (result.IsFailure)
        {
            return channelFactory.SafeDeleteQueue(options.QueueName.ToString()).Match(
                onSuccess: _ => Configure(channelFactory, options),
                onFailure: _ => throw new InvalidOperationException($"Failed to delete queue [{options.QueueName}].")
            );
        }

        if (ExchangeConfigurator.DlxApplicable(options))
        {
            var dlxConfigurationResult = ExchangeConfigurator.ConfigureDlx(channelFactory, options).Match(
                onSuccess: () => QueueConfigurator.ConfigurePoison(channelFactory, options).Match(
                    onFailure: _ => channelFactory.SafeDeleteQueue(options.QueueName.AsPoison().ToString()).Match(
                        onSuccess: _ => QueueConfigurator.ConfigurePoison(channelFactory, options),
                        onFailure: _ => throw new InvalidOperationException($"Failed to delete queue [{options.QueueName.AsPoison()}]."))));

            return dlxConfigurationResult.IsSuccess
                ? result
                : ConfigurationResult<QueueName>.Failure(dlxConfigurationResult.Error);
        }

        return result;
    }
}
