using System.Collections.Generic;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker.Tests;

/// <summary>
/// Creates a stub for IBasicProperties.
/// Delivery mode is set to 1 by default (non-persistent).
/// </summary>
public class DumbBasicProperties : IBasicProperties
{
    public ushort ProtocolClassId { get; }
    public string ProtocolClassName { get; }

    public DumbBasicProperties()
    {
        DeliveryMode = 1;
    }

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

    public string AppId { get; set; }
    public string ClusterId { get; set; }
    public string ContentEncoding { get; set; }
    public string ContentType { get; set; }
    public string CorrelationId { get; set; }
    public byte DeliveryMode { get; set; }
    public string Expiration { get; set; }
    public IDictionary<string, object> Headers { get; set; }
    public string MessageId { get; set; }
    public bool Persistent { get; set; }
    public byte Priority { get; set; }
    public string ReplyTo { get; set; }
    public PublicationAddress ReplyToAddress { get; set; }
    public AmqpTimestamp Timestamp { get; set; }
    public string Type { get; set; }
    public string UserId { get; set; }
}
