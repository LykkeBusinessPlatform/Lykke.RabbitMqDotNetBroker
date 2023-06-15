// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.RabbitMqBroker
{
    internal static class EnvironmentVariables
    {
        private static IEnumerable<string> _ignoredMessageTypes;
        private static bool? _disableOutgoingMessagePersistence;
        
        /// <summary>
        /// The list of message types that should be ignored by the persister.
        /// Defaults to empty list if not set or not parsable from environment variable
        /// "NOVA_FILTERED_MESSAGE_TYPES".
        /// </summary>
        public static IEnumerable<string> IgnoredMessageTypes
        {
            get
            {
                if (_ignoredMessageTypes == null)
                {
                    var ignoredMessageTypes = Environment.GetEnvironmentVariable("NOVA_FILTERED_MESSAGE_TYPES");
                    _ignoredMessageTypes = string.IsNullOrWhiteSpace(ignoredMessageTypes)
                        ? Array.Empty<string>()
                        : ignoredMessageTypes.Split(',').Distinct();
                }
                
                return _ignoredMessageTypes;
            }
        }
        
        /// <summary>
        /// Flag to disable outgoing message persistence.
        /// Default value is false if not set or not parsable from environment variable
        /// "NOVA_DISABLE_OUTGOING_MESSAGE_PERSISTENCE".
        /// </summary>
        public static bool DisableOutgoingMessagePersistence
        {
            get
            {
                if (_disableOutgoingMessagePersistence.HasValue)
                    return _disableOutgoingMessagePersistence.Value;
                
                var disableOutgoingMessagePersistence = 
                    Environment.GetEnvironmentVariable("NOVA_DISABLE_OUTGOING_MESSAGE_PERSISTENCE");
                _disableOutgoingMessagePersistence =
                    bool.TryParse(disableOutgoingMessagePersistence, out var result) && result;
                
                return _disableOutgoingMessagePersistence.Value;
            }
        }
    }
}
