// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

namespace Lykke.RabbitMqBroker.Subscriber
{
    public class RabbitMqListenerOptions<T> where T : class
    {
        /// <summary>
        /// Instructs the listener to request shared connection if possible instead of creating a new one.
        /// If there is no shared connection available, a new one will be created.
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
        public ConsumerCount ConsumerCount { get; set; } = ConsumerCount.Default;


        public static class Json
        {
            public static RabbitMqListenerOptions<T> Default => NoLoss;

            public static RabbitMqListenerOptions<T> NoLoss => new()
            {
                SerializationFormat = SerializationFormat.Json,
                ShareConnection = true,
                SubscriptionTemplate = SubscriptionTemplate.NoLoss
            };

            public static RabbitMqListenerOptions<T> LossAcceptable => new()
            {
                SerializationFormat = SerializationFormat.Json,
                ShareConnection = true,
                SubscriptionTemplate = SubscriptionTemplate.LossAcceptable
            };
        }

        public static class MessagePack
        {
            public static RabbitMqListenerOptions<T> Default => NoLoss;

            public static RabbitMqListenerOptions<T> NoLoss => new()
            {
                SerializationFormat = SerializationFormat.Messagepack,
                ShareConnection = true,
                SubscriptionTemplate = SubscriptionTemplate.NoLoss
            };

            public static RabbitMqListenerOptions<T> LossAcceptable => new()
            {
                SerializationFormat = SerializationFormat.Messagepack,
                ShareConnection = true,
                SubscriptionTemplate = SubscriptionTemplate.LossAcceptable
            };
        }

        public void CopyFrom<TModel>(RabbitMqListenerOptions<TModel> source) where TModel : class
        {
            ShareConnection = source.ShareConnection;
            SerializationFormat = source.SerializationFormat;
            SubscriptionTemplate = source.SubscriptionTemplate;
            ConsumerCount = source.ConsumerCount;
        }
    }
}