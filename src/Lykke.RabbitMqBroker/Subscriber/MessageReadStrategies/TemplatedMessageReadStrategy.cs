// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

public abstract class TemplatedMessageReadStrategy : IMessageReadStrategy
{
    private readonly string _routingKey;
        
    protected bool Durable { get; init; }
    protected bool AutoDelete { get; init; }

    protected TemplatedMessageReadStrategy(string routingKey = "")
    {
        _routingKey = routingKey ?? string.Empty;
    }

    public string Configure(RabbitMqSubscriptionSettings settings, IModel channel)
    {
        if (settings.ShouldConfigureDeadLetter())
            ConfigureDeadLetter(settings, channel);
            
        var queueName = settings.GetQueueName();
        var args = settings.CreateArguments();
        channel.QueueDeclare(queueName, durable: Durable, exclusive: false, autoDelete: AutoDelete, arguments: args);

        var effectiveRoutingKey = string.IsNullOrWhiteSpace(_routingKey)
            ? settings.RoutingKey ?? string.Empty
            : _routingKey;
        channel.QueueBind(queueName, settings.ExchangeName, effectiveRoutingKey);

        return queueName;
    }

    private void ConfigureDeadLetter(RabbitMqSubscriptionSettings settings, IModel channel)
    {
        var queueName = settings.GetPoisonQueueName();
        channel.ExchangeDeclare(settings.DeadLetterExchangeName, "direct", durable: true);
        channel.QueueDeclare(queueName, durable: Durable, exclusive: false, autoDelete: AutoDelete);
        channel.QueueBind(queueName, settings.DeadLetterExchangeName, settings.RoutingKey ?? string.Empty);
    }
}
