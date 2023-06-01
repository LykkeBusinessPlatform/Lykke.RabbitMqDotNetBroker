// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Lykke.RabbitMqBroker.Publisher
{
    /// <summary>
    /// The purpose of this buffer is to avoid blocking the thread that is publishing messages to RabbitMq.
    /// The implementation relies on a thread-safe <see cref="BlockingCollection{T}"/> which has blocking and
    /// bounding capabilities instead of implementing them manually using <see cref="ConcurrentQueue{T}"/> and
    /// locking behaviour.
    /// Implementation does not guarantee that no item will be lost if the outer processing (publishing to RabbitMq)
    /// fails. 
    /// </summary>
    internal sealed class LockFreeBuffer : IPublisherBuffer
    {
        private readonly BlockingCollection<RawMessage> _items;
        private bool _disposed;

        public LockFreeBuffer()
        {
            _items = new BlockingCollection<RawMessage>();
        }
        
        /// <summary>
        /// It is supposed that enumeration is being called only in case of persisting items to the repository,
        /// e. g. when the application is shutting down. Thus, we can use consuming enumerable to be faster and
        /// less resource consuming. However, it can be reimplemented to return a copy of the collection if needed.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RawMessage> GetEnumerator()
        {
            return _items.GetConsumingEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public int Count => _items.Count;
        
        public void Enqueue(RawMessage message, CancellationToken cancellationToken)
        {
            _items.Add(message, cancellationToken);
        }

        public void Dequeue(CancellationToken cancellationToken)
        {
            // Do nothing
        }

        public RawMessage WaitOneAndPeek(CancellationToken cancellationToken)
        {
            return _items.Take(cancellationToken);
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
                return; 

            _items?.Dispose();

            _disposed = true;
        }
    }
}
