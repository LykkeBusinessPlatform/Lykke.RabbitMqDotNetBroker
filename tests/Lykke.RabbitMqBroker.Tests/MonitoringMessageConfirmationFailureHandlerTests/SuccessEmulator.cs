using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageConfirmationFailureHandlerTests;

internal sealed class SuccessEmulator : IListenerRegistrationHandler
{
    public string Name => nameof(SuccessEmulator);

    public Task Handle(IListenerRegistration registration)
    {
        return Task.CompletedTask;
    }
}
