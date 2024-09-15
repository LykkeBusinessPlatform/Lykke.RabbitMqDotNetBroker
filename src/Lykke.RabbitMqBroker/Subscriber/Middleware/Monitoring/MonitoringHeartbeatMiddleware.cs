using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Monitoring;

namespace Lykke.RabbitMqBroker.Subscriber.Middleware.Monitoring;

internal sealed class MonitoringHeartbeatMiddleware<T> : IEventMiddleware<T>
{
    private readonly IMonitoringHeartbeatReceiver _monitoringHeartbeatReceiver;

    public MonitoringHeartbeatMiddleware(IMonitoringHeartbeatReceiver monitoringHeartbeatReceiver)
    {
        _monitoringHeartbeatReceiver = monitoringHeartbeatReceiver;
    }

    public Task ProcessAsync(IEventContext<T> context)
    {
        if (context.BasicProperties.Type == ServiceMessageType.Monitoring.ToString())
        {
            //_monitoringHeartbeatReceiver.Handle(context.Message, context.DeliveryId);

            context.MessageAcceptor.Accept();
            return Task.CompletedTask;
        }

        return context.InvokeNextAsync();
    }
}