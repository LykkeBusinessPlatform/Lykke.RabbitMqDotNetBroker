namespace Lykke.RabbitMqBroker
{
    public class RabbitMqConfiguration
    {
        public int ListenersRegistryHandleIntervalMs { get; set; }
        public RabbitMqMonitoringConfiguration Monitoring { get; set; }
    }

    public class RabbitMqMonitoringConfiguration
    {
        public int MessageExpirationMs { get; set; }
        public int PublishConfirmationWaitTimeoutMs { get; set; }
    }
}