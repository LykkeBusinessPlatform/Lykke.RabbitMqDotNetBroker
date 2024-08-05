using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker
{
    internal sealed class ListenersRegistryHandlersRunner : IListenersRegistryHandlersRunner
    {
        private readonly IListenersRegistry _listenersRegistry;
        private readonly IEnumerable<IListenerRegistrationHandler> _handlers;
        private readonly ILogger<ListenersRegistryHandlersRunner> _logger;

        public ListenersRegistryHandlersRunner(
            IEnumerable<IListenerRegistrationHandler> handlers,
            ILogger<ListenersRegistryHandlersRunner> logger,
            IListenersRegistry listenersRegistry = null)
        {
            _handlers = handlers;
            _logger = logger;
            _listenersRegistry = listenersRegistry;
        }

        public async Task Run()
        {
            if (_listenersRegistry == null)
            {
                return;
            }

            foreach (var handler in _handlers)
            {
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
}