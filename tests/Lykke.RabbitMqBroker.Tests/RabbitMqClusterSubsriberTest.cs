﻿// Copyright (c) Lykke Corp.
// Licensed under the MIT License. See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.Deserializers;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;
using Lykke.RabbitMqBroker.Subscriber.Middleware.Deduplication;
using Lykke.RabbitMqBroker.Subscriber.Middleware.ErrorHandling;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests
{
    [TestFixture(Category = "Integration")]
    [Explicit]
    public class RabbitMqClusterSubsriberTest
    {
        protected const string ConnectionString = "amqp://[username]:[password]@[URL]:5672/IntegrationTests";
        protected const string AlternativeConnectionString = "amqp://[username]:[password]@[URL]:5672/IntegrationTests";
        protected const string InvalidConnectionString = "amqp://[username]:[password]@[URL]:5672/IntegrationTests";

        protected const string ExchangeName = "TestClusterExchange";
        protected const string QueueName = "TestClusterQueue";

        private RabbitMqPullingSubscriber<string> _pullingSubscriber;
        private int _messagesCount;

        [Test]
        public void ReceivingAndDeduplicationMessagesFromBothExchanges()
        {
            // arrange
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = ConnectionString,
                ExchangeName = ExchangeName,
                IsDurable = true,
                QueueName = QueueName
            };
            _pullingSubscriber = new RabbitMqPullingSubscriber<string>(
                    new NullLogger<RabbitMqPullingSubscriber<string>>(),
                    settings)
                .UseMiddleware(new InMemoryDeduplicationMiddleware<string>())
                .UseMiddleware(new ExceptionSwallowMiddleware<string>(new NullLogger<ExceptionSwallowMiddleware<string>>()))
                .SetMessageDeserializer(new DefaultStringDeserializer())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .SetAlternativeExchange(AlternativeConnectionString)
                .CreateDefaultBinding();

            var handler = new Func<string, Task>(s =>
            {
                _messagesCount++;
                return Task.CompletedTask;
            });
            _pullingSubscriber.Subscribe(handler);
            _pullingSubscriber.Start();

            // act
            {
                var factory = new ConnectionFactory {Uri = new Uri(ConnectionString, UriKind.Absolute)};
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("1"));
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("2"));
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("4"));
                }
            }
            {
                var factory = new ConnectionFactory {Uri = new Uri(AlternativeConnectionString, UriKind.Absolute)};
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("1"));
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("3"));
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("4"));
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            // assert
            Assert.AreEqual(4, _messagesCount);
        }

        [Test]
        public void ReceivingMessagesWhenOneRabbitIsNotReachable()
        {
            // arrange
            var settings = new RabbitMqSubscriptionSettings
            {
                ConnectionString = InvalidConnectionString,
                ExchangeName = ExchangeName,
                IsDurable = true,
                QueueName = QueueName
            };
            _pullingSubscriber = new RabbitMqPullingSubscriber<string>(
                    new NullLogger<RabbitMqPullingSubscriber<string>>(), 
                    settings)
                .UseMiddleware(new InMemoryDeduplicationMiddleware<string>())
                .UseMiddleware(new ExceptionSwallowMiddleware<string>(new NullLogger<ExceptionSwallowMiddleware<string>>()))
                .SetMessageDeserializer(new DefaultStringDeserializer())
                .SetMessageReadStrategy(new MessageReadQueueStrategy())
                .SetAlternativeExchange(AlternativeConnectionString)
                .CreateDefaultBinding();

            var handler = new Func<string, Task>(s =>
            {
                _messagesCount++;
                return Task.CompletedTask;
            });
            _pullingSubscriber.Subscribe(handler);
            _pullingSubscriber.Start();

            // act
            {
                var factory = new ConnectionFactory {Uri = new Uri(AlternativeConnectionString, UriKind.Absolute)};
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("1"));
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("3"));
                    channel.BasicPublish(ExchangeName, "", body: Encoding.UTF8.GetBytes("4"));
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));

            // assert
            Assert.AreEqual(3, _messagesCount);
        }

        [TearDown]
        public void TearDown()
        {
            _pullingSubscriber.Stop();
        }
    }
}