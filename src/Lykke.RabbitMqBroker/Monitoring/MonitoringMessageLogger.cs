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
        _logger.LogCritical(
            new MonitoringHeartbeatUndeliveredException(
                messageDelivery.DispatchedTimestamp,
                messageDelivery.Failure,
                messageDelivery.Route), "");
        return Task.CompletedTask;
    }

    public Task NotifyLateDelivery(MessageDelivery messageDelivery)
    {
        _logger.LogError(
            new MonitoringHearbeatLateDeliveryException(
                messageDelivery.DispatchedTimestamp,
                messageDelivery.ReceivedTimestamp,
                messageDelivery.Route), "");
        return Task.CompletedTask;
    }
}