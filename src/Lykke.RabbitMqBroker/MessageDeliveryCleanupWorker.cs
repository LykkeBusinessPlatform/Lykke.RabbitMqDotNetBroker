using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker;

internal sealed class MessageDeliveryCleanupWorker : IMessageDeliveryCleanupWorker
{
    private readonly IMessageDeliveryMaintenance _messageStorage;
    public MessageDeliveryCleanupWorker(IMessageDeliveryMaintenance messageDeliveryMaintenance)
    {
        _messageStorage = messageDeliveryMaintenance;
    }

    public async Task Execute()
    {

    }
}
