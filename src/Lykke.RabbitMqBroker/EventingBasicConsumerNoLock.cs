// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker
{
    public class EventingBasicConsumerNoLock : EventingBasicConsumer
    {
        public SharedConcurrentQueue<BasicDeliverEventArgs> Queue { get; private set; }
        
        private EventingBasicConsumerNoLock() : base(null)
        {
        }
        
        public EventingBasicConsumerNoLock(IModel model) : this(model, new SharedConcurrentQueue<BasicDeliverEventArgs>())
        {
            
        }
        
        public EventingBasicConsumerNoLock(IModel model, SharedConcurrentQueue<BasicDeliverEventArgs> queue) : base(model)
        {
            Queue = queue;
            Received += OnReceived;
        }

        private void OnReceived(object sender, BasicDeliverEventArgs e)
        {
            var bodyCopy = new byte[e.Body.Length];
            Buffer.BlockCopy(e.Body.ToArray(), 0, bodyCopy, 0, e.Body.Length);
            
            var eventArgs = new BasicDeliverEventArgs(e.ConsumerTag,
                e.DeliveryTag,
                e.Redelivered,
                e.Exchange,
                e.RoutingKey,
                e.BasicProperties,
                new ReadOnlyMemory<byte>(bodyCopy));
            
            Queue.Enqueue(eventArgs);
        }
    }
}
