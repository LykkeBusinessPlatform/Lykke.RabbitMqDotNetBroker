using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

/// <summary>
/// Middleware for handling monitoring heartbeat messages.
/// Detects monitoring heartbeat messages and passes them to the monitoring heartbeat receiver.
/// Propagates the message to the next middleware if the message is not a monitoring heartbeat.
/// Skips monitoring messages with destination different from the one registered listeners have.
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class MonitoringHeartbeatMiddleware<T> : IEventMiddleware<T>
{
    private readonly IMonitoringHeartbeatReceiver _monitoringHeartbeatReceiver;
    private readonly IListenersRegistry _listenersRegistry;

    public MonitoringHeartbeatMiddleware(
        IMonitoringHeartbeatReceiver monitoringHeartbeatReceiver = null,
        IListenersRegistry listenersRegistry = null)
    {
        _monitoringHeartbeatReceiver = monitoringHeartbeatReceiver;
        _listenersRegistry = listenersRegistry;
    }

    public async Task ProcessAsync(IEventContext<T> context)
    {
        if (context.IsMonitoringMessage() && context.MonitoringMessageDestinationOneOf(GetMonitoredQueues()))
        {
            if (_monitoringHeartbeatReceiver is not null)
            {
                await _monitoringHeartbeatReceiver.Handle(context.Body, context.BasicProperties.GetDeliveryId());
            }
            context.MessageAcceptor.Accept();
            return;
        }

        await context.InvokeNextAsync();
    }

    private IEnumerable<QueueName> GetMonitoredQueues() => _listenersRegistry?.Select(l => l.ListenerRoute.QueueName) ?? [];
}