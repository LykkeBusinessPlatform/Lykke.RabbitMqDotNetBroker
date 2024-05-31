// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker
{
    internal static class RabbitMqSettingsExtension
    {
        private const string DeadLetterExchangeHeader = "x-dead-letter-exchange";
        
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
            return $"{settings.GetQueueName()}-poison";
        }
        
        /// <summary>
        /// Create arguments to be used when declaring the queue.
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        [CanBeNull]
        internal static IDictionary<string, object> CreateArguments(this RabbitMqSubscriptionSettings settings)
        {
            if (string.IsNullOrEmpty(settings.DeadLetterExchangeName))
            {
                return null;
            }
            
            return new Dictionary<string, object>
            {
                { DeadLetterExchangeHeader, settings.DeadLetterExchangeName }
            };
        }
        
        internal static bool ShouldConfigureDeadLetter(this RabbitMqSubscriptionSettings settings)
        {
            return !string.IsNullOrEmpty(settings.DeadLetterExchangeName);
        }
    }
}
