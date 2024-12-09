using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

internal static class ArgumentsBuilderExtensions
{
    public static ulong ToExpirationMilliseconds(this double src)
    {
        var decimalValue = (decimal)src;

        return decimalValue switch
        {
            > ulong.MaxValue => ulong.MaxValue,
            < ulong.MinValue => ulong.MinValue,
            _ => (ulong)src
        };
    }

    public static ulong ToExpirationMilliseconds(this TimeToLive src) =>
        src.Value.TotalMilliseconds.ToExpirationMilliseconds();
}