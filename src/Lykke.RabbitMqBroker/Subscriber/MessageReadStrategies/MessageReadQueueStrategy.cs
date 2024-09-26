// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
    public class MessageReadQueueStrategy : IMessageReadStrategy
    {
        public QueueName Configure(RabbitMqSubscriptionSettings settings, Func<IModel> channelFactory)
        {
            return QueueName.Create(settings.QueueName);
        }
    }
}
