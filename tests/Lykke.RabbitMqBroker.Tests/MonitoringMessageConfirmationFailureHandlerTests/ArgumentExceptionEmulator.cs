using System;
using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageConfirmationFailureHandlerTests;

internal sealed class ArgumentExceptionEmulator : IListenerRegistrationHandler
{
    public string Name => nameof(ArgumentExceptionEmulator);

    public Task Handle(IListenerRegistration registration)
    {
        throw new ArgumentException("Some unexpected exception");
    }
}
