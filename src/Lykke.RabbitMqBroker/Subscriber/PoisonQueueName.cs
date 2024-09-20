using System;
using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record PoisonQueueName(string Value) : QueueName(Value)
{
    private const string PoisonQueueSuffix = "poison";
    public static new PoisonQueueName Create(string value) =>
        value.EndsWith(PoisonQueueSuffix, StringComparison.InvariantCultureIgnoreCase)
            ? new(value)
            : new($"{value}-{PoisonQueueSuffix}");

    public static PoisonQueueName FromQueueName(QueueName queueName) => Create(queueName.Value);
    public override string ToString() => base.ToString();
}
