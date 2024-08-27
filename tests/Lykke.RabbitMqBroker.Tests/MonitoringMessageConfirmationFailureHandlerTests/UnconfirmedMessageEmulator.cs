using System.Threading.Tasks;

using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageConfirmationFailureHandlerTests;

internal sealed class UnconfirmedMessageEmulator : IListenerRegistrationHandler
{
    public string Name => nameof(UnconfirmedMessageEmulator);

    public Task Handle(IListenerRegistration registration)
    {
        // Emulate the situation when the message was not confirmed by the broker
        // and the OperationInterruptedException was thrown as per documentation
        throw new OperationInterruptedException(
            new RabbitMQ.Client.ShutdownEventArgs(RabbitMQ.Client.ShutdownInitiator.Library,
            0,
            "Confirmation from broker was not received within the specified timeout."));
    }
}
