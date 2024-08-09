namespace Lykke.RabbitMqBroker
{
    public class RabbitMqConfiguration
    {
        public RabbitMqMonitoringConfiguration Monitoring { get; set; }
    }

    public class RabbitMqMonitoringConfiguration
    {
        public int IntervalMs { get; set; }
        public byte DeliveryMode { get; set; }
        public int MessageExpirationMs { get; set; }
        public int PublishConfirmationWaitTimeoutMs { get; set; }
    }
}