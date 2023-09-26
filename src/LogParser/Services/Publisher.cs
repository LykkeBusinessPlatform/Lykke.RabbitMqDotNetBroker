using System;
using LogParser.Configuration;
using Lykke.RabbitMqBroker.Logging;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace LogParser.Services
{
    public sealed class Publisher : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _publishingChannel;
        private readonly ILogger<Publisher> _logger;
        private readonly Configuration.Configuration _configuration;

        public Publisher(ILogger<Publisher> logger, Configuration.Configuration configuration)
        {
            _logger = logger;

            _configuration = configuration;
            var factory = new ConnectionFactory
            {
                HostName = _configuration.RabbitConfig.HostName,
                Port = Convert.ToInt32(_configuration.RabbitConfig.Port),
                UserName = configuration.RabbitConfig.UserName,
                Password = configuration.RabbitConfig.Password,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(60)
            };

            _connection = factory.CreateConnection();
            _publishingChannel = _connection.CreateModel();
        }

        public void Publish(OutgoingMessage outgoingMessage)
        {
            var properties = _publishingChannel.CreateBasicProperties();
            properties.Type = outgoingMessage.MessageTypeName;
            properties.Headers = outgoingMessage.Headers;

            var message = Convert.FromBase64String(outgoingMessage.Message);

            _publishingChannel.BasicReturn += (sender, args) =>
            {
                // not sure if we can use published message body here
                string? returnedMessage = null;
                if (args.Body.Length > 0)
                {
                    returnedMessage = Convert.ToBase64String(args.Body.ToArray());
                }

                var returnedMessageString = returnedMessage ?? "empty";
                _logger.LogError(
                    "Message was not published. Reason: {ReplyText}, code: {ReplyCode}, body in base64: {Message)}",
                    args.ReplyText,
                    args.ReplyCode,
                    returnedMessageString);
            };

            try
            {
                _publishingChannel.BasicPublish(outgoingMessage.Exchange,
                    outgoingMessage.RoutingKey,
                    true,
                    properties,
                    message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error: {e.Message}", e.Message);
                throw;
            }
        }

        public void Dispose()
        {
            _publishingChannel?.Close();
            _publishingChannel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();
        }
    }
}
