using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lykke.RabbitMqBroker;

internal sealed class MonitoringIntroDisplay : IHostedService
{
    private readonly ILogger<MonitoringIntroDisplay> _logger;
    private readonly RabbitMqMonitoringConfiguration _configuration;
    private readonly IListenersRegistry _listenersRegistry;

    public MonitoringIntroDisplay(
        ILogger<MonitoringIntroDisplay> logger,
        RabbitMqMonitoringConfiguration configuration,
        IListenersRegistry listenersRegistry)
    {
        _logger = logger;
        _configuration = configuration;
        _listenersRegistry = listenersRegistry;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(BuildMonitoringConfigurationIntro());
        _logger.LogInformation(BuildListenersIntro());

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private string BuildMonitoringConfigurationIntro() =>
        new MonitoringIntroBuilder()
            .AddHeartbeatExpiration(_configuration.MessageExpirationMs)
            .AddHeartbeatFairDelay(_configuration.MessageDeliveryFairDelayMs)
            .AddHeartbeatPublisherConfirmationWaitTimeout(_configuration.PublishConfirmationWaitTimeoutMs)
            .AddHeartbeatCleanupPeriod(_configuration.MessagesCleanupPeriod)
            .AddHeartbeatRetentionPeriod(_configuration.MessageRetentionPeriod)
            .AddAnalysisPeriod(_configuration.AnalysisPeriod)
            .Build();

    private string BuildListenersIntro() =>
        string.Join(
            Environment.NewLine,
            "List of queues being monitored:",
            BuildMonitoredQueues()
        );

    private string BuildMonitoredQueues() =>
        string.Join(Environment.NewLine, _listenersRegistry.Select(l => $"Queue: {l.ListenerRoute.QueueName}"));
}
