using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

class CountingMessageAcceptor : IMessageAcceptor
{
    public uint AcceptCallsCounter { get; private set; } = 0;
    public uint RejectCallsCounter { get; private set; } = 0;
    public void Accept()
    {
        AcceptCallsCounter++;
    }

    public void Reject(bool requeue = false)
    {
        RejectCallsCounter++;
    }
}