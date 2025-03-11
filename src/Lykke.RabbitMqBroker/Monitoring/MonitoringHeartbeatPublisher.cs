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
            message.ListenerRoute,
            GetPropertiesConfigurator(message));
    }

    private static Action<IBasicProperties> GetPropertiesConfigurator(MonitoringHeartbeat heartbeat) =>
        props =>
        {
            props.Headers ??= new Dictionary<string, object>();
            props.DeliveryMode = NonPersistentDeliveryMode;
            props.Type = ServiceMessageType.Monitoring.ToString();
            props.SetDestinationQueueHeader(heartbeat.ListenerRoute.QueueName);
        };

    private static string SerializeMessage(MonitoringHeartbeat message) => JsonConvert.SerializeObject(message);
}