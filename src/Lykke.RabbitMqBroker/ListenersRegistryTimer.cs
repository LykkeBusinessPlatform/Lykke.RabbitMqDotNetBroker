using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Lykke.RabbitMqBroker
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class ListenersRegistryTimer : BackgroundService
    {
        private readonly IListenersRegistryProcessor _processor;
        private readonly TimeSpan _interval;
        private readonly static TimeSpan DefaultInterval = TimeSpan.FromMinutes(1);

        public ListenersRegistryTimer(IListenersRegistryProcessor processor, TimeSpan? interval = null)
        {
            _processor = processor;
            _interval = interval ?? DefaultInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _processor.ProcessListeners();
                await Task.Delay((int)_interval.TotalMilliseconds, stoppingToken);
            }
        }
    }
}