// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker.Logging
{
    /// <summary>
    /// Composite class that builds and logs outgoing messages.
    /// </summary>
    internal sealed class OutgoingMessageLogPersister : IOutgoingMessagePersister
    {
        private sealed record Configuration
        {
            public string ExchangeName { get; init; }
            public string RoutingKey { get; init; }
            public HashSet<string> IgnoredMessageTypes { get; init; }
            public SerializationFormat SerializationFormat { get; init; }
        }
            
        [CanBeNull] private OutgoingMessageBuilder _builder;
        [CanBeNull] private ILogger _systemLogger;

        private Configuration _configuration;
        
        private const SerializationFormat DefaultSerializationFormat = SerializationFormat.Unknown;

        public OutgoingMessageLogPersister()
        {
            _configuration = new Configuration
            {
                ExchangeName = null,
                RoutingKey = null,
                IgnoredMessageTypes = null,
                SerializationFormat = DefaultSerializationFormat
            };
        }

        public IOutgoingMessagePersister SetExchangeName(string exchangeName)
        {
            if (_configuration.ExchangeName != null)
                throw new InvalidOperationException("Exchange name can't be changed after it was set.");
            
            _configuration = _configuration with { ExchangeName = exchangeName };

            return this;
        }

        public IOutgoingMessagePersister SetRoutingKey(string routingKey)
        {
            if (_configuration.RoutingKey != null)
                throw new InvalidOperationException("Routing key can't be changed after it was set.");
            
            _configuration = _configuration with { RoutingKey = routingKey };
            
            return this;
        }

        public IOutgoingMessagePersister SetIgnoredMessageTypes(IEnumerable<string> ignoredMessageTypes)
        {
            if (_configuration.IgnoredMessageTypes != null)
                throw new InvalidOperationException("Ignored message types can't be changed after it was set.");
            
            if (ignoredMessageTypes == null)
                throw new ArgumentNullException(nameof(ignoredMessageTypes));
            
            _configuration = _configuration with { IgnoredMessageTypes = ignoredMessageTypes.ToHashSet() };
            
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
            if (_configuration.SerializationFormat != DefaultSerializationFormat)
                throw new InvalidOperationException("Serialization format can't be changed after it was set.");
            
            _configuration = _configuration with { SerializationFormat = format };

            return this;
        }

        public void Persist<TMessageModel>(byte[] messageBody, IDictionary<string, object> headers)
        {
            if (_systemLogger == null)
                throw new InvalidOperationException("System logger is not set.");
            
            if (_configuration.IgnoredMessageTypes.Contains(typeof(TMessageModel).Name))
                return;
            
            _builder ??= new OutgoingMessageBuilder(_configuration.ExchangeName, _configuration.RoutingKey,
                _configuration.SerializationFormat);

            var message = _builder.Create<TMessageModel>(messageBody, headers);
            _systemLogger.LogInformation(message.ToString());
        }
    }
}
