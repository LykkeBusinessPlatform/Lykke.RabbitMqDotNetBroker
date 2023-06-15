// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Logging
{
    internal interface IOutgoingMessagePersister
    {
        IOutgoingMessagePersister SetSerializationFormat(SerializationFormat format);
        IOutgoingMessagePersister SetExchangeName(string exchangeName);
        IOutgoingMessagePersister SetRoutingKey(string routingKey);
        IOutgoingMessagePersister SetIgnoredMessageTypes(IEnumerable<string> ignoredMessageTypes);
        IOutgoingMessagePersister SetSystemLogger(ILogger systemLogger);
        void Persist<TMessageModel>(byte[] messageBody, IDictionary<string, object> headers);
    }
}
