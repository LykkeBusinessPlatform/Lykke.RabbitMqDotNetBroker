// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

public abstract class TemplatedMessageReadStrategy : IMessageReadStrategy
{
    private const string StrategyDefaultDeadLetterExchangeType = "direct";
    
    private readonly string _routingKey;
        
    protected bool Durable { get; init; }
    protected bool AutoDelete { get; init; }

    protected TemplatedMessageReadStrategy(string routingKey = "")
    {
        _routingKey = routingKey ?? string.Empty;
    }

    public string Configure(RabbitMqSubscriptionSettings settings, IModel channel)
    {
        var queueConfigurationResult = QueueConfigurator.Configure(
            channel, 
            CreateQueueConfigurationOptions(settings));
        
        return queueConfigurationResult.QueueName;
    }
    
    private QueueConfigurationOptions CreateQueueConfigurationOptions(RabbitMqSubscriptionSettings settings)
    {
        var durabilityFromStrategy = Durable;
        var autoDeleteFromStrategy = AutoDelete;
        var routingKeyFromStrategy = _routingKey;
        
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
            RoutingKey = effectiveRoutingKey
        };
    }
}
