
using Lykke.RabbitMqBroker;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

internal sealed class ListenerRegistryLogger : IHostedService
{
    private readonly ILogger<ListenerRegistryLogger> _logger;
    private readonly IListenersRegistry? _listenersRegistry;

    public ListenerRegistryLogger(ILogger<ListenerRegistryLogger> logger, IListenersRegistry? listenersRegistry = null)
    {
        _logger = logger;
        _listenersRegistry = listenersRegistry;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        LogListeners();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        LogListeners();

        return Task.CompletedTask;
    }

    private void LogListeners()
    {
        if (_listenersRegistry == null)
        {
            _logger.LogWarning("Listeners registry is not available");
            return;
        }

        foreach (var listener in _listenersRegistry)
        {
            _logger.LogInformation($"Listener: exchange {listener.ExchangeName} -> queue {listener.QueueName} with routing key ({listener.RoutingKey}) registered");
        }
    }
}