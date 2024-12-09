using System;

using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

internal static class ArgumentsBuilderExtensions
{
    public static ulong ToExpirationMilliseconds(this double src) => src switch
    {
        > ulong.MaxValue => throw new ArgumentOutOfRangeException(nameof(src), "Value is too large to be converted to ulong"),
        < ulong.MinValue => throw new ArgumentOutOfRangeException(nameof(src), "Value is too small to be converted to ulong"),
        _ => (ulong)Math.Floor(src)
    };

    public static ulong ToExpirationMilliseconds(this TimeToLive src) =>
        src.Value.TotalMilliseconds.ToExpirationMilliseconds();
}