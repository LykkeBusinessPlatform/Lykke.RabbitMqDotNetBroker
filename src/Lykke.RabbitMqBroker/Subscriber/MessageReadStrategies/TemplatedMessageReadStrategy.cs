// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
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
            var queue = settings.GetQueueName();
            channel.QueueDeclare(queue, durable: Durable, exclusive: false, autoDelete: AutoDelete);

            var effectiveRoutingKey = string.IsNullOrWhiteSpace(_routingKey)
                ? settings.RoutingKey ?? string.Empty
                : _routingKey;
            channel.QueueBind(queue, settings.ExchangeName, effectiveRoutingKey);

            return queue;
        }
    }
}
