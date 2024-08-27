using System;
using System.Collections.Generic;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Lykke.RabbitMqBroker.Tests.Fakes
{
    internal class FakeConnection : IAutorecoveringConnection
    {
        public List<FakeChannel> Channels { get; } = [];
        public int LocalPort { get; }
        public int RemotePort { get; }

        public bool Disposed { get; private set; } = false;

        public void Dispose()
        {
            Disposed = true;
        }

        public void UpdateSecret(string newSecret, string reason)
        {
            throw new NotImplementedException();
        }

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
        }

        public void Close(ushort reasonCode, string reasonText)
        {
        }

        public void Close(TimeSpan timeout)
        {
        }

        public void Close(ushort reasonCode, string reasonText, TimeSpan timeout)
        {
        }

        public IModel CreateModel()
        {
            var channel = new FakeChannel();
            Channels.Add(channel);
            return channel;
        }

        public void HandleConnectionBlocked(string reason)
        {
            throw new NotImplementedException();
        }

        public void HandleConnectionUnblocked()
        {
            throw new NotImplementedException();
        }

        public ushort ChannelMax { get; }
        public IDictionary<string, object> ClientProperties { get; }
        public ShutdownEventArgs CloseReason { get; }
        public AmqpTcpEndpoint Endpoint { get; }
        public uint FrameMax { get; }
        public TimeSpan Heartbeat { get; }
        public bool IsOpen { get; }
        public AmqpTcpEndpoint[] KnownHosts { get; }
        public IProtocol Protocol { get; }
        public IDictionary<string, object> ServerProperties { get; }
        public IList<ShutdownReportEntry> ShutdownReport { get; }
        public string ClientProvidedName { get; }
        public event EventHandler<CallbackExceptionEventArgs> CallbackException;
        public event EventHandler<ConnectionBlockedEventArgs> ConnectionBlocked;
        public event EventHandler<ShutdownEventArgs> ConnectionShutdown;
        public event EventHandler<EventArgs> ConnectionUnblocked;
        public event EventHandler<EventArgs> RecoverySucceeded;
        public event EventHandler<ConnectionRecoveryErrorEventArgs> ConnectionRecoveryError;
        public event EventHandler<ConsumerTagChangedAfterRecoveryEventArgs> ConsumerTagChangeAfterRecovery;
        public event EventHandler<QueueNameChangedAfterRecoveryEventArgs> QueueNameChangeAfterRecovery;

        public FakeChannel LatestChannel { get; private set; }
    }
}
