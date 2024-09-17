using System.Collections.Generic;
using System.Text;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Monitoring;

public static class MonitoringMessageHeaders
{
    private const string DeliveryIdHeader = "DeliveryId";

    public static MessageDeliveryId GetDeliveryId(this IBasicProperties props)
    {
        if (props == null)
        {
            return MessageDeliveryId.Empty;
        }

        if (props.Headers.TryGetValue(DeliveryIdHeader, out var value))
        {
            return value switch
            {
                // when message is received from the broker, it is deserialized as byte[]
                byte[] bytes => MessageDeliveryId.Parse(Encoding.UTF8.GetString(bytes)),
                // when testing, the broker is emulated and the message is deserialized as string
                string str => MessageDeliveryId.Parse(str),
                _ => MessageDeliveryId.Empty
            };
        }

        return MessageDeliveryId.Empty;
    }

    public static void SetDeliveryId(this IBasicProperties props, MessageDeliveryId deliveryId)
    {
        if (props == null)
        {
            return;
        }

        props.Headers ??= new Dictionary<string, object>();
        props.Headers[DeliveryIdHeader] = deliveryId.ToString();
    }
}