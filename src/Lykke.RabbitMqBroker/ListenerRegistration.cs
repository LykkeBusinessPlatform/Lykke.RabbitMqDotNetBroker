using System;
using System.Collections.Generic;

namespace Lykke.RabbitMqBroker
{

    public sealed class ListenerRegistration<TModel> : IListenerRegistration, IEquatable<ListenerRegistration<TModel>>
    {
        public string ExchangeName { get; }
        public string QueueName { get; }
        public string RoutingKey { get; }

        public ListenerRegistration(string exchangeName, string queueName, string routingKey = "")
        {
            if (string.IsNullOrWhiteSpace(exchangeName)) throw new ArgumentException("Exchange name is required", nameof(exchangeName));
            if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException("Queue name is required", nameof(queueName));

            ExchangeName = exchangeName;
            QueueName = queueName;
            RoutingKey = routingKey ?? string.Empty;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ListenerRegistration<TModel>);
        }

        public bool Equals(ListenerRegistration<TModel> other)
        {
            return other != null &&
                   ExchangeName == other.ExchangeName &&
                   QueueName == other.QueueName &&
                   RoutingKey == other.RoutingKey;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExchangeName, QueueName, RoutingKey, typeof(TModel));
        }

        public static bool operator ==(ListenerRegistration<TModel> left, ListenerRegistration<TModel> right)
        {
            return EqualityComparer<ListenerRegistration<TModel>>.Default.Equals(left, right);
        }

        public static bool operator !=(ListenerRegistration<TModel> left, ListenerRegistration<TModel> right)
        {
            return !(left == right);
        }
    }
}
