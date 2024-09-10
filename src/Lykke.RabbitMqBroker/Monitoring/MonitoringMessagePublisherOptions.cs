using System;

using Lykke.RabbitMqBroker.Publisher;

namespace Lykke.RabbitMqBroker.Monitoring
{
    internal class MonitoringMessagePublisherOptions : RabbitMqPublisherOptions<MonitoringMessage>
    {
        public static MonitoringMessagePublisherOptions Create(
            int confirmationTimeoutMs,
            int messageExpirationMs = 0) =>
            new MonitoringMessagePublisherOptions
            {
                ShareConnection = false,
                ConnectionNameWhenExclusive = "Monitoring",
                PublisherConfirmsEnabled = true,
                Mandatory = true,
                ConfirmationTimeout = TimeSpan.FromMilliseconds(confirmationTimeoutMs),
                MessageExpirationMs = messageExpirationMs switch
                {
                    > 0 => (uint)messageExpirationMs,
                    _ => 0
                }
            };
    }
}