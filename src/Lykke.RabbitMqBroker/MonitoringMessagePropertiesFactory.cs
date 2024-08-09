using System;
using System.Collections.Generic;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker
{
    internal sealed class MonitoringMessagePropertiesFactory : IMonitoringMessagePropertiesFactory
    {
        private readonly byte _deliveryMode;
        private readonly int _messageExpirationMs;
        private const byte DefaultDeliveryMode = 1;
        private const int DefaultMessageExpirationMs = 10_000;

        public MonitoringMessagePropertiesFactory(RabbitMqMonitoringConfiguration configuration)
        {
            _deliveryMode = configuration.DeliveryMode == default
                ? DefaultDeliveryMode
                : configuration.DeliveryMode;
            _messageExpirationMs = configuration.MessageExpirationMs <= 0
                ? DefaultMessageExpirationMs
                : configuration.MessageExpirationMs;
        }

        public IBasicProperties Create(IModel channel, string messageRoute)
        {
            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "Host", Environment.MachineName },
                { "Route", messageRoute },
                { "Version", "1.0" },
            };
            props.AppId = new HostAssemblyDisplayName().ToString();
            props.DeliveryMode = _deliveryMode;
            props.Expiration = _messageExpirationMs.ToString();
            props.MessageId = Guid.NewGuid().ToString();
            props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            props.Type = ServiceMessageType.Monitoring.ToString();
            return props;
        }
    }
}