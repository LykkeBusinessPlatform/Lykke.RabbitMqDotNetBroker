// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.IO;

namespace Lykke.RabbitMqBroker
{
    public class SharedConcurrentQueue<T>
    {
        private bool _isOpen = true;
        private ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        
        public void Enqueue(T item)
        {
            EnsureIsOpen();
            _queue.Enqueue(item);
        }
        
        public bool Dequeue(out T item)
        {
            EnsureIsOpen();
            return _queue.TryDequeue(out item);
        }

        public void Close()
        {
            _isOpen = false;
        }
        
        private void EnsureIsOpen()
        {
            if (!_isOpen)
            {
                throw new EndOfStreamException("SharedConcurrentQueue closed");
            }
        }
        
        
    }
}
