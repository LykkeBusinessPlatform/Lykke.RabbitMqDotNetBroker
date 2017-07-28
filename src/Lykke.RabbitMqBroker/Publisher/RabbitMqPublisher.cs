﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Common;
using Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Publisher
{

    public interface IRabbitMqSerializer<in TMessageModel>
    {
        byte[] Serialize(TMessageModel model);
    }

    public interface IRabbitMqPublishStrategy
    {
        void Configure(RabbitMqSubscriptionSettings settings, IModel channel);
        void Publish(RabbitMqSubscriptionSettings settings, IModel channel, byte[] data);
    }

    public class RabbitMqPublisher<TMessageModel> : IMessageProducer<TMessageModel>, IStartable, IStopable
    {
        private readonly RabbitMqSubscriptionSettings _settings;
        private readonly Queue<TMessageModel> _items = new Queue<TMessageModel>();
        private Thread _thread;
        private IRabbitMqSerializer<TMessageModel> _serializer;
        private ILog _log;
        private IConsole _console;
        private IRabbitMqPublishStrategy _publishStrategy;

        public RabbitMqPublisher(RabbitMqSubscriptionSettings settings)
        {
            _settings = settings;
        }

        #region Configurator

        public RabbitMqPublisher<TMessageModel> SetSerializer(IRabbitMqSerializer<TMessageModel> serializer)
        {
            _serializer = serializer;
            return this;
        }

        public RabbitMqPublisher<TMessageModel> SetPublishStrategy(IRabbitMqPublishStrategy publishStrategy)
        {
            _publishStrategy = publishStrategy;
            return this;
        }

        public RabbitMqPublisher<TMessageModel> SetLogger(ILog log)
        {
            _log = log;
            return this;
        }

        public RabbitMqPublisher<TMessageModel> SetConsole(IConsole console)
        {
            _console = console;
            return this;
        }

        #endregion

        public Task ProduceAsync(TMessageModel message)
        {
            lock (_items)
                _items.Enqueue(message);
            return Task.FromResult(0);
        }

        public RabbitMqPublisher<TMessageModel> Start()
        {
            if (_publishStrategy == null)
                _publishStrategy = new DefaultFnoutPublishStrategy();
            
            if (_thread == null)
            {
                _thread = new Thread(ConnectionThread);
                _thread.Start();
            }

            return this;
        }

        void IStartable.Start()
        {
            Start();
        }

        void IStopable.Stop()
        {
            var thread = _thread;

            if (thread == null)
                return;

            if (_serializer == null)
                throw new Exception("RabbitMQPublisher serializer is not specified");

            if (_publishStrategy == null)
                _publishStrategy = new DefaultFanoutPublishStrategy();

            _thread = null;
            thread.Join();
        }

        private bool IsStopped()
        {
            return _thread == null;
        }

        private TMessageModel EnqueueMessage()
        {
            lock (_items)
            {
                if (_items.Count > 0)
                    return _items.Dequeue();
            }

            return default(TMessageModel);
        }

        private void ConnectAndWrite()
        {
            var factory = new ConnectionFactory { Uri = _settings.ConnectionString };

            _console?.WriteLine($"{_settings.GetPublisherName()}: trying to connect to {_settings.ConnectionString} ({_settings.GetQueueOrExchangeName()})");

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                _console?.WriteLine($"{_settings.GetPublisherName()}: connected to {_settings.ConnectionString} ({_settings.GetQueueOrExchangeName()})");
                _publishStrategy.Configure(_settings, channel);

                while (true)
                {
                    if (!connection.IsOpen)
                        throw new Exception($"{_settings.GetPublisherName()}: connection to {_settings.ConnectionString} is closed");

                    var message = EnqueueMessage();

                    if (message == null)
                    {
                        if (IsStopped())
                        {
                            _console?.WriteLine($"{_settings.GetPublisherName()}: {_settings.GetPublisherName()} is stopped");
                            return;
                        }

                        Thread.Sleep(300);
                        continue;
                    }

                    var body = _serializer.Serialize(message);
                    _publishStrategy.Publish(_settings, channel, body);
                }
            }
        }

        private void ConnectionThread()
        {
            while (!IsStopped())
            {
                try
                {
                    ConnectAndWrite();
                }
                catch (Exception e)
                {
                    _console?.WriteLine($"{_settings.GetPublisherName()}: ERROR: {e.Message}");
                    _log?.WriteErrorAsync(_settings.GetPublisherName(), "ConnectionThread", "", e).Wait();
                }
            }
        }
    }
}
