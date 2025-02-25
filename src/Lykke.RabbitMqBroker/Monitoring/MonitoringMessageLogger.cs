using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Monitoring;

public sealed class MonitoringMessageLogger : IMonitoringMessageNotifier
{
    private readonly ILogger<MonitoringMessageLogger> _logger;

    public MonitoringMessageLogger(ILogger<MonitoringMessageLogger> logger)
    {
        _logger = logger;
    }

    public Task NotifyNotDelivered(MessageDelivery messageDelivery)
    {
        _logger.LogCritical("Message delivery monitoring issue: {MessageDelivery}", messageDelivery);
        return Task.CompletedTask;
    }

    public Task NotifyLateDelivery(MessageDelivery messageDelivery)
    {
        _logger.LogWarning("Monitoring message was delivered but late: {MessageDelivery}", messageDelivery);
        return Task.CompletedTask;
    }
}