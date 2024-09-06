using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Tests.Fakes;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using NSubstitute;

using NUnit.Framework;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture]
    public class RabbitMqSubscriberTests
    {
        private RabbitMqSubscriber<StubMessage> _subscriber;
        private FakeConnection _connection;
        private ILogger<RabbitMqSubscriber<StubMessage>> _logger;
        private RabbitMqSubscriptionSettings _settings;
        private IMessageDeserializer<StubMessage> _messageDeserializer;
        private IMessageReadStrategy _messageReadStrategy;
        private Func<StubMessage, Task> _eventHandler;
        private Func<StubMessage, CancellationToken, Task> _cancellableEventHandler;

        [SetUp]
        public void SetUp()
        {
            _logger = NullLogger<RabbitMqSubscriber<StubMessage>>.Instance;
            _settings = new RabbitMqSubscriptionSettings { QueueName = "test-queue", ExchangeName = "test-exchange" };
            _connection = new FakeConnection();
            _subscriber = new RabbitMqSubscriber<StubMessage>(_logger, _settings, _connection);
        }

        [Test]
        public void Test_SetMessageDeserializer_ShouldSetDeserializer()
        {
            _messageDeserializer = Substitute.For<IMessageDeserializer<StubMessage>>();
            _subscriber.SetMessageDeserializer(_messageDeserializer);
            Assert.That(_subscriber.MessageDeserializer, Is.EqualTo(_messageDeserializer));
        }

        [Test]
        public void Test_Subscribe_EventHandler_ShouldSetEventHandler()
        {
            _eventHandler = Substitute.For<Func<StubMessage, Task>>();
            _subscriber.Subscribe(_eventHandler);
            Assert.That(_subscriber.EventHandler, Is.EqualTo(_eventHandler));
        }

        [Test]
        public void Test_Subscribe_CancellableEventHandler_ShouldSetCancellableEventHandler()
        {
            _cancellableEventHandler = Substitute.For<Func<StubMessage, CancellationToken, Task>>();
            _subscriber.Subscribe(_cancellableEventHandler);
            Assert.That(_subscriber.CancellableEventHandler, Is.EqualTo(_cancellableEventHandler));
        }

        [Test]
        public void Test_SetMessageReadStrategy_ShouldSetMessageReadStrategy()
        {
            _messageReadStrategy = Substitute.For<IMessageReadStrategy>();
            _subscriber.SetMessageReadStrategy(_messageReadStrategy);
            Assert.That(_subscriber.MessageReadStrategy, Is.EqualTo(_messageReadStrategy));
        }

        [Test]
        public void Test_Start_ShouldThrowException_WhenNoMessageDeserializer()
        {
            Assert.Throws<InvalidOperationException>(() => _subscriber.Start());
        }

        [Test]
        public void Test_Start_ShouldThrowException_WhenNoHandler()
        {
            _messageDeserializer = Substitute.For<IMessageDeserializer<StubMessage>>();
            _subscriber.SetMessageDeserializer(_messageDeserializer);
            Assert.Throws<InvalidOperationException>(() => _subscriber.Start());
        }

        [Test]
        public void Test_Start_ShouldUseDefaultStrategy_WhenNoStrategyProvided()
        {
            _messageDeserializer = Substitute.For<IMessageDeserializer<StubMessage>>();
            _eventHandler = Substitute.For<Func<StubMessage, Task>>();
            _subscriber
                .SetMessageDeserializer(_messageDeserializer)
                .Subscribe(_eventHandler);

            _subscriber.Start();

            Assert.That(_subscriber.MessageReadStrategy, Is.Not.Null);
        }

        [Test]
        public void Test_Start_ShouldConfigureChannelPrefetch_WhenPrefetchCountIsSet()
        {
            ushort prefetchCount = 10;

            _messageDeserializer = Substitute.For<IMessageDeserializer<StubMessage>>();
            _eventHandler = Substitute.For<Func<StubMessage, Task>>();
            _subscriber
                .SetMessageDeserializer(_messageDeserializer)
                .SetPrefetchCount(prefetchCount)
                .Subscribe(_eventHandler);

            _subscriber.Start();

            var consumerChannel = _connection.Channels.SingleOrDefault(x => x?.PrefetchCount == prefetchCount);
            Assert.That(consumerChannel, Is.Not.Null);
        }


        [TearDown]
        public void TearDown()
        {
            _subscriber?.Stop();
            _subscriber?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _subscriber?.Stop();
            _subscriber?.Dispose();

            _connection?.Dispose();
        }
    }
}
