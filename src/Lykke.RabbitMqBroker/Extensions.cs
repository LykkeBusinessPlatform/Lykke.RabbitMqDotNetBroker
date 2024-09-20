// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker
{
    internal static class RabbitMqSettingsExtension
    {
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
        internal static QueueName GetQueueName(this RabbitMqSubscriptionSettings settings)
        {
            return string.IsNullOrEmpty(settings.QueueName)
                ? QueueName.FromExchangeName(settings.ExchangeName)
                : QueueName.Create(settings.QueueName);
        }
    }
}
