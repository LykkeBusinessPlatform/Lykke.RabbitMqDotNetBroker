// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;
using MessagePack;

namespace Lykke.RabbitMqBroker.Publisher.Serializers;

/// <summary>
/// Uses MessagePack to serialize the message.
/// </summary>
[PublicAPI]
public class MessagePackMessageSerializer<TMessage> : IRabbitMqSerializer<TMessage>
{
    private readonly MessagePackSerializerOptions _options;

    /// <summary>
    /// If resolver is not specified it uses <see cref="MessagePack.Resolvers.ContractlessStandardResolver.Options"/>.
    /// </summary>
    /// <param name="formatResolver"></param>
    public MessagePackMessageSerializer(IFormatterResolver formatResolver = null)
    {
        _options = formatResolver switch
        {
            null => MessagePack.Resolvers.ContractlessStandardResolver.Options,
            _ => MessagePackSerializerOptions.Standard.WithResolver(formatResolver)
        };
    }

    /// <summary>
    /// If options is not specified it uses <see cref="MessagePackSerializerOptions.Standard"/>.
    /// </summary>
    /// <param name="options"></param>
    public MessagePackMessageSerializer(MessagePackSerializerOptions options = null)
    {
        _options = options ?? MessagePackSerializerOptions.Standard;
    }

    public byte[] Serialize(TMessage model) => MessagePackSerializer.Serialize(model, _options);

    public async Task<byte[]> SerializeAsync(TMessage model, CancellationToken cancellationToken = default)
    {
        await using var stream = new MemoryStream();
        await MessagePackSerializer.SerializeAsync(stream, model, _options, cancellationToken);
        return stream.ToArray();
    }

    public SerializationFormat SerializationFormat { get; } = SerializationFormat.Messagepack;
}
