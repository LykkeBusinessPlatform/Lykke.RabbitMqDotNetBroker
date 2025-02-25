using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware
{
    internal interface IMiddlewareQueue<T>
    {
        /// <summary>
        /// Adds middleware to the end of the queue
        /// </summary>
        /// <param name="middleware"></param>
        void AddMiddleware(IEventMiddleware<T> middleware);

        /// <summary>
        /// Adds middleware to the queue at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="middleware"></param>
        void AddMiddlewareAt(int index, IEventMiddleware<T> middleware);

        Task RunMiddlewaresAsync(
            ReadOnlyMemory<byte> body,
            [CanBeNull] IBasicProperties properties,
            T evt,
            IMessageAcceptor ma,
            CancellationToken cancellationToken);

        IEventMiddleware<T> GetNext(int currentIndex);

        bool HasMiddleware<TMiddleware>();
    }
}
