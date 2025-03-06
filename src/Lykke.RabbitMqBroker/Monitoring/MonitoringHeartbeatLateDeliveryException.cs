using System;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

public class MonitoringHeartbeatLateDeliveryException(
    DateTime? dispatchedTimestamp,
    DateTime? receivedTimestamp,
    MessageRoute route) : Exception(BuildMessage(dispatchedTimestamp, receivedTimestamp, route))
{

    private static string BuildMessage(
        DateTime? dispatchedTimestamp,
        DateTime? receivedTimestamp,
        MessageRoute route) =>
        $"Monitoring heartbeat was delivered but late. DispatchedTimestamp: {dispatchedTimestamp}, ReceivedTimestamp: {receivedTimestamp}, TotalDelay: {receivedTimestamp - dispatchedTimestamp}, Route: {route}";
}
