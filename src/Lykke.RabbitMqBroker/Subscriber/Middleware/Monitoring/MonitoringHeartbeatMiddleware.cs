using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

/// <summary>
/// Middleware for handling monitoring heartbeat messages.
/// Detects monitoring heartbeat messages and passes them to the monitoring heartbeat receiver.
/// Propagates the message to the next middleware if the message is not a monitoring heartbeat.
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class MonitoringHeartbeatMiddleware<T> : IEventMiddleware<T>
{
    private readonly IMonitoringHeartbeatReceiver _monitoringHeartbeatReceiver;

    public MonitoringHeartbeatMiddleware(IMonitoringHeartbeatReceiver monitoringHeartbeatReceiver = null)
    {
        _monitoringHeartbeatReceiver = monitoringHeartbeatReceiver;
    }

    public async Task ProcessAsync(IEventContext<T> context)
    {
        if (context.IsMonitoringMessage())
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
}