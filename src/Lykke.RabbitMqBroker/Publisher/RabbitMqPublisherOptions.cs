using System;

namespace Lykke.RabbitMqBroker.Publisher;

public class RabbitMqPublisherOptions<T> where T : class
{
    /// <summary>
    /// Instructs the publisher to request shared connection 
    /// if possible instead of creating a new one.
    /// If there is no shared connection available, a new one 
    /// will be created.
    /// </summary>
    public bool ShareConnection { get; set; } = true;

    /// <summary>
    /// Connection name to be displayed in dashboard.
    /// Note: When connection is shared single client can't change the display name.
    /// </summary>
    public string ConnectionNameWhenExclusive { get; set; } = string.Empty;

    /// <summary>
    /// Activates RabbitMQ extension to AMQP 0-9-1 protocol called "Publisher Confirms".
    /// It is about realiable publishing which means that the broker will confirm
    /// the message delivery to the exchange.
    /// </summary>
    public bool PublisherConfirmsEnabled { get; set; } = false;

    /// <summary>
    /// This option makes sense only if PublisherConfirmsEnabled is set to true.
    /// It is the time to synchronously wait for the confirmation from the broker.
    /// If publisher implements asynchronous confirmation strategy, this option
    /// will be ignored.
    /// </summary>
    public TimeSpan? ConfirmationTimeout { get; set; } = null;

    /// <summary>
    /// Activates AMQP 0-9-1 protocol feature with same name.
    /// If message can't be routed to any queue, it will be returned to the publisher.
    /// </summary>
    public bool Mandatory { get; set; } = false;

    public uint MessageExpirationMs { get; set; } = 0;

    public void CopyFrom<TModel>(RabbitMqPublisherOptions<TModel> source) where TModel : class
    {
        ShareConnection = source.ShareConnection;
        PublisherConfirmsEnabled = source.PublisherConfirmsEnabled;
        ConfirmationTimeout = source.ConfirmationTimeout;
        Mandatory = source.Mandatory;
        MessageExpirationMs = source.MessageExpirationMs;
        ConnectionNameWhenExclusive = source.ConnectionNameWhenExclusive;
    }
}