// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Subscriber.Deserializers
{
    public class DefaultStringDeserializer : IMessageDeserializer<string>
    {
        public string Deserialize(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public Task<string> DeserializeAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Deserialize(data));
        }

    }
}
