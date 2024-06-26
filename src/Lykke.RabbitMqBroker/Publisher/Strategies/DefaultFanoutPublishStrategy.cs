﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;

namespace Lykke.RabbitMqBroker.Publisher.Strategies
{
    /// <summary>
    /// Publish strategy for fanout exchange.
    /// </summary>
    [Obsolete("Use FanoutPublishStrategy instead")]
    public sealed class DefaultFanoutPublishStrategy : BasicPublishStrategy
    {
        public DefaultFanoutPublishStrategy(RabbitMqSubscriptionSettings settings)
            : base(settings, RabbitMQ.Client.ExchangeType.Fanout)
        {
        }

        protected override string GetRoutingKey(RawMessage message)
        {
            return string.Empty;
        }
    }
}
