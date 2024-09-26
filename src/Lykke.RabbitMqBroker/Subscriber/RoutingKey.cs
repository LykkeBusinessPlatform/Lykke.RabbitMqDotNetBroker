using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record RoutingKey(string Value)
{
    public static RoutingKey Create(string value) =>
        value switch
        {
            _ when string.IsNullOrWhiteSpace(value) => Empty,
            _ => new(value)
        };
    public static RoutingKey Empty => new(string.Empty);
    public override string ToString() => Value;
}
