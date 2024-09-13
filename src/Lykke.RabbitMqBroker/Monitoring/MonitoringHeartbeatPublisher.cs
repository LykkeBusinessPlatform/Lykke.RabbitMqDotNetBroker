using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Publisher;

using Newtonsoft.Json;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Monitoring;

internal sealed class MonitoringHeartbeatPublisher(
    ITrackableMessagePublisher<MonitoringHeartbeat> publisher) : IMessageProducer<MonitoringHeartbeat>
{
    private readonly ITrackableMessagePublisher<MonitoringHeartbeat> _publisher = publisher;
    private const byte NonPersistentDeliveryMode = 1;

    public async Task ProduceAsync(MonitoringHeartbeat message)
    {
        var messageBody = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(SerializeMessage(message)));

        await _publisher.Publish(
            messageBody,
            props => ConfigureProperties(props, message),
            message.Metadata.ExchangeName,
            message.Metadata.RoutingKey);
    }

    private static void ConfigureProperties(IBasicProperties properties, MonitoringHeartbeat message)
    {
        properties.Headers ??= new Dictionary<string, object>();
        properties.Headers["Route"] = message.Metadata.RouteText;

        properties.DeliveryMode = NonPersistentDeliveryMode;
        properties.Type = ServiceMessageType.Monitoring.ToString();
    }

    private static string SerializeMessage(MonitoringHeartbeat message) => JsonConvert.SerializeObject(message);
}