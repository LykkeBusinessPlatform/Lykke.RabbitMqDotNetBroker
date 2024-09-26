using System;
using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record DeadLetterExchangeName(string Value) : ExchangeName(Value)
{
    private const string DeadLetterExchangeSuffix = "dlx";
    public static new DeadLetterExchangeName Create(string value) =>
        value.EndsWith(DeadLetterExchangeSuffix, StringComparison.InvariantCultureIgnoreCase)
            ? new(value)
            : new($"{value}.{DeadLetterExchangeSuffix}");
    public static DeadLetterExchangeName FromExchangeName(ExchangeName exchangeName) => Create(exchangeName.Value);
    public override string ToString() => base.ToString();
}