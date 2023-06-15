// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Logging
{
    /// <summary>
    /// Composite class that builds and logs outgoing messages.
    /// </summary>
    internal sealed class OutgoingMessageLogPersister : IOutgoingMessagePersister
    {
        [CanBeNull] private OutgoingMessageBuilder _builder;
        [CanBeNull] private OutgoingMessageLogger _logger;
        
        [CanBeNull] private string _exchangeName;
        [CanBeNull] private string _routingKey;
        [CanBeNull] private IEnumerable<string> _ignoredMessageTypes;
        [CanBeNull] private ILogger _systemLogger;
        [CanBeNull] private SerializationFormat? _serializationFormat;

        public IOutgoingMessagePersister SetExchangeName(string exchangeName)
        {
            if (_exchangeName != null)
                throw new InvalidOperationException("Exchange name can't be changed after it was set.");
            
            _exchangeName = exchangeName;

            return this;
        }

        public IOutgoingMessagePersister SetRoutingKey(string routingKey)
        {
            if (_routingKey != null)
                throw new InvalidOperationException("Routing key can't be changed after it was set.");
            
            _routingKey = routingKey;
            
            return this;
        }

        public IOutgoingMessagePersister SetIgnoredMessageTypes(IEnumerable<string> ignoredMessageTypes)
        {
            if (_ignoredMessageTypes != null)
                throw new InvalidOperationException("Ignored message types can't be changed after it was set.");
            
            _ignoredMessageTypes = ignoredMessageTypes ?? throw new ArgumentNullException(nameof(ignoredMessageTypes));
            
            return this;
        }

        public IOutgoingMessagePersister SetSystemLogger(ILogger systemLogger)
        {
            if (_systemLogger != null)
                throw new InvalidOperationException("System logger can't be changed after it was set.");
            
            _systemLogger = systemLogger ?? throw new ArgumentNullException(nameof(systemLogger));

            return this;
        }

        IOutgoingMessagePersister IOutgoingMessagePersister.SetSerializationFormat(SerializationFormat format)
        {
            if (_serializationFormat != null)
                throw new InvalidOperationException("Serialization format can't be changed after it was set.");

            _serializationFormat = format;
            
            return this;
        }

        public void Persist<TMessageModel>(byte[] messageBody, IDictionary<string, object> headers)
        {
            _builder ??= new OutgoingMessageBuilder(_exchangeName, _routingKey,
                _serializationFormat ?? SerializationFormat.Messagepack);
            _logger ??= new OutgoingMessageLogger(_ignoredMessageTypes, _systemLogger);
            
            var message = _builder.Create<TMessageModel>(messageBody, headers);
            _logger.Log(message);
        }
    }
}
