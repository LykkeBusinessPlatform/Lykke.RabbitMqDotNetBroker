// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Publisher.Serializers;
using Lykke.RabbitMqBroker.Publisher.Strategies;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Publisher;

public sealed class RabbitMqSlimPublisher<TMessageModel> : IStartStop, IMessageProducer<TMessageModel>
{
    private readonly RabbitMqSubscriptionSettings _settings;
    private readonly ILogger<RabbitMqSlimPublisher<TMessageModel>> _log;
    private readonly List<Func<IDictionary<string, object>>> _writeHeadersFunсs = new();
    private readonly IAutorecoveringConnection _connection;
    
    private IRabbitMqPublishStrategy _publishStrategy;
    private IRabbitMqSerializer<TMessageModel> _serializer;
    private IModel _channel;
    
    public string Name => _settings.GetPublisherName();
    
    public RabbitMqSlimPublisher(
        [NotNull] ILoggerFactory loggerFactory,
        [NotNull] RabbitMqSubscriptionSettings settings,
        [NotNull] IAutorecoveringConnection connection)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _log = loggerFactory.CreateLogger<RabbitMqSlimPublisher<TMessageModel>>();
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }
    public void Start()
    {
        if (_serializer == null)
        {
            throw new InvalidOperationException(
                $"Please, setup message serializer, using {nameof(SetSerializer)}() method, before start publisher");
        }

        if (_publishStrategy == null)
        {
            SetPublishStrategy(new FanoutPublishStrategy(_settings));
        }
        
        _channel ??= _connection.CreateModel();
        _publishStrategy!.Configure(_channel);
    }

    public void Stop()
    {
        if (_channel == null) return;
        
        _channel.Close();
        _channel.Dispose();
        _channel = null;
    }

    public Task ProduceAsync(TMessageModel message, string routingKey)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (_channel == null)
        {
            throw new InvalidOperationException("Publisher is not started. Call Start() method before use it.");
        }

        var body = _serializer.Serialize(message);
        var headers = GetMessageHeaders();
        var rawMessage = new RawMessage(body, routingKey, headers);
        _publishStrategy.Publish(_channel, rawMessage);
        
        return Task.CompletedTask;
    }

    public Task ProduceAsync(TMessageModel message)
    {
        return ProduceAsync(message, null);
    }
    
    public RabbitMqSlimPublisher<TMessageModel> SetSerializer(IRabbitMqSerializer<TMessageModel> serializer)
    {
        _serializer = serializer ?? throw new InvalidOperationException("Serializer can not be null");
        return this;
    }
    
    public RabbitMqSlimPublisher<TMessageModel> SetPublishStrategy(IRabbitMqPublishStrategy publishStrategy)
    {
        _publishStrategy = publishStrategy ?? throw new InvalidOperationException("Publish strategy can not be null");
        return this;
    }
    
    public RabbitMqSlimPublisher<TMessageModel> SetWriteHeadersFunc(Func<IDictionary<string, object>> func)
    {
        if (func != null)
        {
            _writeHeadersFunсs.Add(func);
        }
        return this;
    }
    
    private IDictionary<string, object> GetMessageHeaders()
    {
        var result = new Dictionary<string, object>();

        var keyValuePairs = _writeHeadersFunсs
            .Select(x => x())
            .Where(x => x != null && x.Any())
            .SelectMany(x => x)
            .ToList();

        if (!keyValuePairs.Any()) return result;
        
        foreach (var keyValuePair in keyValuePairs)
        {
            var added = result.TryAdd(keyValuePair.Key, keyValuePair.Value);
            if (!added)
            {
                _log.LogError($"Header with key '{keyValuePair.Key}' already exists. Discarded value is '${keyValuePair.Value}'. Please, use unique headers only.");
            }
        }

        return result;
    }

    public void Dispose()
    {
        Stop();
    }
}
