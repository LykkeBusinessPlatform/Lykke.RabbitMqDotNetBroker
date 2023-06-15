using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Logging
{
    internal sealed class OutgoingMessageLogger
    {
        private readonly HashSet<string> _ignoredMessageTypes;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a logger that logs outgoing messages.
        /// </summary>
        /// <param name="ignoredMessageTypes">Types of outgoing messages that should not be logged</param>
        /// <param name="logger"></param>
        /// <exception cref="NullReferenceException"></exception>
        public OutgoingMessageLogger(IEnumerable<string> ignoredMessageTypes, ILogger logger)
        {
            _ignoredMessageTypes = ignoredMessageTypes?.Distinct().ToHashSet() ?? new HashSet<string>();
            _logger = logger;
        }

        /// <summary>
        /// Logs outgoing message in a specific format.
        /// </summary>
        /// <param name="message"></param>
        public void Log(OutgoingMessage message)
        {
            if (_ignoredMessageTypes.Contains(message.MessageTypeName))
                return;

            _logger.LogInformation(message.ToString());
        }
    }
}
