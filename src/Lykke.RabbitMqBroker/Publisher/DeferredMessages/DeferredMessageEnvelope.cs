﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker.Publisher.DeferredMessages
{
    /// <summary>
    /// Deferred message envelope, used to transfer message data between repository and deferred messaging core
    /// </summary>
    [PublicAPI]
    public class DeferredMessageEnvelope
    {
        /// <summary>
        /// Should identify the message within repository instance
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// The message
        /// </summary>
        public RawMessage Message { get; }

        /// <summary>
        /// Deferred message envelope, used to transfer message data between repository and deferred messaging core
        /// </summary>
        public DeferredMessageEnvelope(string key, RawMessage message)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
