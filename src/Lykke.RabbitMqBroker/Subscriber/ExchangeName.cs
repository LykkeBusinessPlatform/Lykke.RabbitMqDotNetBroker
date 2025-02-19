using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record ExchangeName(string Value) : ResourceName(Validate(Value))
{
    public static ExchangeName Create(string value) => new(value);
    public override string ToString() => base.ToString();
    public static implicit operator string(ExchangeName name) => name.ToString();
}