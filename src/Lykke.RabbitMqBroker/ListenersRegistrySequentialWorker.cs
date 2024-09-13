using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            _listenersRegistry = listenersRegistry;
        }

        public async Task Execute()
        {
            if (_listenersRegistry == null)
            {
                return;
            }

            foreach (var handler in _handlers)
                foreach (var registration in _listenersRegistry)
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
}