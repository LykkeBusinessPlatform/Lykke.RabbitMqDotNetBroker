using System;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Monitoring;

public class MonitoringHeartbeatUndeliveredException(
    DateTime? dispatchedTimestamp,
    MessageDeliveryFailure failure,
    MessageRoute route) : Exception(BuildMessage(dispatchedTimestamp, failure, route))
{

    private static string BuildMessage(
        DateTime? dispatchedTimestamp,
        MessageDeliveryFailure failure,
        MessageRoute route) => dispatchedTimestamp switch
        {
            null => $"Monitoring heartbeat was not dispatched.{GetReasonText(failure)} Route: {route}",
            _ => $"Monitoring heartbeat was not delivered. DispatchedTimestamp: {dispatchedTimestamp},{GetReasonText(failure)} Route: {route}"
        };

    private static string GetReasonText(MessageDeliveryFailure failure) =>
        failure.IsEmpty ? string.Empty : $" Reason: {failure.Reason},";
}
