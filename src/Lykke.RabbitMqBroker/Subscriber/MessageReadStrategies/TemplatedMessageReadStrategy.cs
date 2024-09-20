// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
public abstract class TemplatedMessageReadStrategy : IMessageReadStrategy
{
    private const string StrategyDefaultDeadLetterExchangeType = "direct";

    private readonly string _routingKey;

    protected bool Durable { get; init; }
    protected bool AutoDelete { get; init; }
    protected QueueType QueueType { get; init; }

    protected TemplatedMessageReadStrategy(string routingKey = "")
    {
        _routingKey = routingKey ?? string.Empty;
    }

    public QueueName Configure(RabbitMqSubscriptionSettings settings, Func<IModel> channelFactory)
    {
        var options = CreateQueueConfigurationOptions(settings);

        return TryConfigure(channelFactory, options).Match(
            queueName => queueName,
            _ => RetryWithQueueRecreation(channelFactory, options).Match(
                queueName => queueName,
                 _ => throw new InvalidOperationException($"Failed to configure queue [{options.QueueName}] after precondition failure")));
    }

    private static QueueConfigurationResult<QueueName> TryConfigure(Func<IModel> channelFactory, QueueConfigurationOptions options) =>
        QueueConfigurator.Configure(channelFactory, options);

    private static QueueConfigurationResult<QueueName> RetryWithQueueRecreation(Func<IModel> channelFactory, QueueConfigurationOptions options)
    {
        return channelFactory.SafeDeleteClassicQueue(options.QueueName.ToString()).Match(
            onSuccess: _ => TryConfigure(channelFactory, options),
            onFailure: _ => throw new InvalidOperationException($"Failed to delete queue [{options.QueueName}]."));
    }

    private QueueConfigurationOptions CreateQueueConfigurationOptions(RabbitMqSubscriptionSettings settings)
    {
        var durabilityFromStrategy = Durable;
        var autoDeleteFromStrategy = AutoDelete;
        var routingKeyFromStrategy = _routingKey;
        var queueType = QueueType;

        var effectiveRoutingKey = string.IsNullOrWhiteSpace(routingKeyFromStrategy)
            ? settings.RoutingKey ?? string.Empty
            : routingKeyFromStrategy;

        return new QueueConfigurationOptions
        {
            QueueName = settings.GetQueueName(),
            ExchangeName = settings.ExchangeName,
            DeadLetterExchangeName = settings.DeadLetterExchangeName,
            DeadLetterExchangeType = StrategyDefaultDeadLetterExchangeType,
            Durable = durabilityFromStrategy,
            AutoDelete = autoDeleteFromStrategy,
            RoutingKey = effectiveRoutingKey,
            QueueType = queueType
        };
    }
}
