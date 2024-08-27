namespace Lykke.RabbitMqBroker
{
    public class RabbitMqConfiguration
    {
        public RabbitMqMonitoringConfiguration Monitoring { get; set; }
    }

    public class RabbitMqMonitoringConfiguration
    {
        public int IntervalMs { get; set; }
        /// <summary>
        /// Delivery mode for messages. 1 - non-persistent, 2 - persistent.
        /// </summary>
        public byte DeliveryMode { get; set; }
        public int MessageExpirationMs { get; set; }
        public int PublishConfirmationWaitTimeoutMs { get; set; }
    }
}