// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

namespace Lykke.RabbitMqBroker.Subscriber
{
    public class RabbitMqListenerOptions<T> where T : class
    {
        /// <summary>
        /// Instructs the listener to request shared connection instead of creating a new one.
        /// </summary>
        public bool ShareConnection { get; set; } = true;
        
        /// <summary>
        /// Creates subscriber(-s) with JSON serialization format by default.
        /// </summary>
        public SerializationFormat SerializationFormat { get; set; } = SerializationFormat.Json;
        
        /// <summary>
        /// Creates subscriber(-s) with "no loss" subscription template by default.
        /// </summary>
        public SubscriptionTemplate SubscriptionTemplate { get; set; } = SubscriptionTemplate.NoLoss;
        
        /// <summary>
        /// The number of low-level subscribers to create.
        /// </summary>
        public byte ConsumerCount { get; set; } = 1;
        
        /// <summary>
        /// Instructs the listener to read the correlation ID from the message headers.
        /// </summary>
        public bool ReadCorrelationId { get; set; } = false;
        
        public static RabbitMqListenerOptions<T> Default => new();
    }
}
