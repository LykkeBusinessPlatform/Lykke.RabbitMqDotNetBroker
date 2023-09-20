// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.IO;

namespace Lykke.RabbitMqBroker
{
    public class SharedConcurrentQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        
        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
        }
        
        public bool Dequeue(out T item)
        {
            return _queue.TryDequeue(out item);
        }
    }
}
