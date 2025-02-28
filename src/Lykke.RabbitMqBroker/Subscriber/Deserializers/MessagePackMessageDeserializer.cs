// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using JetBrains.Annotations;

using MessagePack;

namespace Lykke.RabbitMqBroker.Subscriber.Deserializers;

/// <summary>
/// Uses MessagePack to deserialize the message
/// </summary>
[PublicAPI]
public class MessagePackMessageDeserializer<TMessage> : IMessageDeserializer<TMessage>
{
    private readonly MessagePackSerializerOptions _options;

    /// <summary>
    /// Create an instance of <see cref="MessagePackMessageDeserializer{TMessage}"/>.
    /// Kept for backward compatibility.
    /// </summary>
    /// <param name="formatterResolver">
    /// If resolver is not specified it uses 
    /// <see cref="MessagePack.Resolvers.ContractlessStandardResolver.Options"/>.
    /// </param>
    public MessagePackMessageDeserializer(IFormatterResolver formatterResolver)
    {
        _options = formatterResolver switch
        {
            null => MessagePack.Resolvers.ContractlessStandardResolver.Options,
            _ => MessagePackSerializerOptions.Standard.WithResolver(formatterResolver),
        };
    }

    /// <summary>
    /// Create an instance of <see cref="MessagePackMessageDeserializer{TMessage}"/>.
    /// </summary>
    /// <param name="options">
    /// If options is not specified it uses <see cref="MessagePackSerializerOptions.Standard"/>.
    /// </param>
    public MessagePackMessageDeserializer(MessagePackSerializerOptions options = null)
    {
        _options = options ?? MessagePackSerializerOptions.Standard;
    }

    public TMessage Deserialize(byte[] data) => MessagePackSerializer.Deserialize<TMessage>(data, _options);

    public TMessage Deserialize(ReadOnlyMemory<byte> data) => MessagePackSerializer.Deserialize<TMessage>(data, _options);


    public async Task<TMessage> DeserializeAsync(
        byte[] data,
        CancellationToken cancellationToken = default
    )
    {
        await using var stream = new MemoryStream(data);
        return await MessagePackSerializer.DeserializeAsync<TMessage>(stream, _options, cancellationToken);
    }
}
