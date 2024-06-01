// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;

namespace Lykke.RabbitMqBroker
{
    internal static class RabbitMqSettingsExtension
    {
        private const string PoisonQueueSuffix = "poison"; 
        
        internal static string GetPublisherDisplayName(this RabbitMqSubscriptionSettings settings)
        {
            return $"Publisher {settings.ExchangeName}";
        }

        internal static string GetSubscriberDisplayName(this RabbitMqSubscriptionSettings settings)
        {
            return $"Subscriber {settings.QueueName}{(!string.IsNullOrWhiteSpace(settings.RoutingKey) ? $"/{settings.RoutingKey}" : string.Empty)}";
        }

        internal static string GetQueueOrExchangeDisplayName(this RabbitMqSubscriptionSettings settings)
        {
            return $"Exchange: {settings.ExchangeName}{(!string.IsNullOrEmpty(settings.QueueName) ? $" Queue: {settings.QueueName}" : string.Empty)}";
        }

        /// <summary>
        /// Gets the queue name. If the queue name is not set, it will be generated.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal static string GetQueueName(this RabbitMqSubscriptionSettings settings)
        {
            return string.IsNullOrEmpty(settings.QueueName)
                ? settings.ExchangeName + "." + Guid.NewGuid()
                : settings.QueueName;
        }
        
        /// <summary>
        /// Gets the poison queue name. If the queue name is not set, it will be generated.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        internal static string GetPoisonQueueName(this RabbitMqSubscriptionSettings settings)
        {
            return settings.GetQueueName().GetPoisonQueueName();
        }

        /// <summary>
        /// Gets the poison queue name for the regular queue.
        /// </summary>
        /// <param name="regularQueueName"></param>
        /// <returns></returns>
        internal static string GetPoisonQueueName(this string regularQueueName)
        {
            if (string.IsNullOrEmpty(regularQueueName))
            {
                throw new ArgumentNullException(nameof(regularQueueName));
            }
            
            if (regularQueueName.EndsWith(PoisonQueueSuffix))
            {
                return regularQueueName;
            }
            
            return $"{regularQueueName}-{PoisonQueueSuffix}";
        }
    }
}
