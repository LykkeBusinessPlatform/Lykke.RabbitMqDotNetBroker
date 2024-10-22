// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Publisher.Serializers;

namespace TestInvoke.PublishExample
{
    public class TestMessageSerializer : IRabbitMqSerializer<string>
    {
        public byte[] Serialize(string model)
        {
            return Encoding.UTF8.GetBytes(model);
        }

        public Task<byte[]> SerializeAsync(string model, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Serialize(model));
        }


        public SerializationFormat SerializationFormat { get; } = SerializationFormat.Unknown;
    }
}
