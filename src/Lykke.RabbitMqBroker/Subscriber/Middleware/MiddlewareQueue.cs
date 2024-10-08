using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware
{
    internal class MiddlewareQueue<T> : IMiddlewareQueue<T>, IEnumerable<IEventMiddleware<T>>
    {
        private readonly List<IEventMiddleware<T>> _eventMiddlewares = [];
        private readonly RabbitMqSubscriptionSettings _settings;

        internal MiddlewareQueue(RabbitMqSubscriptionSettings settings)
        {
            _settings = settings;
        }

        public void AddMiddleware(IEventMiddleware<T> middleware)
        {
            _eventMiddlewares.Add(middleware);
        }

        public void AddMiddlewareAt(int index, IEventMiddleware<T> middleware)
        {
            if (index < 0)
                throw new InvalidOperationException($"{nameof(index)} must be non-negative");

            if (index > _eventMiddlewares.Count)
                throw new InvalidOperationException($"{nameof(index)} must be less than or equal to the number of middlewares");

            _eventMiddlewares.Insert(index, middleware);
        }

        public Task RunMiddlewaresAsync(
            ReadOnlyMemory<byte> body,
            [CanBeNull] IBasicProperties properties,
            T evt,
            IMessageAcceptor ma,
            CancellationToken cancellationToken)
        {
            var context = new EventContext<T>(
                body,
                properties,
                evt,
                ma,
                _settings,
                0,
                this,
                cancellationToken);
            return _eventMiddlewares[0].ProcessAsync(context);
        }

        public IEventMiddleware<T> GetNext(int currentIndex)
        {
            if (currentIndex < 0)
                throw new InvalidOperationException($"{nameof(currentIndex)} must be non-negative");

            if (currentIndex >= _eventMiddlewares.Count - 1)
                return null;

            return _eventMiddlewares[currentIndex + 1];
        }

        public bool HasMiddleware<TMiddleware>()
        {
            var type = typeof(TMiddleware);
            return _eventMiddlewares.Exists(m => m.GetType() == type);
        }

        public IEnumerator<IEventMiddleware<T>> GetEnumerator()
        {
            return _eventMiddlewares.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
