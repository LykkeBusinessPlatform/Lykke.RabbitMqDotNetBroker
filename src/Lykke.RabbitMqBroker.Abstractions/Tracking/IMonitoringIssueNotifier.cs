namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Monitoring failures notifier interface.
/// </summary>
public interface IMonitoringIssueNotifier
{
    Task NotifyNotDelivered(MessageDelivery messageDelivery);
    Task NotifyLateDelivery(MessageDelivery messageDelivery);
}