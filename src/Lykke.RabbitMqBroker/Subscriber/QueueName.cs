using System;
using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record QueueName(string Value) : ResourceName(Validate(Value))
{
    private const string PoisonQueueSuffix = "poison";

    public static QueueName Create(string value) => new(Validate(value));

    public static QueueName CreateUnique(string prefix) => Create($"{prefix}.{Guid.NewGuid()}");

    public static QueueName FromExchangeName(string exchangeName) => CreateUnique(exchangeName);
    public QueueName CreatePoisonQueueName() =>
        Value.EndsWith(PoisonQueueSuffix, StringComparison.InvariantCultureIgnoreCase)
            ? Create(Value)
            : Create($"{Value}-{PoisonQueueSuffix}");
    public override string ToString() => base.ToString();
}
