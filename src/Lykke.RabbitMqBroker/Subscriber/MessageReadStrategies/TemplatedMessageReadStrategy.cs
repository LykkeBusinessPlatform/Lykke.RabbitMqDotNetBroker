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
        var queueType = QueueType;

        var effectiveRoutingKey = _routingKey == RoutingKey.Empty
            ? RoutingKey.Create(settings.RoutingKey)
            : _routingKey;

        return new QueueConfigurationOptions
        {
            QueueName = settings.GetQueueName(),
            ExchangeName = ExchangeName.Create(settings.ExchangeName),
            DeadLetterExchangeName = DeadLetterExchangeName.Create(settings.DeadLetterExchangeName),
            DeadLetterExchangeType = StrategyDefaultDeadLetterExchangeType,
            Durable = durabilityFromStrategy,
            AutoDelete = autoDeleteFromStrategy,
            RoutingKey = effectiveRoutingKey,
            QueueType = queueType
        };
    }
}
