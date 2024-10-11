using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

internal sealed class MessageDeliveryAnalysisWorker : IMessageDeliveryAnalysisWorker
{
    private readonly IMessageDeliveryStorage _messageDeliveryStorage;
    private readonly IMonitoringIssueNotifier _monitoringIssueNotifier;

    public MessageDeliveryAnalysisWorker(
        IMessageDeliveryStorage messageStorage,
        IMonitoringIssueNotifier monitoringIssueNotifier)
    {
        _messageDeliveryStorage = messageStorage;
        _monitoringIssueNotifier = monitoringIssueNotifier;
    }

    public async Task Execute()
    {
        await foreach (var message in _messageDeliveryStorage.GetLatestForEveryRoute())
        {
            if (message.NotDelivered())
            {
                await _monitoringIssueNotifier.Notify(message);
            }
        }
    }
}
