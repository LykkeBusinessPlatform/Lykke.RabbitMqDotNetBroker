﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

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

        public string Configure(RabbitMqSubscriptionSettings settings, IModel channel)
        {
            var queueName = settings.GetQueueName();
            var autodelete = !settings.IsDurable;
            IDictionary<string, object> args = null;
            
            if (!string.IsNullOrEmpty(settings.DeadLetterExchangeName))
            {
                var poisonQueueName = settings.GetPoisonQueueName();
                args = new QueueDeclarationArgumentsBuilder().WithDeadLetterExchange(settings.DeadLetterExchangeName).Build();
                channel.ExchangeDeclare(settings.DeadLetterExchangeName, "direct", durable: true);
                channel.QueueDeclare(poisonQueueName, durable: settings.IsDurable, exclusive: false, autoDelete: false);
                channel.QueueBind(poisonQueueName, settings.DeadLetterExchangeName, settings.RoutingKey ?? string.Empty);
            }

            settings.QueueName = channel.QueueDeclare(queueName, durable: settings.IsDurable, exclusive: false, autoDelete: autodelete, arguments: args).QueueName;


            channel.QueueBind(
                queue: settings.QueueName,
                exchange: settings.ExchangeName,
                routingKey: string.IsNullOrWhiteSpace(_routingKey) ? settings.RoutingKey ?? string.Empty : _routingKey);

            return settings.QueueName;
        }

    }

}
