using System.Collections.Generic;

using JetBrains.Annotations;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class QueueDeclarationArgumentsBuilder
{
    private readonly Dictionary<string, object> _arguments = [];

    public QueueDeclarationArgumentsBuilder UseQuorumQueue()
    {
        _arguments[Headers.XQueueType] = "quorum";
        return this;
    }

    public QueueDeclarationArgumentsBuilder UseClassicQueue()
    {
        _arguments.Remove(Headers.XQueueType); // classic is default
        return this;
    }

    public DealLetterExchangeArgumentsBuilder AddDeadLetterExchange(DeadLetterExchangeName exchangeName)
    {
        _arguments[Headers.XDeadLetterExchange] = exchangeName.ToString();
        return new DealLetterExchangeArgumentsBuilder(this, _arguments);
    }

    /// <summary>
    /// Sets the time to live for the queue once there are no consumers.
    /// </summary>
    /// <param name="ttl"></param>
    /// <returns></returns>
    public QueueDeclarationArgumentsBuilder UseTimeToLive(TimeToLive ttl)
    {
        _arguments[Headers.XExpires] = ttl.ToExpirationMilliseconds();
        return this;
    }

    [CanBeNull]
    public Dictionary<string, object> Build()
    {
        return _arguments.Count > 0 ? _arguments : null;
    }
}

internal sealed class DealLetterExchangeArgumentsBuilder
{
    private readonly QueueDeclarationArgumentsBuilder _parentBuilder;
    private readonly Dictionary<string, object> _arguments;

    public DealLetterExchangeArgumentsBuilder(QueueDeclarationArgumentsBuilder parentBuilder, Dictionary<string, object> arguments)
    {
        _parentBuilder = parentBuilder;
        _arguments = arguments;
    }

    public DealLetterExchangeArgumentsBuilder WithAtLeastOnceStrategy()
    {
        _arguments["x-dead-letter-strategy"] = "at-least-once";
        _arguments[Headers.XOverflow] = "reject-publish";
        return this;
    }

    public DealLetterExchangeArgumentsBuilder WithAtMostOnceStrategy()
    {
        _arguments.Remove("x-dead-letter-strategy"); // at-most-once is default
        _arguments.Remove(Headers.XOverflow); // drop-head is default
        return this;
    }

    public Dictionary<string, object> Build() => _parentBuilder.Build();
}
