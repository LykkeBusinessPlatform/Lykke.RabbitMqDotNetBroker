// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker
{
    public class QueueingBasicConsumerNoLock : DefaultBasicConsumer
    {
        public QueueingBasicConsumerNoLock() : this(null)
        {
            
        }

        public QueueingBasicConsumerNoLock(IModel model) : this(model, new SharedConcurrentQueue<BasicDeliverEventArgs>())
        {
            
        }

        public QueueingBasicConsumerNoLock(IModel model, SharedConcurrentQueue<BasicDeliverEventArgs> queue) : base(model)
        {
            Queue = queue;
        }
        
        public SharedConcurrentQueue<BasicDeliverEventArgs> Queue { get; private set; }

        public override void HandleBasicDeliver(string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            ReadOnlyMemory<byte> body)
        {
            var bodyCopy = new byte[body.Length];
            Buffer.BlockCopy(body.ToArray(), 0, bodyCopy, 0, body.Length);
            
            var eventArgs = new BasicDeliverEventArgs(consumerTag,
                deliveryTag,
                redelivered,
                exchange,
                routingKey,
                properties,
                new ReadOnlyMemory<byte>(bodyCopy));
            
            Queue.Enqueue(eventArgs);
        }
    }
}
