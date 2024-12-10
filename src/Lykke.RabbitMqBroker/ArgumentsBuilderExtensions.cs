using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker;

internal static class ArgumentsBuilderExtensions
{
    public static long ToExpirationMilliseconds(this double src)
    {
        var decimalValue = (decimal)src;

        return decimalValue switch
        {
            > long.MaxValue => long.MaxValue,
            <= 0 => 0,
            _ => (long)src
        };
    }

    public static long ToExpirationMilliseconds(this TimeToLive src) =>
        src.Value.TotalMilliseconds.ToExpirationMilliseconds();
}