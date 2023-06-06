﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Lykke.RabbitMqBroker.Publisher
{
    internal sealed class InMemoryBuffer : IPublisherBuffer
    {
        private readonly ConcurrentQueue<RawMessage> _items;
        private readonly AutoResetEvent _publishLock;
        private bool _disposed;

        public InMemoryBuffer()
        {
            _publishLock = new AutoResetEvent(false);
            _items = new ConcurrentQueue<RawMessage>();
        }

        public int Count => _items.Count;

        public void Enqueue(RawMessage message, CancellationToken cancellationToken)
        {
            _items.Enqueue(message);
            _publishLock.Set();
        }

        public void Dequeue(CancellationToken cancellationToken)
        {
            _items.TryDequeue(out _);
        }

        public RawMessage WaitOneAndPeek(CancellationToken cancellationToken)
        {
            do
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (_items.Count > 0 || _publishLock.WaitOne(TimeSpan.FromSeconds(1)))
                {
                    if (_items.TryPeek(out var result))
                    {
                        return result;
                    }
                }
            } while (true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
                return;

            _publishLock?.Dispose();
            
            _disposed = true;
        }

        public IEnumerator<RawMessage> GetEnumerator()
        {
            return ((IEnumerable<RawMessage>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
