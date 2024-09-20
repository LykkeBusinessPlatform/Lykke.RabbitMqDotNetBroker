using System;
using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record QueueName(string Value) : ResourceName(Validate(Value))
{
    public static QueueName Create(string value) => new(Validate(value));
    public static QueueName CreateUnique(string prefix) => Create($"{prefix}.{Guid.NewGuid()}");
    public static QueueName FromExchangeName(string exchangeName) => CreateUnique(exchangeName);
    public override string ToString() => base.ToString();
}
