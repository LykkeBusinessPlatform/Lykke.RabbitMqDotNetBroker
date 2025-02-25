namespace Lykke.RabbitMqBroker.Abstractions.Tracking;

/// <summary>
/// Monitoring message failures notifier interface.
/// 
/// It supports only message delivery failures so far,
/// however it can be extended to support other types 
/// of monitoring issues, like miconfiguration etc.
/// </summary>
public interface IMonitoringMessageNotifier
{
    Task NotifyNotDelivered(MessageDelivery messageDelivery);
    Task NotifyLateDelivery(MessageDelivery messageDelivery);
}