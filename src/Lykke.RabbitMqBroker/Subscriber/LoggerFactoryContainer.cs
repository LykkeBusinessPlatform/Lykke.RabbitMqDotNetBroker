using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Lykke.RabbitMqBroker.Subscriber;

/// <summary>
/// Class for internal usage in static methods
/// </summary>
internal static class LoggerFactoryContainer
{
    private static ILoggerFactory s_instance;
    public static ILoggerFactory Instance
    {
        get {return s_instance ?? NullLoggerFactory.Instance;}
        set => s_instance ??= value;
    }
}