namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Monitoring failures notifier interface.
/// </summary>
public interface IMonitoringIssueNotifier
{
    Task Notify(MessageDelivery messageDelivery);
}