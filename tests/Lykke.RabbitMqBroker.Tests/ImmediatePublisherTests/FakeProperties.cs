using System;
using System.Collections.Generic;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.ImmediatePublisherTests;

internal class FakeProperties : IBasicProperties
{
    public string AppId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ClusterId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ContentEncoding { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string CorrelationId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public byte DeliveryMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Expiration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public IDictionary<string, object> Headers { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string MessageId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool Persistent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public byte Priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string ReplyTo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public PublicationAddress ReplyToAddress { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public AmqpTimestamp Timestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public ushort ProtocolClassId => throw new NotImplementedException();

    public string ProtocolClassName => throw new NotImplementedException();

    public void ClearAppId()
    {
        throw new NotImplementedException();
    }

    public void ClearClusterId()
    {
        throw new NotImplementedException();
    }

    public void ClearContentEncoding()
    {
        throw new NotImplementedException();
    }

    public void ClearContentType()
    {
        throw new NotImplementedException();
    }

    public void ClearCorrelationId()
    {
        throw new NotImplementedException();
    }

    public void ClearDeliveryMode()
    {
        throw new NotImplementedException();
    }

    public void ClearExpiration()
    {
        throw new NotImplementedException();
    }

    public void ClearHeaders()
    {
        throw new NotImplementedException();
    }

    public void ClearMessageId()
    {
        throw new NotImplementedException();
    }

    public void ClearPriority()
    {
        throw new NotImplementedException();
    }

    public void ClearReplyTo()
    {
        throw new NotImplementedException();
    }

    public void ClearTimestamp()
    {
        throw new NotImplementedException();
    }

    public void ClearType()
    {
        throw new NotImplementedException();
    }

    public void ClearUserId()
    {
        throw new NotImplementedException();
    }

    public bool IsAppIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsClusterIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsContentEncodingPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsContentTypePresent()
    {
        throw new NotImplementedException();
    }

    public bool IsCorrelationIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsDeliveryModePresent()
    {
        throw new NotImplementedException();
    }

    public bool IsExpirationPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsHeadersPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsMessageIdPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsPriorityPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsReplyToPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsTimestampPresent()
    {
        throw new NotImplementedException();
    }

    public bool IsTypePresent()
    {
        throw new NotImplementedException();
    }

    public bool IsUserIdPresent()
    {
        throw new NotImplementedException();
    }
}
