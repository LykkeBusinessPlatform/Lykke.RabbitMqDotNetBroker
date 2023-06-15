// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Logging
{
    /// <summary>
    /// Composite class that builds and logs outgoing messages.
    /// </summary>
    internal sealed class OutgoingMessagePersister
    {
        private readonly OutgoingMessageBuilder _builder;
        private readonly OutgoingMessageLogger _logger;

        public OutgoingMessagePersister(string exchangeName,
            string routingKey,
            IEnumerable<string> ignoredMessageTypes,
            ILogger systemLogger)
        {
            _builder = new OutgoingMessageBuilder(exchangeName, routingKey);
            _logger = new OutgoingMessageLogger(ignoredMessageTypes, systemLogger);
        }
        
        public void SetSerializationFormat(SerializationFormat format)
        {
            _builder.SetSerializationFormat(format);
        }
        
        public void Persist<TMessageModel>(byte[] messageBody, IDictionary<string, object> headers)
        {
            var message = _builder.Create<TMessageModel>(messageBody, headers);
            _logger.Log(message);
        }
    }
}
