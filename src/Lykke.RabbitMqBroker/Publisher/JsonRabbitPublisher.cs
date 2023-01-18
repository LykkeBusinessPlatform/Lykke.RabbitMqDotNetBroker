using System;
using System.Threading.Tasks;
using Autofac;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Publisher.Serializers;
using Lykke.RabbitMqBroker.Publisher.Strategies;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Publisher
{
    /// <summary>
    /// Standard implementation for IRabbitPublisher with json serializer.
    /// </summary>
    /// <typeparam name="TMessage">Message type.</typeparam>
    [PublicAPI]
    public class JsonRabbitPublisher<TMessage> : IRabbitPublisher<TMessage>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _connectionString;
        private readonly string _exchangeName;
        private readonly IAutorecoveringConnection _connection;
        private RabbitMqPublisher<TMessage> _rabbitMqPublisher;

        public JsonRabbitPublisher(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IAutorecoveringConnection connection,
            string connectionString,
            string exchangeName)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _exchangeName = exchangeName ?? throw new ArgumentNullException(nameof(exchangeName));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc cref="IStartable.Start"/>
        public void Start()
        {
            var settings = RabbitMqSubscriptionSettings
                .ForPublisher(_connectionString, _exchangeName)
                .MakeDurable();

            _rabbitMqPublisher = new RabbitMqPublisher<TMessage>(_loggerFactory, settings, _connection)
                .SetSerializer(new JsonMessageSerializer<TMessage>())
                .SetPublishStrategy(new FanoutPublishStrategy(settings))
                .PublishSynchronously();
            _rabbitMqPublisher.Start();
        }

        /// <inheritdoc cref="IStartStop.Stop"/>
        public void Stop()
        {
            var rabbitMqPublisher = _rabbitMqPublisher;

            if (_rabbitMqPublisher == null)
                return;

            _rabbitMqPublisher = null;

            rabbitMqPublisher.Stop();
            rabbitMqPublisher.Dispose();
        }

        /// <inheritdoc cref="IDisposable"/>
        public void Dispose()
        {
            Stop();
        }

        /// <inheritdoc cref="IRabbitPublisher{TMessage}"/>
        public async Task PublishAsync(TMessage message)
        {
            if (_rabbitMqPublisher == null)
                throw new InvalidOperationException("Publisher is not started");

            await _rabbitMqPublisher.ProduceAsync(message);
        }
    }
}
