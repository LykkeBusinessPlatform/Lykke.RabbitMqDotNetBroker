using System;
using System.Threading.Tasks;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker
{
    internal sealed class MonitoringMessageSender : IListenerRegistrationHandler
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly IMonitoringMessagePropertiesFactory _propertiesFactory;
        private readonly string _connectionString;
        private readonly TimeSpan _confirmationWaitTimeout;

        public MonitoringMessageSender(
            IConnectionProvider connectionProvider,
            IMonitoringMessagePropertiesFactory propertiesFactory,
            string connectionString,
            TimeSpan confirmationWaitTimeout)
        {
            _connectionProvider = connectionProvider;
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _confirmationWaitTimeout = confirmationWaitTimeout;
            _propertiesFactory = propertiesFactory;
        }

        public string Name => nameof(MonitoringMessageSender);

        public Task Handle(IListenerRegistration registration)
        {
            using var connection = CreateConnection();
            using var channel = CreateChannel(connection);
            var properties = _propertiesFactory.Create(channel, registration.MessageRoute);
            channel.BasicPublish(
                exchange: registration.ExchangeName,
                routingKey: registration.RoutingKey ?? string.Empty,
                mandatory: true,
                basicProperties: properties,
                body: ReadOnlyMemory<byte>.Empty
            );
            channel.WaitForConfirmsOrDie(_confirmationWaitTimeout);

            return Task.CompletedTask;
        }

        private IConnection CreateConnection()
        {
            var connection = _connectionProvider.GetExclusive(_connectionString, nameof(MonitoringMessageSender));
            return connection;
        }

        private static IModel CreateChannel(IConnection connection)
        {
            var channel = connection.CreateModel();
            channel.ConfirmSelect();
            channel.BasicReturn += (sender, args) =>
            {
                throw new InvalidOperationException($"Monitoring essage was returned: {args.ReplyText}");
            };
            return channel;
        }
    }
}