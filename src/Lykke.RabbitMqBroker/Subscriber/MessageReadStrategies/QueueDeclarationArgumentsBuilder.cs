using System.Collections.Generic;

using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class QueueDeclarationArgumentsBuilder
{
    private readonly Dictionary<string, object> _arguments = [];

    public QueueDeclarationArgumentsBuilder UseQuorumQueue()
    {
        _arguments["x-queue-type"] = "quorum";
        return this;
    }

    public QueueDeclarationArgumentsBuilder UseClassicQueue()
    {
        _arguments.Remove("x-queue-type"); // classic is default
        return this;
    }

    public DealLetterExchangeArgumentsBuilder AddDeadLetterExchange(DeadLetterExchangeName exchangeName)
    {
        _arguments["x-dead-letter-exchange"] = exchangeName.ToString();
        return new DealLetterExchangeArgumentsBuilder(this, _arguments);
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
        _arguments["overflow"] = "reject-publish";
        return this;
    }

    public DealLetterExchangeArgumentsBuilder WithAtMostOnceStrategy()
    {
        _arguments.Remove("x-dead-letter-strategy"); // at-most-once is default
        _arguments.Remove("overflow"); // drop-head is default
        return this;
    }

    public Dictionary<string, object> Build() => _parentBuilder.Build();
}
