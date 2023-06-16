// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Logging
{
    internal sealed class NullOutgoingMessagePersister : IOutgoingMessagePersister
    {
        public IOutgoingMessagePersister SetExchangeName(string exchangeName)
        {
            return this;
        }

        public IOutgoingMessagePersister SetRoutingKey(string routingKey)
        {
            return this;
        }

        public IOutgoingMessagePersister SetIgnoredMessageTypes(IEnumerable<string> ignoredMessageTypes)
        {
            return this;
        }

        public IOutgoingMessagePersister SetSystemLogger(ILogger systemLogger)
        {
            return this;
        }

        IOutgoingMessagePersister IOutgoingMessagePersister.SetSerializationFormat(SerializationFormat format)
        {
            return this;
        }

        public void Persist<TMessageModel>(byte[] messageBody, IDictionary<string, object> headers)
        {
        }
    }
}
