using System;
using System.Collections.Generic;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Tests.ImmediatePublisherTests;

internal class FakeModel : IModel
{
    public bool PublisherConfirms { get; private set; } = false;

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
        PublisherConfirms = true;
    }

    public uint ConsumerCount(string queue)
    {
        throw new NotImplementedException();
    }

    public IBasicProperties CreateBasicProperties()
    {
        return new FakeProperties();
    }

    public IBasicPublishBatch CreateBasicPublishBatch()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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
    }
}
