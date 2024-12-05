using System;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class TimteToLiveExtensions
{
    /// <summary>
    /// Converts the original time-to-live to a poison time-to-live
    /// Adds one day to the original time-to-live
    /// </summary>
    /// <param name="ttl"></param>
    /// <returns></returns>
    public static TimeToLive AsPoison(this TimeToLive ttl) => ttl switch
    {
        { IsInfinite: true } => TimeToLive.Infinite,
        _ => TimeToLive.Create(ttl.Value.Add(TimeSpan.FromDays(1)))
    };
}