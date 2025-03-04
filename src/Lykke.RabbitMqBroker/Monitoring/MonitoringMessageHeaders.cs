using System.Collections.Generic;
using System.Text;

using Lykke.RabbitMqBroker.Abstractions.Tracking;
using Lykke.RabbitMqBroker.Subscriber;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Monitoring;

public static class MonitoringMessageHeaders
{
    internal const string DeliveryIdHeader = "codedoc-monitoring-delivery-id";
    internal const string DestinationQueueHeader = "codedoc-monitoring-destination-queue";
    internal const string HostHeader = "codedoc-monitoring-host";

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

    public static QueueName GetDestinationQueue(this IBasicProperties props)
    {
        if (props == null)
        {
            return null;
        }

        if (props.Headers.TryGetValue(DestinationQueueHeader, out var value))
        {
            return value switch
            {
                // when message is received from the broker, it is deserialized as byte[]
                byte[] bytes => QueueName.Create(Encoding.UTF8.GetString(bytes)),
                // when testing, the broker is emulated and the message is deserialized as string
                string str => QueueName.Create(str),
                _ => null
            };
        }

        return null;
    }

    public static void SetHostHeader(this IBasicProperties props, string hostname)
    {
        if (props == null)
        {
            return;
        }

        props.Headers ??= new Dictionary<string, object>();
        props.Headers[HostHeader] = hostname;
    }

    public static void SetDeliveryIdHeader(this IBasicProperties props, MessageDeliveryId deliveryId)
    {
        if (props == null)
        {
            return;
        }

        props.Headers ??= new Dictionary<string, object>();
        props.Headers[DeliveryIdHeader] = deliveryId.ToString();
    }

    public static void SetDestinationQueueHeader(this IBasicProperties props, QueueName queueName)
    {
        if (props == null)
        {
            return;
        }

        props.Headers ??= new Dictionary<string, object>();
        props.Headers[DestinationQueueHeader] = queueName.ToString();
    }
}