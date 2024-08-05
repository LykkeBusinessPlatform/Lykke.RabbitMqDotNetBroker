using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Lykke.RabbitMqBroker
{
    internal sealed class ListenersRegistryTimer : BackgroundService
    {
        private readonly IListenersRegistryHandlersRunner _handlersRunner;
        private readonly TimeSpan _interval;
        private readonly static TimeSpan DefaultInterval = TimeSpan.FromMinutes(1);

        public ListenersRegistryTimer(IListenersRegistryHandlersRunner handlersRunner, TimeSpan? interval = null)
        {
            _handlersRunner = handlersRunner;
            _interval = interval ?? DefaultInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _handlersRunner.Run();
                await Task.Delay(_interval.Milliseconds, stoppingToken);
            }
        }
    }
}