// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
    /// <summary>
    /// Message read strategy that allows message loss.
    /// Ignores <see cref="RabbitMqSubscriptionSettings.IsDurable"/> value and always declares a non-durable queue.
    /// </summary>
    public sealed class LossAcceptableMessageReadStrategy : TemplatedMessageReadStrategy
    {
        public LossAcceptableMessageReadStrategy(string routingKey = "") : base(routingKey)
        {
            Durable = false;
            AutoDelete = true;
        }
    }
}
