// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.


using System.Threading;
using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Publisher.Serializers
{
    public interface IRabbitMqSerializer<in TMessageModel>
    {
        byte[] Serialize(TMessageModel model);
        Task<byte[]> SerializeAsync(TMessageModel model, CancellationToken cancellationToken = default);
        SerializationFormat SerializationFormat { get; }
    }
}
