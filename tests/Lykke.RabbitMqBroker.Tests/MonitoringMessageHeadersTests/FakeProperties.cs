using System.Collections.Generic;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests.MonitoringMessageHeadersTests;

internal sealed class FakeProperties : IBasicProperties
{
    public string AppId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string ClusterId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string ContentEncoding { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string ContentType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string CorrelationId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public byte DeliveryMode { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string Expiration { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public IDictionary<string, object> Headers { get; set; }
    public string MessageId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool Persistent { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public byte Priority { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string ReplyTo { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public PublicationAddress ReplyToAddress { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public AmqpTimestamp Timestamp { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string Type { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string UserId { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public ushort ProtocolClassId => throw new System.NotImplementedException();

    public string ProtocolClassName => throw new System.NotImplementedException();

    public void ClearAppId()
    {
        throw new System.NotImplementedException();
    }

    public void ClearClusterId()
    {
        throw new System.NotImplementedException();
    }

    public void ClearContentEncoding()
    {
        throw new System.NotImplementedException();
    }

    public void ClearContentType()
    {
        throw new System.NotImplementedException();
    }

    public void ClearCorrelationId()
    {
        throw new System.NotImplementedException();
    }

    public void ClearDeliveryMode()
    {
        throw new System.NotImplementedException();
    }

    public void ClearExpiration()
    {
        throw new System.NotImplementedException();
    }

    public void ClearHeaders()
    {
        throw new System.NotImplementedException();
    }

    public void ClearMessageId()
    {
        throw new System.NotImplementedException();
    }

    public void ClearPriority()
    {
        throw new System.NotImplementedException();
    }

    public void ClearReplyTo()
    {
        throw new System.NotImplementedException();
    }

    public void ClearTimestamp()
    {
        throw new System.NotImplementedException();
    }

    public void ClearType()
    {
        throw new System.NotImplementedException();
    }

    public void ClearUserId()
    {
        throw new System.NotImplementedException();
    }

    public bool IsAppIdPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsClusterIdPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsContentEncodingPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsContentTypePresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsCorrelationIdPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsDeliveryModePresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsExpirationPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsHeadersPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsMessageIdPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsPriorityPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsReplyToPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsTimestampPresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsTypePresent()
    {
        throw new System.NotImplementedException();
    }

    public bool IsUserIdPresent()
    {
        throw new System.NotImplementedException();
    }

}