using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Lykke.RabbitMqBroker;

internal sealed class MessageDeliveryAnalysisTimer : BackgroundService
{
    private readonly IMessageDeliveryAnalysisWorker _worker;
    private readonly TimeSpan _interval;
    private readonly static TimeSpan DefaultInterval = TimeSpan.FromMinutes(1);

    public MessageDeliveryAnalysisTimer(IMessageDeliveryAnalysisWorker worker, TimeSpan? interval = null)
    {
        _worker = worker;
        _interval = interval ?? DefaultInterval;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await _worker.Execute();
        }
    }
}
