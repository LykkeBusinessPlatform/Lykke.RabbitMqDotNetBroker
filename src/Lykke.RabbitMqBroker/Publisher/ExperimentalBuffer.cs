// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lykke.RabbitMqBroker.Publisher
{
    /// <summary>
    /// The purpose of this buffer is to avoid blocking the thread that is publishing messages to RabbitMq.
    /// The implementation relies on a thread-safe <see cref="BlockingCollection{T}"/> which has blocking and
    /// bounding capabilities instead of implementing them manually using <see cref="ConcurrentQueue{T}"/> and
    /// locking behaviour.
    /// <see cref="ConcurrentQueue{T}"/> is used only as a temporary buffer to keep current item while it is being
    /// published to RabbitMq. It is expected to have only 1 item in the queue at a time.
    /// Implementation does guarantee that no item will be lost even if the outer processing (publishing to RabbitMq)
    /// fails. 
    /// </summary>
    internal sealed class ExperimentalBuffer : IPublisherBuffer
    {
        private readonly BlockingCollection<RawMessage> _items;
        private readonly ConcurrentQueue<RawMessage> _currentItem;
        private bool _disposed;

        public ExperimentalBuffer()
        {
            _items = new BlockingCollection<RawMessage>();
            _currentItem = new ConcurrentQueue<RawMessage>();
        }
        
        public IEnumerator<RawMessage> GetEnumerator()
        {
            var items = _currentItem.ToList();
            
            items.AddRange(_items);

            return items.GetEnumerator();
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

        public int Count => _items.Count + _currentItem.Count;
        
        public void Enqueue(RawMessage message, CancellationToken cancellationToken)
        {
            _items.Add(message, cancellationToken);
        }

        /// <summary>
        /// Removes the current item from the queue.
        /// It is vital that WaitOneAndPeek() is called before this method to ensure that the queue is not empty.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Dequeue(CancellationToken cancellationToken)
        {
            _currentItem.TryDequeue(out _);
        }

        /// <summary>
        /// Peeks the item from the buffer. Being used in multi-threaded environment, it is possible that the same
        /// item will be peeked by multiple threads.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public RawMessage WaitOneAndPeek(CancellationToken cancellationToken)
        {
            if (_currentItem.Count == 0)
            {
                _currentItem.Enqueue(_items.Take(cancellationToken));
            }

            if (_currentItem.TryPeek(out var result))
                return result;

            return null;
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
