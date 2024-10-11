using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Monitoring;

public sealed class MonitoringIssueLogger : IMonitoringIssueNotifier
{
    private readonly ILogger<MonitoringIssueLogger> _logger;

    public MonitoringIssueLogger(ILogger<MonitoringIssueLogger> logger)
    {
        _logger = logger;
    }

    public Task Notify(MessageDelivery messageDelivery)
    {
        _logger.LogCritical("Message delivery monitoring issue: {MessageDelivery}", messageDelivery);
        return Task.CompletedTask;
    }
}