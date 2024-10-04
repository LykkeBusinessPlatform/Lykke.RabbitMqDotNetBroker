using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Lykke.RabbitMqBroker;

internal sealed class MessageDeliveryCleanupTimer : BackgroundService
{
    private readonly IMessageDeliveryCleanupWorker _worker;
    private readonly TimeSpan _interval;
    private readonly static TimeSpan DefaultInterval = TimeSpan.FromMinutes(1);

    public MessageDeliveryCleanupTimer(IMessageDeliveryCleanupWorker worker, TimeSpan? interval = null)
    {
        _worker = worker;
        _interval = interval ?? DefaultInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _worker.Execute();
            await Task.Delay((int)_interval.TotalMilliseconds, stoppingToken);
        }
    }
}
