using System;
using System.Threading.Tasks;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

using Lykke.RabbitMqBroker.Monitoring;

namespace Lykke.RabbitMqBroker.Tests.MonitoringHeartbeatMiddlewareTests;

class MonitoringHeartbeatCountingReceiver : IMonitoringHeartbeatReceiver
{
    public uint HandleCallsCounter { get; private set; } = 0;
    public Task Handle(ReadOnlyMemory<byte> body, MessageDeliveryId deliveryId)
    {
        HandleCallsCounter++;
        return Task.CompletedTask;
    }
}