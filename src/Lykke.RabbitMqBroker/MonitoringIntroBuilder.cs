using System;
using System.Text;

namespace Lykke.RabbitMqBroker;

internal sealed class MonitoringIntroBuilder
{
    private readonly StringBuilder _sb = new();

    public MonitoringIntroBuilder()
    {
        _sb.AppendLine($"RabbitMq subscribers monitoring started for {new HostAssemblyDisplayName()}.");
    }

    public MonitoringIntroBuilder AddHeartbeatExpiration(int heartbeatExpirationMs)
    {
        _sb.AppendLine($"Monitoring heartbeat expiration: {heartbeatExpirationMs} ms.");
        return this;
    }

    public MonitoringIntroBuilder AddHeartbeatFairDelay(int? heartbeatFairDelayMs)
    {
        AppendValueOrDefault("Period to consider as honorable infrastructure delay", heartbeatFairDelayMs, "ms");
        return this;
    }

    public MonitoringIntroBuilder AddAnalysisPeriod(TimeSpan? analysisPeriod)
    {
        AppendValueOrDefault("Monitoring analysis period", analysisPeriod);
        return this;
    }

    public MonitoringIntroBuilder AddHeartbeatPublisherConfirmationWaitTimeout(int publishConfirmationWaitTimeoutMs)
    {
        _sb.AppendLine($"Heartbeat publisher confirmation wait timeout: {publishConfirmationWaitTimeoutMs} ms.");
        return this;
    }

    public MonitoringIntroBuilder AddHeartbeatCleanupPeriod(TimeSpan? heartbeatCleanupPeriod)
    {
        AppendValueOrDefault("Monitoring heartbeat cleanup period", heartbeatCleanupPeriod);
        return this;
    }

    public MonitoringIntroBuilder AddHeartbeatRetentionPeriod(TimeSpan? heartbeatRetentionPeriod)
    {
        AppendValueOrDefault("Monitoring heartbeat retention period", heartbeatRetentionPeriod);
        return this;
    }

    private void AppendValueOrDefault<T>(string description, T? value, string unit = "", string defaultValue = "default") where T : struct
    {
        _sb.AppendLine($"{description}: {value?.ToString() ?? defaultValue} {unit}.");
    }

    public string Build() => _sb.ToString();

    public override string ToString() => _sb.ToString();
}
