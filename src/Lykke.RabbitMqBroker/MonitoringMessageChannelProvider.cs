using System;

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker;


/// <summary>
/// Creates a new channel for the monitoring messages for specified connection 
/// string.
/// Channels created by this provider should be disposed after use by the caller.
/// </summary>
/// <param name="connectionProvider"></param>
/// <param name="connectionString"></param>
internal sealed class MonitoringMessageChannelProvider(
    IConnectionProvider connectionProvider,
    string connectionString) : IMonitoringMessageChannelProvider
{
    private readonly IConnectionProvider _connectionProvider = connectionProvider;
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    private const string ConnectionName = "Monitoring messages connection";

    /// <summary>
    /// Creates a new channel for the monitoring messages.
    /// </summary>
    /// <returns>
    /// A new channel for the monitoring messages which should be disposed after use.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     When the monitoring message was not taken by the broker.
    /// </exception>
    public IModel Create()
    {
        var connection = _connectionProvider.GetExclusive(
            _connectionString,
            ConnectionName);

        var channel = connection.CreateModel();
        channel.ConfirmSelect();
        channel.BasicReturn += (sender, args) =>
        {
            throw new InvalidOperationException($"Monitoring message was returned: {args.ReplyText}");
        };
        return channel;
    }
}