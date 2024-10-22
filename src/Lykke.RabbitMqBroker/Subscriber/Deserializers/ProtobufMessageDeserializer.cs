﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker.Subscriber.Deserializers
{
    /// <summary>
    /// Uses Protobuf to deserialize the message
    /// </summary>
    [PublicAPI]
    public class ProtobufMessageDeserializer<TMessage> : IMessageDeserializer<TMessage>
    {
        /// <inheritdoc />
        public TMessage Deserialize(byte[] data)
        {
            using var stream = new MemoryStream(data);
            return ProtoBuf.Serializer.Deserialize<TMessage>(stream);
        }
    }
}
