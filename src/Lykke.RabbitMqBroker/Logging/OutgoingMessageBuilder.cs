using System;
using System.Collections.Generic;

namespace Lykke.RabbitMqBroker.Logging
{
    internal sealed class OutgoingMessageBuilder
    {
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private SerializationFormat _serializationFormat;

        public OutgoingMessageBuilder(string exchangeName, string routingKey, SerializationFormat serializationFormat)
        {
            _exchangeName = exchangeName;
            _routingKey = routingKey;
            _serializationFormat = serializationFormat;
        }

        public OutgoingMessageBuilder(string exchangeName, string routingKey) : this(exchangeName, routingKey, 
            SerializationFormat.Unknown)
        {
        }

        public void SetSerializationFormat(SerializationFormat format)
        {
            if (_serializationFormat != SerializationFormat.Unknown)
            {
                throw new InvalidOperationException($"Cannot set {nameof(format)} twice");
            }
            _serializationFormat = format;
        }

        public OutgoingMessage Create<TMessage>(byte[] serializedMessage, IDictionary<string, object> headers)
        {
            var type = typeof(TMessage);
            
            var message = new OutgoingMessage
            {
                MessageTypeName = type.Name,
                MessageTypeFullName = type.FullName,
                Exchange = _exchangeName,
                RoutingKey = _routingKey,
                Format = _serializationFormat,
                Headers = headers == null
                    ? new Dictionary<string, object>()
                    : new Dictionary<string, object>(headers),
                Timestamp = DateTime.UtcNow,
                Message = Convert.ToBase64String(serializedMessage),
            };

            return message;
        }
    }
}
