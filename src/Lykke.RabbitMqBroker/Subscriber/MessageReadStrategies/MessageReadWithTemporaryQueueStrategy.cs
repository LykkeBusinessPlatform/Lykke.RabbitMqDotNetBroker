// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
    public class MessageReadWithTemporaryQueueStrategy : IMessageReadStrategy
    {
        private readonly string _routingKey;

        public MessageReadWithTemporaryQueueStrategy(string routingKey = "")
        {
            _routingKey = routingKey ?? string.Empty;
        }

        public QueueName Configure(RabbitMqSubscriptionSettings settings, Func<IModel> channelFactory)
        {
            using var channel = channelFactory();

            var queueName = settings.GetQueueName();
            var autodelete = !settings.IsDurable;
            IDictionary<string, object> args = null;

            if (!string.IsNullOrEmpty(settings.DeadLetterExchangeName))
            {
                var poisonQueueName = settings.GetQueueName().AsPoison();
                var deadLetterExchangeName = DeadLetterExchangeName.Create(settings.DeadLetterExchangeName);
                args = new QueueDeclarationArgumentsBuilder().AddDeadLetterExchange(deadLetterExchangeName).Build();
                channel.ExchangeDeclare(settings.DeadLetterExchangeName, "direct", durable: true);
                channel.QueueDeclare(poisonQueueName.ToString(), durable: settings.IsDurable, exclusive: false, autoDelete: false);
                channel.QueueBind(poisonQueueName.ToString(), settings.DeadLetterExchangeName, settings.RoutingKey ?? string.Empty);
            }

            settings.QueueName = channel.QueueDeclare(queueName.ToString(), durable: settings.IsDurable, exclusive: false, autoDelete: autodelete, arguments: args).QueueName;


            channel.QueueBind(
                queue: settings.QueueName,
                exchange: settings.ExchangeName,
                routingKey: string.IsNullOrWhiteSpace(_routingKey) ? settings.RoutingKey ?? string.Empty : _routingKey);

            return QueueName.Create(settings.QueueName);
        }

    }

}
