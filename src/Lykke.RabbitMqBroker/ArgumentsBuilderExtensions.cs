using System;

using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

internal static class ArgumentsBuilderExtensions
{
    public static uint ToExpirationMilliseconds(this double src) => src switch
    {
        > uint.MaxValue => throw new ArgumentOutOfRangeException(nameof(src), "Value is too large to be converted to uint"),
        < uint.MinValue => throw new ArgumentOutOfRangeException(nameof(src), "Value is too small to be converted to uint"),
        _ => (uint)Math.Floor(src)
    };

    public static uint ToExpirationMilliseconds(this TimeToLive src) =>
        src.Value.TotalMilliseconds.ToExpirationMilliseconds();
}