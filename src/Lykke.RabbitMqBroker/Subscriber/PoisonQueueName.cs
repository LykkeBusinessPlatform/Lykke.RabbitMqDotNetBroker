using System;
using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record PoisonQueueName(string Value) : QueueName(AddSuffix(Value))
{
    private const string PoisonQueueSuffix = "poison";
    public static new PoisonQueueName Create(string value) => new(AddSuffix(value));
    public static bool Is(string value) => value.EndsWith(PoisonQueueSuffix, StringComparison.InvariantCultureIgnoreCase);
    protected static string AddSuffix(string value) => Is(value) ? value : $"{value}-{PoisonQueueSuffix}";

    public static PoisonQueueName FromQueueName(QueueName queueName) => Create(queueName.Value);
    public override string ToString() => base.ToString();
    public static implicit operator string(PoisonQueueName name) => name.ToString();
}
