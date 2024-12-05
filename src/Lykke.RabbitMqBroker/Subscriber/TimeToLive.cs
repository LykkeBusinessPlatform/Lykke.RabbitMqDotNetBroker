using System;
using System.Diagnostics;

namespace Lykke.RabbitMqBroker.Subscriber;

[DebuggerDisplay("{Value}")]
public record TimeToLive(TimeSpan Value)
{
    public static TimeToLive Infinite { get; } = new(TimeSpan.Zero);
    public static TimeToLive Create(TimeSpan value) => new(value);
    public static TimeToLive Create(long milliseconds) => new(TimeSpan.FromMilliseconds(milliseconds));
    public static TimeToLive OneMillisecond { get; } = Create(TimeSpan.FromMilliseconds(1));
    public static TimeToLive OneSecond { get; } = Create(TimeSpan.FromSeconds(1));
    public static TimeToLive OneMinute { get; } = Create(TimeSpan.FromMinutes(1));
    public static TimeToLive OneHour { get; } = Create(TimeSpan.FromHours(1));

    public bool IsInfinite => Value == TimeSpan.Zero;

}
