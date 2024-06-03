using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal sealed class QueueDeclarationArgumentsBuilder
{
    private readonly Dictionary<string, object> _arguments = new();

    public QueueDeclarationArgumentsBuilder WithDeadLetterExchange(string exchangeName)
    {
        _arguments.Add("x-dead-letter-exchange", exchangeName);
        return this;
    }

    [CanBeNull]
    public Dictionary<string, object> Build()
    {
        return _arguments.Count > 0 ? _arguments : null;
    }
}
