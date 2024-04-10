// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using JetBrains.Annotations;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
    [PublicAPI]
    public interface IMessageReadStrategy
    {
        /// <summary>
        /// Configures the channel for the message reading
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="channel"></param>
        /// <returns>
        /// The name of the queue that was created
        /// </returns>
        string Configure(RabbitMqSubscriptionSettings settings, IModel channel);
    }
}
