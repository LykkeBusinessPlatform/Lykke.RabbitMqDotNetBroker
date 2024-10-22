// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker.Publisher.Serializers
{
    /// <summary>
    /// Uses Protobuf to serialize the message
    /// </summary>
    [PublicAPI]
    public class ProtobufMessageSerializer<TMessage> : IRabbitMqSerializer<TMessage>
    {
        /// <inheritdoc />
        public byte[] Serialize(TMessage model)
        {
            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, model);
            return stream.ToArray();
        }

        public Task<byte[]> SerializeAsync(
            TMessage model,
            CancellationToken cancellationToken = default
        ) => Task.FromResult(Serialize(model));

        public SerializationFormat SerializationFormat { get; } = SerializationFormat.Protobuf;
    }
}
