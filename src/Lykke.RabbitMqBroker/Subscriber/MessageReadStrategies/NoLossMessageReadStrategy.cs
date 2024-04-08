// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies
{
    /// <summary>
    /// Message read strategy that does not lose messages.
    /// Ignores <see cref="RabbitMqSubscriptionSettings.IsDurable"/> value and always declares a durable queue.
    /// Additional server-side configuration is required to set up mirroring feature.
    /// </summary>
    public sealed class NoLossMessageReadStrategy : TemplatedMessageReadStrategy
    {
        public NoLossMessageReadStrategy(string routingKey = "") : base(routingKey)
        {
            Durable = true;
            AutoDelete = false;
        }
    }
}
