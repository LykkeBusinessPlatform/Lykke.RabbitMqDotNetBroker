using Lykke.RabbitMqBroker.Abstractions.Tracking;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Monitoring;

public static class MonitoringMessageHeaders
{
    public static MessageDeliveryId GetDeliveryId(this IBasicProperties props)
    {
        if (props == null)
        {
            return MessageDeliveryId.Empty;
        }

        if (props.Headers.TryGetValue("DeliveryId", out var value))
        {
            return MessageDeliveryId.Parse(value.ToString());
        }

        return MessageDeliveryId.Empty;

    }
}