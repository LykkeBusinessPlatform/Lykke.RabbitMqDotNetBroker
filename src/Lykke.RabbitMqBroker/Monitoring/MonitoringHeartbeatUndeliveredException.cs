using System;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

public class MonitoringHeartbeatUndeliveredException : Exception
{
    public MonitoringHeartbeatUndeliveredException(
        DateTime? dispatchedTimestamp,
        MessageDeliveryFailure failure,
        MessageRoute route)
        : base(BuildMessage(dispatchedTimestamp, failure, route))
    {
    }

    private static string BuildMessage(
        DateTime? dispatchedTimestamp,
        MessageDeliveryFailure failure,
        MessageRoute route) =>
        dispatchedTimestamp switch
        {
            null => failure.IsEmpty
                ? $"Monitoring heartbeat was not dispatched. Route: {route}"
                : $"Monitoring heartbeat was not dispatched. Reason: {failure.Reason}, Route: {route}",
            _ => failure.IsEmpty
                ? $"Monitoring heartbeat was not delivered. DispatchedTimestamp: {dispatchedTimestamp}, Route: {route}"
                : $"Monitoring heartbeat was not delivered. DispatchedTimestamp: {dispatchedTimestamp}, Reason: {failure.Reason}, Route: {route}"
        };
}
