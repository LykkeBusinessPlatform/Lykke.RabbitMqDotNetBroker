// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

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
            get => _ignoredMessageTypes ??= 
                ParseList(GetEnvironmentVariable("NOVA_FILTERED_MESSAGE_TYPES"));
        }
        
        /// <summary>
        /// Flag to disable outgoing message persistence.
        /// Default value is false if not set or not parsable from environment variable
        /// "NOVA_DISABLE_OUTGOING_MESSAGE_PERSISTENCE".
        /// </summary>
        public static bool DisableOutgoingMessagePersistence
        {
            get => _disableOutgoingMessagePersistence ??=
                ParseBool(GetEnvironmentVariable("NOVA_DISABLE_OUTGOING_MESSAGE_PERSISTENCE"));
        }

        /// <summary>
        /// Parses boolean value from string. Defaults to false if parsing fails.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static bool ParseBool(string value, bool defaultValue = false) =>
            bool.TryParse(value, out var result) ? result : defaultValue;

        /// <summary>
        /// Parses comma-separated list of strings from string. Defaults to empty list if parsing fails.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static IEnumerable<string> ParseList(string value) =>
            string.IsNullOrWhiteSpace(value)
                ? Array.Empty<string>()
                : value.Split(',');

        /// <summary>
        /// Reads environment variable and trims double quotes if any.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetEnvironmentVariable(string name) =>
            Environment.GetEnvironmentVariable(name)?.Trim('"');
    }
}
