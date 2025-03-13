// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Threading;

using JetBrains.Annotations;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
    [PublicAPI]
    public interface IMessageReadStrategy
    {
        /// <summary>
        /// Configures the queue
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="channelFactory"></param>
        /// <returns> The name of the queue </returns>
        QueueName Configure(RabbitMqSubscriptionSettings settings, Func<IModel> channelFactory);
    }
}