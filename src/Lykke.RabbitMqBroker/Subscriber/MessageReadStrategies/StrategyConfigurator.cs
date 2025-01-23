using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class StrategyConfigurator
{
    private const int ConfigurationTriesMaxCount = 300;
    private const int ConfigurationTriesDelayMs = 3000;

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
    /// <param name="cancellationToken"></param>
    /// <param name="tryNumber">Technical trick for retry organization</param>
    /// <param name="timeoutMs">Technical trick for retry organization</param>
    /// <param name="tryCount">Technical trick for retry organization</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IConfigurationResult<QueueName> Configure(
        Func<IModel> channelFactory,
        QueueConfigurationOptions options,
        CancellationToken cancellationToken = default,
        int tryNumber = 1,
        int timeoutMs = ConfigurationTriesDelayMs,
        int tryCount = ConfigurationTriesMaxCount)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ConfigurationResult<QueueName>.Failure(
                new ConfigurationError(ConfigurationErrorCode.Cancelled, "Operation canceled."));
        }

        var result = QueueConfigurator.Configure(channelFactory, options);

        if (result.IsFailure)
        {
            // it could be either failure of declaration, or failure of binding
            // failure of declaration must be fixed by recreation, but that works safely only for classic queues: https://github.com/rabbitmq/rabbitmq-server/issues/10543
            if (result.Error.Code.Code.Equals(Constants.PreconditionFailed))
                switch (options.QueueType)
                {
                    // NOTE: case with migration from classic queue to quorum WILL fail. however, it is not considered as real need
                    case QueueType.Classic:
                        return channelFactory.SafeDeleteQueue(options.QueueName.ToString()).Match(
                            onSuccess: _ => Configure(channelFactory, options, cancellationToken, tryNumber),
                            onFailure: _ => throw new InvalidOperationException($"Failed to delete queue: precondition failed for queue '[{options.QueueName}]'.")
                        );
                    case QueueType.Quorum:
                        {
                            // run declare on options shallow copy with a different queue type to catch migration situation from classic to quorum
                            var optionsForRegularQueue = options with { QueueType = QueueType.Classic };
                            if (channelFactory.DeclareQueue(options, optionsForRegularQueue.BuildArguments()).IsSuccess)
                            {
                                return channelFactory.SafeDeleteQueue(options.QueueName.ToString()).Match(
                                    onSuccess: _ => Configure(channelFactory, options, cancellationToken, tryNumber, timeoutMs, tryCount),
                                    onFailure: _ => throw new InvalidOperationException($"Failed to delete queue: precondition failed for queue '[{options.QueueName}]'.")
                                );
                            }

                            // if other parameter was changed, then we can't safely recreate the queue
                            break;
                        }
                }

            // Otherwise we just keep retrying hoping service operator will notice this and will deal with the queue manually
            var logger = LoggerFactoryContainer.Instance.CreateLogger(nameof(StrategyConfigurator));

            if (tryNumber++ < tryCount)
            {
                logger.LogWarning($"Queue `{options.QueueName}` declaration OR binding has failed. Reason: {result.Error.Message}. Code: {result.Error.Code}." + Environment.NewLine +
                                  $"Try #{tryNumber} out of {tryCount} total is coming in {timeoutMs} milliseconds." + Environment.NewLine +
                                  $"Please make sure that " + Environment.NewLine +
                                  $"- queue parameters in service configuration matches queue real state from RabbitMQ. If is an intentional upgrade, workaround is to delete the queue manually." + Environment.NewLine +
                                  $"- exchange {options.ExistingExchangeName} exists. Check responsible service.");

                if (cancellationToken.WaitHandle.WaitOne(timeoutMs))
                {
                    return ConfigurationResult<QueueName>.Failure(
                        new ConfigurationError(ConfigurationErrorCode.Cancelled, "Operation canceled during retry."));
                }

                return Configure(channelFactory, options, cancellationToken, tryNumber, timeoutMs, tryCount);
            }

            logger.LogCritical($"Configuration of the queue {options.QueueName} failed after {tryNumber} attempts. Please fix problems from above and restart the service.");

            return ConfigurationResult<QueueName>.Failure(result.Error);
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