// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
public abstract class TemplatedMessageReadStrategy : IMessageReadStrategy
{
    private const string StrategyDefaultDeadLetterExchangeType = "direct";

    private readonly RoutingKey _routingKey;

    protected bool Durable { get; init; }
    protected bool AutoDelete { get; init; }
    protected QueueType QueueType { get; init; }

    protected TemplatedMessageReadStrategy(RoutingKey routingKey)
    {
        _routingKey = routingKey;
    }

    public QueueName Configure(RabbitMqSubscriptionSettings settings, Func<IModel> channelFactory)
    {
        var options = CreateQueueConfigurationOptions(settings);

        var (queueName, _) = channelFactory.StrategyTryConfigure(options).Match(
            onFailure: _ => channelFactory.StrategyRetryWithQueueRecreation(options).Match(
                 onFailure: _ => throw new InvalidOperationException($"Failed to configure queue [{options.QueueName}] after precondition failure")));

        return queueName;
    }

    private QueueConfigurationOptions CreateQueueConfigurationOptions(RabbitMqSubscriptionSettings settings)
    {
        var effectiveRoutingKey = _routingKey == RoutingKey.Empty
            ? RoutingKey.Create(settings.RoutingKey)
            : _routingKey;

        return QueueType switch
        {
            QueueType.Classic => QueueConfigurationOptions.ForClassicQueue(
                settings.GetQueueName(),
                ExchangeName.Create(settings.ExchangeName),
                string.IsNullOrWhiteSpace(settings.DeadLetterExchangeName) ? null : DeadLetterExchangeName.Create(settings.DeadLetterExchangeName),
                StrategyDefaultDeadLetterExchangeType,
                Durable,
                AutoDelete,
                effectiveRoutingKey),
            QueueType.Quorum => QueueConfigurationOptions.ForQuorumQueue(
                settings.GetQueueName(),
                ExchangeName.Create(settings.ExchangeName),
                string.IsNullOrWhiteSpace(settings.DeadLetterExchangeName) ? null : DeadLetterExchangeName.Create(settings.DeadLetterExchangeName),
                StrategyDefaultDeadLetterExchangeType,
                effectiveRoutingKey),
            _ => throw new InvalidOperationException($"Unsupported queue type [{QueueType}]")
        };
    }
}
