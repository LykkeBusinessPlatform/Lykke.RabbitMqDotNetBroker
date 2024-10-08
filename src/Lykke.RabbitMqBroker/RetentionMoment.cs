using System;

namespace Lykke.RabbitMqBroker;

internal sealed record RetentionMoment(DateTime Value)
{
    public static RetentionMoment From(TimeSpan retentionPeriod, TimeProvider timeProvider)
    {
        if (retentionPeriod <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(retentionPeriod), retentionPeriod, "Retention period must be positive.");
        }

        var now = timeProvider?.GetUtcNow().DateTime ?? throw new ArgumentNullException(nameof(timeProvider));

        try
        {
            return new(now - retentionPeriod);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new RetentionMoment(DateTime.MinValue);
        }
    }

    public static implicit operator DateTime(RetentionMoment retentionMoment) => retentionMoment.Value;
}
