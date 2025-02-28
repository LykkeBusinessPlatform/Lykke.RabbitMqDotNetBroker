using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware
{
    internal class ActualHandlerMiddleware<T> : IEventMiddleware<T>
    {
        private readonly Func<ReadOnlyMemory<byte>, Task> _eventHandler;
        private readonly Func<ReadOnlyMemory<byte>, CancellationToken, Task> _cancellableEventHandler;

        internal ActualHandlerMiddleware(Func<ReadOnlyMemory<byte>, Task> eventHandler)
        {
            _eventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        internal ActualHandlerMiddleware(Func<ReadOnlyMemory<byte>, CancellationToken, Task> cancellableEventHandler)
        {
            _cancellableEventHandler = cancellableEventHandler ?? throw new ArgumentNullException(nameof(cancellableEventHandler));
        }

        public async Task ProcessAsync(IEventContext<T> context)
        {
            await (_cancellableEventHandler != null
                ? _cancellableEventHandler(context.Body, context.CancellationToken)
                : _eventHandler(context.Body));
            context.MessageAcceptor.Accept();
        }
    }
}
