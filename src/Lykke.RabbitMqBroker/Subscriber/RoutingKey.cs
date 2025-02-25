using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record RoutingKey(string Value)
{
    public bool IsEmpty => this == Empty;
    public static RoutingKey Create(string value) =>
        value switch
        {
            _ when string.IsNullOrWhiteSpace(value) => Empty,
            _ => new(value)
        };
    public static RoutingKey Empty => new(string.Empty);
    public override string ToString() => Value;
    public static implicit operator string(RoutingKey key) => key.ToString();
}
