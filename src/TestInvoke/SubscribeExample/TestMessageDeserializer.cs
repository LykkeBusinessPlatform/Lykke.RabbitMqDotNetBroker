﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Text;

using Lykke.RabbitMqBroker.Subscriber.Deserializers;

namespace TestInvoke.SubscribeExample
{
    public class TestMessageDeserializer : IMessageDeserializer<string>
    {
        public string Deserialize(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public string Deserialize(ReadOnlyMemory<byte> data)
        {
            return Encoding.UTF8.GetString(data.Span);
        }

    }
}
