using System;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

public class MonitoringHearbeatLateDeliveryException : Exception
{
    public MonitoringHearbeatLateDeliveryException(
        DateTime? dispatchedTimestamp,
        DateTime? receivedTimestamp,
        MessageRoute route)
        : base(BuildMessage(dispatchedTimestamp, receivedTimestamp, route))
    {
    }

    private static string BuildMessage(
        DateTime? dispatchedTimestamp,
        DateTime? receivedTimestamp,
        MessageRoute route) =>
        $"Monitoring heartbeat was delivered but late. DispatchedTimestamp: {dispatchedTimestamp}, ReceivedTimestamp: {receivedTimestamp}, TotalDelay: {receivedTimestamp - dispatchedTimestamp}, Route: {route}";
}
