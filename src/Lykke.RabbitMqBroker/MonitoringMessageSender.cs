using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker
{
    internal sealed class MonitoringMessageSender : IListenerRegistrationHandler
    {
        private readonly IConnectionProvider _connectionProvider;
        private readonly string _connectionString;

        public MonitoringMessageSender(IConnectionProvider connectionProvider, string connectionString)
        {
            _connectionProvider = connectionProvider;
            _connectionString = connectionString;
        }

        public string Name => nameof(MonitoringMessageSender);

        public Task Handle(IListenerRegistration registration)
        {
            using var connection = _connectionProvider.GetExclusive(_connectionString, "Monitoring message sender");
            using var channel = connection.CreateModel();
            channel.ConfirmSelect();
            var props = channel.CreateBasicProperties();
            props.Headers = BuildHeaders(registration);
            props.DeliveryMode = 1;
            props.Expiration = "60000";
            props.MessageId = Guid.NewGuid().ToString();
            props.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            props.Type = "Monitoring";
            channel.BasicReturn += (sender, args) => throw new InvalidOperationException($"Failed to send monitoring message: {args.ReplyText}, route: {registration}");

            channel.BasicPublish(
                exchange: registration.ExchangeName,
                routingKey: registration.RoutingKey ?? string.Empty,
                true,
                basicProperties: props,
                body: ReadOnlyMemory<byte>.Empty);

            try
            {
                channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));
            }
            catch (OperationInterruptedException ex)
            {
                throw new InvalidOperationException($"Monitoring message was not confirmed: {ex.Message}", ex);
            }

            return Task.CompletedTask;
        }

        private static Dictionary<string, object> BuildHeaders(IListenerRegistration registration)
        {
            return new Dictionary<string, object>
            {
                { "ComponentName", new HostAssemblyDisplayName() },
                { "Route", registration },
                { "Version", "1.0" },
                { "Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") },
                { "EnvironmentInfo", Environment.GetEnvironmentVariable("ENV_INFO") },
                { "Host", Environment.MachineName },
            };
        }
    }
}