using System;

namespace Lykke.RabbitMqBroker
{
    public class RabbitMqConfiguration
    {
        public int ListenersRegistryHandleIntervalMs { get; set; }
        public RabbitMqMonitoringConfiguration Monitoring { get; set; }
    }

    public class RabbitMqMonitoringConfiguration
    {
        /// <summary>
        /// Delivery messages RabbitMQ expiration period.
        /// If message is not delivered during this period, it will be removed from the queue.
        /// </summary>
        public int MessageExpirationMs { get; set; }

        /// <summary>
        /// Delivery messages retention period.
        /// </summary>
        public TimeSpan? MessageRetentionPeriod { get; set; }

        /// <summary>
        /// Delivery messages cleanup period.
        /// The cleanup worker is triggered every this period.
        /// </summary>
        public TimeSpan? MessagesCleanupPeriod { get; set; }

        /// <summary>
        /// Delivery messages period to consider as honorable infrastructure delay.
        /// </summary>
        public int? MessageDeliveryFairDelayMs { get; set; }

        /// <summary>
        /// Delivery analysis period.
        /// </summary>
        public TimeSpan? AnalysisPeriod { get; set; }
        public int PublishConfirmationWaitTimeoutMs { get; set; }
    }
}