using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Publisher;

/// <summary>
/// Basic implementation of the RabbitMq message publisher.
/// Publishes messages immediately without any buffering.
/// "Publisher confirms" can be enabled to ensure that the 
/// message is delivered to the broker. In this case publisher
/// operates in synchronous mode which means that it
/// waits for the confirmation from the broker.
/// If the message is not confirmed, an exception 
/// <see cref="OperationInterruptedException"/> is thrown.
/// 
/// Confirmation timeout can be set to wait for the confirmation.
/// If it is not provided the default value (5 sec) is used.
/// 
/// If the message is mandatory, the <see cref="ReturnedEventHandler"/>
/// event is provided to handle unrouted messages.
/// 
/// Dedicated channel is used for each publisher. This is a way 
/// for further impovement. 
/// 
/// This publisher is not thread-safe.
/// 
/// Connection can be shared which depends on the options.
/// </summary>
/// <param name="connectionProvider"></param>
/// <param name="optionsAccessor"></param>
/// <param name="settings"></param>
public sealed class ImmediatePublisher<T>(
    IConnectionProvider connectionProvider,
    IOptions<RabbitMqPublisherOptions<T>> optionsAccessor,
    RabbitMqSubscriptionSettings settings) : IPurePublisher<T>, IDisposable
    where T : class
{
    private const int DefaultConfirmationTimeoutMs = 5_000;
    private readonly IConnectionProvider _connectionProvider = connectionProvider;
    private readonly RabbitMqSubscriptionSettings _settings = settings;
    private readonly RabbitMqPublisherOptions<T> _options = optionsAccessor?.Value;
    private readonly TimeSpan _actualConfirmationTimeout =
        optionsAccessor?.Value?.ConfirmationTimeout ?? TimeSpan.FromMilliseconds(DefaultConfirmationTimeoutMs);

    // Thread-unsafe fields
    private IModel _channel;
    private bool _initialized = false;

    public event AsyncEventHandler<BasicReturnEventArgs> ReturnedEventHandler;

    public void Dispose()
    {
        if (_channel == null)
            return;

        try
        {
            if (_channel.IsOpen)
            {
                _channel.Close(200, "Goodbye");
            }
        }
        catch (Exception)
        {
            // here we are if only the channel is not closed properly
            // we can't do anything here
        }

        _channel.Dispose();
    }

    /// <summary>
    /// Publish message immediately.
    /// </summary>
    /// <param name="body"></param>
    /// <param name="configurator">Callback to configure properties</param>
    /// <param name="exchangeName">Exchange name, takes precedence over the settings</param>
    /// <param name="routingKey">Routing key, takes precedence over the settings</param>
    public Task Publish(
        ReadOnlyMemory<byte> body,
        Action<IBasicProperties> configurator = null,
        string exchangeName = null,
        string routingKey = null)
    {
        if (!_initialized)
            Initialize();

        var properties = ConfigureProperties(configurator);

        _channel.BasicPublish(
            exchange: exchangeName ?? _settings.ExchangeName,
            routingKey: (routingKey ?? _settings.RoutingKey) ?? string.Empty,
            mandatory: _options.Mandatory,
            basicProperties: properties,
            body: body);

        if (_options.PublisherConfirmsEnabled)
        {
            _channel.WaitForConfirmsOrDie(_actualConfirmationTimeout);
        }

        return Task.CompletedTask;
    }

    private void Initialize()
    {
        _channel = CreateConnection().CreateModel();
        ConfigureChannel();

        _initialized = true;
    }

    private void ConfigureChannel()
    {
        if (_options.PublisherConfirmsEnabled)
        {
            _channel.ConfirmSelect();
        }

        _channel.BasicReturn += OnBasicReturn;
        _channel.ModelShutdown += OnModelShutdown;
    }

    private IBasicProperties ConfigureProperties(Action<IBasicProperties> callerConfigurator)
    {
        var properties = _channel.CreateBasicProperties();

        if (_options.MessageExpirationMs > 0)
        {
            properties.Expiration = _options.MessageExpirationMs.ToString();
        }

        // let the caller configure the properties and override the defaults
        callerConfigurator?.Invoke(properties);

        return properties;
    }

    private async void OnBasicReturn(object sender, BasicReturnEventArgs e)
    {
        if (ReturnedEventHandler != null)
            await ReturnedEventHandler.Invoke(this, e);
    }

    private void OnModelShutdown(object sender, ShutdownEventArgs e)
    {
        _channel.ModelShutdown -= OnModelShutdown;
        _channel.BasicReturn -= OnBasicReturn;
    }

    private IAutorecoveringConnection CreateConnection() => _options.ShareConnection switch
    {
        true => _connectionProvider.GetOrCreateShared(_settings.ConnectionString),
        false => _connectionProvider.GetExclusive(_settings.ConnectionString, _options.ConnectionNameWhenExclusive),
    };
}