using System;
using System.Collections.Generic;

using Lykke.RabbitMqBroker.Subscriber;
using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using NUnit.Framework;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Tests;

[TestFixture]
public class PoisonQueueHandlerTests
{
    class Connection : IAutorecoveringConnection
    {
        private readonly bool _withEmptyQueues;

        public Connection(bool withEmptyQueues)
        {
            _withEmptyQueues = withEmptyQueues;
        }
        public ushort ChannelMax => throw new NotImplementedException();

        public IDictionary<string, object> ClientProperties => throw new NotImplementedException();

        public ShutdownEventArgs CloseReason => throw new NotImplementedException();

        public AmqpTcpEndpoint Endpoint => throw new NotImplementedException();

        public uint FrameMax => throw new NotImplementedException();

        public TimeSpan Heartbeat => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        public AmqpTcpEndpoint[] KnownHosts => throw new NotImplementedException();

        public IProtocol Protocol => throw new NotImplementedException();

        public IDictionary<string, object> ServerProperties => throw new NotImplementedException();

        public IList<ShutdownReportEntry> ShutdownReport => throw new NotImplementedException();

        public string ClientProvidedName => throw new NotImplementedException();

        public int LocalPort => throw new NotImplementedException();

        public int RemotePort => throw new NotImplementedException();

        public event EventHandler<EventArgs> RecoverySucceeded;
        public event EventHandler<ConnectionRecoveryErrorEventArgs> ConnectionRecoveryError;
        public event EventHandler<ConsumerTagChangedAfterRecoveryEventArgs> ConsumerTagChangeAfterRecovery;
        public event EventHandler<QueueNameChangedAfterRecoveryEventArgs> QueueNameChangeAfterRecovery;
        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<ConnectionBlockedEventArgs> ConnectionBlocked;
        public event EventHandler<ShutdownEventArgs> ConnectionShutdown;
        public event EventHandler<EventArgs> ConnectionUnblocked;

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Abort(ushort reasonCode, string reasonText)
        {
            throw new NotImplementedException();
        }

