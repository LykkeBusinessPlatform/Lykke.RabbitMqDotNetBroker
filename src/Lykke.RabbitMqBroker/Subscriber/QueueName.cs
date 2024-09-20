using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.RabbitMqBroker.Subscriber;

public record QueueName(string Value)
{
    private static readonly List<string> ProhibitedPrefixes = ["amq.", "amqp."];

    private const int MaxLengthInBytes = 255;
    private const string PoisonQueueSuffix = "poison";

    public static QueueName Create(string value) => value switch
    {
        null => throw new ArgumentNullException(nameof(value), "Value cannot be null."),
        "" => throw new ArgumentException("Value cannot be empty.", nameof(value)),
        _ when ProhibitedPrefixes.Exists(p => value.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)) => throw new ArgumentException($"Value cannot start with {string.Join(", ", ProhibitedPrefixes)}", nameof(value)),
        _ when Encoding.UTF8.GetByteCount(value) > MaxLengthInBytes => throw new ArgumentException($"Value length in bytes cannot exceed {MaxLengthInBytes}", nameof(value)),
        _ => new QueueName(value.Trim())
    };

    public static QueueName CreateUnique(string prefix) => Create($"{prefix}.{Guid.NewGuid()}");

    public static QueueName FromExchangeName(string exchangeName) => CreateUnique(exchangeName);

    public static implicit operator string(QueueName queueName) => queueName.Value;

    public static implicit operator QueueName(string value) => Create(value);

    public QueueName CreatePoisonQueueName() =>
        Value.EndsWith(PoisonQueueSuffix)
            ? Create(Value)
            : Create($"{Value}-{PoisonQueueSuffix}");
}