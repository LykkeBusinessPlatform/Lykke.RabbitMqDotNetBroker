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
        private static class Const
        {
            public const bool Durable = false;
            public const bool AutoDelete = true;
            public const QueueType QType = QueueType.Classic;
        }

        public LossAcceptableMessageReadStrategy() : base()
        {
            Durable = Const.Durable;
            AutoDelete = Const.AutoDelete;
            QueueType = Const.QType;
        }

        public LossAcceptableMessageReadStrategy(string routingKey) : base(routingKey)
        {
            Durable = Const.Durable;
            AutoDelete = Const.AutoDelete;
            QueueType = Const.QType;
        }
    }
}