        public void Abort(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Abort(ushort reasonCode, string reasonText, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Close(ushort reasonCode, string reasonText)
        {
            throw new NotImplementedException();
        }

        public void Close(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Close(ushort reasonCode, string reasonText, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IModel CreateModel() => _withEmptyQueues
                ? new ChannelWithEmptyQueues()
                : new Channel(100);

        public void Dispose()
        {
        }

        public void HandleConnectionBlocked(string reason)
        {
            throw new NotImplementedException();
        }

        public void HandleConnectionUnblocked()
        {
            throw new NotImplementedException();
        }

        public void UpdateSecret(string newSecret, string reason)
        {
            throw new NotImplementedException();
        }
    }

    class ConnectionProvider : IConnectionProvider
    {
        private readonly bool _withEmptyQueues;

        public ConnectionProvider(bool withEmptyQueues)
        {
            _withEmptyQueues = withEmptyQueues;
        }

        public void Dispose()
        {
        }

        public IAutorecoveringConnection GetExclusive(string connectionString, string name = null)
        {
            return new Connection(withEmptyQueues: _withEmptyQueues);
        }

        public IAutorecoveringConnection GetOrCreateShared(string connectionString)
        {
            throw new NotImplementedException();
        }
    }

    class ChannelWithEmptyQueues : IModel
    {
        public int ChannelNumber => throw new NotImplementedException();

        public ShutdownEventArgs CloseReason => throw new NotImplementedException();

        public IBasicConsumer DefaultConsumer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsClosed => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        public ulong NextPublishSeqNo => throw new NotImplementedException();

        public TimeSpan ContinuationTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<BasicAckEventArgs> BasicAcks;
        public event EventHandler<BasicNackEventArgs> BasicNacks;
        public event EventHandler<EventArgs> BasicRecoverOk;
        public event EventHandler<BasicReturnEventArgs> BasicReturn;
        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<FlowControlEventArgs> FlowControl;
        public event EventHandler<ShutdownEventArgs> ModelShutdown;

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Abort(ushort replyCode, string replyText)
        {
            throw new NotImplementedException();
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            throw new NotImplementedException();
        }

        public void BasicCancel(string consumerTag)
        {
            throw new NotImplementedException();
        }

        public void BasicCancelNoWait(string consumerTag)
        {
            throw new NotImplementedException();
        }

        public string BasicConsume(string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            throw new NotImplementedException();
        }

        public BasicGetResult BasicGet(string queue, bool autoAck)
        {
            throw new NotImplementedException();
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, ReadOnlyMemory<byte> body)
        {
            throw new NotImplementedException();
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            throw new NotImplementedException();
        }

        public void BasicRecover(bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicRecoverAsync(bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Close(ushort replyCode, string replyText)
        {
            throw new NotImplementedException();
        }

        public void ConfirmSelect()
        {
            throw new NotImplementedException();
        }

        public uint ConsumerCount(string queue)
        {
            throw new NotImplementedException();
        }

        public IBasicProperties CreateBasicProperties()
        {
            throw new NotImplementedException();
        }

        public IBasicPublishBatch CreateBasicPublishBatch()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclarePassive(string exchange)
        {
        }

        public void ExchangeDelete(string exchange, bool ifUnused)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
        {
            throw new NotImplementedException();
        }

        public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public uint MessageCount(string queue)
        {
            return 0;
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public QueueDeclareOk QueueDeclarePassive(string queue)
        {
            return new QueueDeclareOk(queue, 0, 0);
        }

        public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
        {
            throw new NotImplementedException();
        }

        public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
        {
            throw new NotImplementedException();
        }

        public uint QueuePurge(string queue)
        {
            throw new NotImplementedException();
        }

        public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void TxCommit()
        {
            throw new NotImplementedException();
        }

        public void TxRollback()
        {
            throw new NotImplementedException();
        }

        public void TxSelect()
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms()
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
        {
            throw new NotImplementedException();
        }

        public void WaitForConfirmsOrDie()
        {
            throw new NotImplementedException();
        }

        public void WaitForConfirmsOrDie(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }

    class Channel : IModel
    {
        private int _messageCount;
        public int AcksCount { get; private set; }
        public int NacksCount { get; private set; }
        public int PublishesCount { get; private set; }

        public Channel(int messageCount)
        {
            _messageCount = messageCount;
        }

        public int ChannelNumber => throw new NotImplementedException();

        public ShutdownEventArgs CloseReason => throw new NotImplementedException();

        public IBasicConsumer DefaultConsumer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsClosed => throw new NotImplementedException();

        public bool IsOpen => throw new NotImplementedException();

        public ulong NextPublishSeqNo => throw new NotImplementedException();

        public TimeSpan ContinuationTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<BasicAckEventArgs> BasicAcks;
        public event EventHandler<BasicNackEventArgs> BasicNacks;
        public event EventHandler<EventArgs> BasicRecoverOk;
        public event EventHandler<BasicReturnEventArgs> BasicReturn;
        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<FlowControlEventArgs> FlowControl;
        public event EventHandler<ShutdownEventArgs> ModelShutdown;

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Abort(ushort replyCode, string replyText)
        {
            throw new NotImplementedException();
        }

        public void BasicAck(ulong deliveryTag, bool multiple)
        {
            AcksCount++;
        }

        public void BasicCancel(string consumerTag)
        {
            throw new NotImplementedException();
        }

        public void BasicCancelNoWait(string consumerTag)
        {
            throw new NotImplementedException();
        }

        public string BasicConsume(string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive, IDictionary<string, object> arguments, IBasicConsumer consumer)
        {
            throw new NotImplementedException();
        }

        public BasicGetResult BasicGet(string queue, bool autoAck)
        {
            if (_messageCount == 0)
            {
                return null;
            }

            _messageCount--;
            return new BasicGetResult(0, false, "exchange", "", (uint)_messageCount, null, null);
        }

        public void BasicNack(ulong deliveryTag, bool multiple, bool requeue)
        {
            NacksCount++;
        }

        public void BasicPublish(string exchange, string routingKey, bool mandatory, IBasicProperties basicProperties, ReadOnlyMemory<byte> body)
        {
            PublishesCount++;
        }

        public void BasicQos(uint prefetchSize, ushort prefetchCount, bool global)
        {
            throw new NotImplementedException();
        }

        public void BasicRecover(bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicRecoverAsync(bool requeue)
        {
            throw new NotImplementedException();
        }

        public void BasicReject(ulong deliveryTag, bool requeue)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
        }

        public void Close(ushort replyCode, string replyText)
        {
            throw new NotImplementedException();
        }

        public void ConfirmSelect()
        {
        }

        public uint ConsumerCount(string queue)
        {
            throw new NotImplementedException();
        }

        public IBasicProperties CreateBasicProperties()
        {
            return null;
        }

        public IBasicPublishBatch CreateBasicPublishBatch()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public void ExchangeBind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeBindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclare(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclareNoWait(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeclarePassive(string exchange)
        {
        }

        public void ExchangeDelete(string exchange, bool ifUnused)
        {
            throw new NotImplementedException();
        }

        public void ExchangeDeleteNoWait(string exchange, bool ifUnused)
        {
            throw new NotImplementedException();
        }

        public void ExchangeUnbind(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void ExchangeUnbindNoWait(string destination, string source, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public uint MessageCount(string queue)
        {
            throw new NotImplementedException();
        }

        public void QueueBind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueBindNoWait(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public QueueDeclareOk QueueDeclare(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void QueueDeclareNoWait(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public QueueDeclareOk QueueDeclarePassive(string queue)
        {
            return new QueueDeclareOk(queue, (uint)_messageCount, 0);
        }

        public uint QueueDelete(string queue, bool ifUnused, bool ifEmpty)
        {
            throw new NotImplementedException();
        }

        public void QueueDeleteNoWait(string queue, bool ifUnused, bool ifEmpty)
        {
            throw new NotImplementedException();
        }

        public uint QueuePurge(string queue)
        {
            throw new NotImplementedException();
        }

        public void QueueUnbind(string queue, string exchange, string routingKey, IDictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        public void TxCommit()
        {
            throw new NotImplementedException();
        }

        public void TxRollback()
        {
            throw new NotImplementedException();
        }

        public void TxSelect()
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms()
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public bool WaitForConfirms(TimeSpan timeout, out bool timedOut)
        {
            throw new NotImplementedException();
        }

        public void WaitForConfirmsOrDie()
        {
        }

        public void WaitForConfirmsOrDie(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }

    [Test]
    public void TryPutMessagesBack_WhenThereAreNoMessages_ReturnsEmptyString()
    {
        var sut = new PoisonQueueHandler(
            "connectionString",
            new ConnectionProvider(withEmptyQueues: true),
            new PoisonQueueConsumerConfigurationOptions(
                PoisonQueueName.Create("queue"),
                ExchangeName.Create("exchange"),
                RoutingKey.Empty));

        var result = sut.TryPutMessagesBack();

        Assert.That(result, Is.Empty);
    }

    [Test]
    public void TryPutMessagesBack_WhenThereAre_100_Messages_RequeuesAll()
    {
        var sut = new PoisonQueueHandler(
            "connectionString",
            new ConnectionProvider(withEmptyQueues: false),
            new PoisonQueueConsumerConfigurationOptions(
                PoisonQueueName.Create("queue"),
                ExchangeName.Create("exchange"),
                RoutingKey.Empty));

        var result = sut.TryPutMessagesBack();

        // number of messages mentioned 2 times in the result
        // 1. Initial number of messages
        // 2. Processed number of messages
        Assert.That(CalculateOccurencies(result, "100"), Is.EqualTo(2));

        static int CalculateOccurencies(string s, string pattern) => s.Split(pattern).Length - 1;
    }
}