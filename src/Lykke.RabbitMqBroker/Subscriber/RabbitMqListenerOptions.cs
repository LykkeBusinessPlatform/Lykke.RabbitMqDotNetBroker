// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

namespace Lykke.RabbitMqBroker.Subscriber
{
    public class RabbitMqListenerOptions<T> where T : class
    {
        public bool ShareConnection { get; set; } = true;
        public SerializationFormat SerializationFormat { get; set; } = SerializationFormat.Json;
        public SubscriptionTemplate SubscriptionTemplate { get; set; } = SubscriptionTemplate.NoLoss;
        public byte ConsumerCount { get; set; } = 1;
        public bool ReadCorrelationId { get; set; } = false;
        
        public static RabbitMqListenerOptions<T> Default => new();
    }
}
