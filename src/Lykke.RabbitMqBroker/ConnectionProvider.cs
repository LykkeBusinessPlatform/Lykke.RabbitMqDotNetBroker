// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker
{
    /// <summary>
    /// Manages RabbitMq connections in terms of shared and exclusive connections.
    /// When a shared connection is requested, it is created once and reused for
    /// all subsequent requests with the same connection string.
    /// When an exclusive connection is requested, a new connection is created each time.
    /// Takes care of attaching and detaching event handlers to connections to log connection events.
    /// </summary>
    public sealed class ConnectionProvider : IConnectionProvider
    {
        private readonly ConcurrentDictionary<ConnectionStringHash, IAutorecoveringConnection> _sharedConnections = new();
        private readonly ConcurrentBag<IAutorecoveringConnection> _exclusiveConnections = new();
        private readonly IAutorecoveringConnectionFactory _connectionFactory;
        private readonly ILogger<ConnectionProvider> _logger;

        public ConnectionProvider(ILogger<ConnectionProvider> logger, IAutorecoveringConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _logger = logger;
        }

        public IAutorecoveringConnection GetOrCreateShared(string connectionString)
        {
            var hash = new ConnectionStringHash(connectionString);
            return _sharedConnections.GetOrAdd(hash, _ => CreateConnection(connectionString, "SharedConnection"));
        }

        public IAutorecoveringConnection GetExclusive(string connectionString, string name = null)
        {
            name ??= "ExclusiveConnection";
            
            var connection = CreateConnection(connectionString, name);
            _exclusiveConnections.Add(connection);
            return connection;
        }
        
        public void Dispose()
        {
            foreach (var connection in _exclusiveConnections)
            {
                DetachConnectionEventHandlers(connection);
                connection.Dispose();
            }
            
            foreach (var connection in _sharedConnections.Values)
            {
                DetachConnectionEventHandlers(connection);
                connection.Dispose();
            }
        }
        
        private IAutorecoveringConnection CreateConnection(string connectionString, string name)
        {
            var connection = _connectionFactory.Create(connectionString, name);
            AttachConnectionEventHandlers(connection);
            return connection;
        }
        
        private void AttachConnectionEventHandlers(IAutorecoveringConnection connection)
        {
            connection.RecoverySucceeded += OnRecoverySucceeded;
            connection.ConnectionBlocked += OnConnectionBlocked;
            connection.ConnectionShutdown += OnConnectionShutdown;
            connection.ConnectionUnblocked += OnConnectionUnblocked;
            connection.CallbackException += OnCallbackException;
            connection.ConnectionRecoveryError += OnConnectionRecoveryError;
        }
        
        private void DetachConnectionEventHandlers(IAutorecoveringConnection connection)
        {
            connection.RecoverySucceeded -= OnRecoverySucceeded;
            connection.ConnectionBlocked -= OnConnectionBlocked;
            connection.ConnectionShutdown -= OnConnectionShutdown;
            connection.ConnectionUnblocked -= OnConnectionUnblocked;
            connection.CallbackException -= OnCallbackException;
            connection.ConnectionRecoveryError -= OnConnectionRecoveryError;
        }
        
        private void OnRecoverySucceeded(object sender, EventArgs e)
        {
            _logger?.LogInformation("RabbitMq connection recovered");
        }
        
        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            _logger?.LogWarning("RabbitMq connection blocked: {Reason}", e.Reason);
        }
        
        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger?.LogWarning(
                "RabbitMq connection shutdown. Initiator: {Initiator}, reply code: {ReplyCode}, reply text: {ReplyText}, method id: {MethodId}",
                e.Initiator, e.ReplyCode, e.ReplyText, e.MethodId);
            
            if (e.Cause != null)
            {
                _logger?.LogWarning("RabbitMq connection shutdown cause object: {Cause}", e.Cause.GetType().FullName);
            }
        }
        
        private void OnConnectionUnblocked(object sender, EventArgs e)
        {
            _logger?.LogInformation("RabbitMq connection unblocked");
        }
        
        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            _logger?.LogError(e.Exception, "RabbitMq connection callback exception");
        }
        
        private void OnConnectionRecoveryError(object sender, ConnectionRecoveryErrorEventArgs e)
        {
            _logger?.LogError(e.Exception, "RabbitMq connection recovery error");
        }
    }
}
