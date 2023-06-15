// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Lykke.RabbitMqBroker.Logging
{
    internal sealed class NullOutgoingMessagePersister : IOutgoingMessagePersister
    {
        public void SetSerializationFormat(SerializationFormat format)
        {
        }

        public void Persist<TMessageModel>(byte[] messageBody, IDictionary<string, object> headers)
        {
        }
    }
}
