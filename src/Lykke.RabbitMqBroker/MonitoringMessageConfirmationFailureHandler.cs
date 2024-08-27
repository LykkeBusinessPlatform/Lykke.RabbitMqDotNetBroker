using System;
using System.Threading.Tasks;

using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker
{
    internal sealed class MonitoringMessageConfirmationFailureHandler : IListenerRegistrationHandler
    {
        private readonly IListenerRegistrationHandler _decoratee;

        public MonitoringMessageConfirmationFailureHandler(IListenerRegistrationHandler decoratee)
        {
            _decoratee = decoratee;
        }

        public string Name => nameof(MonitoringMessageConfirmationFailureHandler);

        public Task Handle(IListenerRegistration registration)
        {
            try
            {
                _decoratee.Handle(registration);
            }
            catch (OperationInterruptedException ex)
            {
                throw new InvalidOperationException($"Failed to confirm sending monitoring message: {ex.Message}", ex);
            }
            return Task.CompletedTask;
        }
    }
}