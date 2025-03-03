// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker.Subscriber.Deserializers
{
    [PublicAPI]
    public interface IMessageDeserializer<TModel>
    {
        TModel Deserialize(ReadOnlyMemory<byte> data);
        [Obsolete("Use Deserialize(ReadOnlyMemory<byte>) instead")]
        TModel Deserialize(byte[] data);
        Task<TModel> DeserializeAsync(byte[] data, CancellationToken cancellationToken = default) =>
            Task.FromResult(Deserialize(data));
    }
}
