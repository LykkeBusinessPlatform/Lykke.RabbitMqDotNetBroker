using System;

namespace Lykke.RabbitMqBroker
{

    public record ListenerRegistration<TModel>(
        string ExchangeName,
        string QueueName,
        string RoutingKey) : IListenerRegistration
    {
        public string MessageRoute => ToString();

        public static ListenerRegistration<TModel> Create(
            string exchangeName,
            string queueName,
            string routingKey = null)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
                throw new ArgumentException("Exchange name is required", nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("Queue name is required", nameof(queueName));

            return new(exchangeName, queueName, routingKey);
        }

        public override string ToString() => RoutingKey switch
        {
            "" or null => $"[Exchange: {ExchangeName}] -> {typeof(TModel).Name} -> [Queue: {QueueName}]",
            _ => $"[Exchange: {ExchangeName}] -> {typeof(TModel).Name} -> [Queue: {QueueName}] by {RoutingKey}"
        };
    }
}
