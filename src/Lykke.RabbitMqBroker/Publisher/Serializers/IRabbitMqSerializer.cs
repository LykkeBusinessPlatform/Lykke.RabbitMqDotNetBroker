﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.


using Lykke.RabbitMqBroker.Logging;

namespace Lykke.RabbitMqBroker.Publisher.Serializers
{
    public interface IRabbitMqSerializer<in TMessageModel>
    {
        byte[] Serialize(TMessageModel model);
        SerializationFormat SerializationFormat { get; }
    }
}
