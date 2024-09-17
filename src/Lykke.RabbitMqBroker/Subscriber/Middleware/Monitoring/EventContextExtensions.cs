namespace Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

internal static class EventContextExtensions
{
    public static bool IsMonitoringMessage<T>(this IEventContext<T> context)
    {
        return context.BasicProperties.Type == ServiceMessageType.Monitoring.ToString();
    }
}