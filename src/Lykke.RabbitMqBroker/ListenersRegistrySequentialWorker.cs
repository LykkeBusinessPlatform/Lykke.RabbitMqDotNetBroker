using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Subscriber;

using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker
{

    /// <summary>
    /// The listeners registry worker.
    /// Iterates sequentially over all the registered 
    /// <see cref="IListenerRegistrationHandler"/> and calls 
    /// their <see cref="IListenerRegistrationHandler.Handle"/> method
    /// for every listener registration.
    /// Guarantees that all the handlers will be called for every registration,
    /// even if some of them throw an exception.
    /// </summary>
    internal sealed class ListenersRegistrySequentialWorker : IListenersRegistryWorker
    {
        private readonly IListenersRegistry _listenersRegistry;
        private readonly IEnumerable<IListenerRegistrationHandler> _handlers;
        private readonly ILogger<ListenersRegistrySequentialWorker> _logger;

        public ListenersRegistrySequentialWorker(
            IEnumerable<IListenerRegistrationHandler> handlers,
            ILogger<ListenersRegistrySequentialWorker> logger,
            IListenersRegistry listenersRegistry = null)
        {
            _handlers = handlers;
            _logger = logger;
            _listenersRegistry = listenersRegistry ?? new ListenersRegistry();
        }

        public Task Execute()
        {
            var tasks = from handler in _handlers
                        from registration in _listenersRegistry
                        select HandleRegistration(handler, registration);

            return tasks.RunSequentially();
        }

        private async Task HandleRegistration(IListenerRegistrationHandler handler, IListenerRegistration registration)
        {
            try
            {
                await handler.Handle(registration);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to handle listener registration {registration} by {handler}",
                    registration,
                    handler.Name);
            }
        }
    }
}