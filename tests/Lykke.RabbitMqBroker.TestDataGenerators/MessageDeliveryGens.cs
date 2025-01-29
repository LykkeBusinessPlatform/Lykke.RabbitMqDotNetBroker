using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.TestDataGenerators;

public static class MessageDeliveryGens
{
    public static Gen<MessageDelivery> Pending =>
            from id in MessageDeliveryId
            from route in MessageRoute
            select MessageDelivery.Create(id, route);

    public static Gen<MessageDelivery> DispatchedAt(DateTime dispatchedTimestamp) =>
        from id in MessageDeliveryId
        from route in MessageRoute
        select MessageDelivery
            .Create(id, route)
            .TrySetDispatched(dispatchedTimestamp);

    public static Gen<MessageDelivery> Dispatched =>
        from dispatchedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
        from dispatchedMessageDelivery in DispatchedAt(dispatchedTimestamp)
        select dispatchedMessageDelivery;

    public static Gen<MessageDelivery> Received =>
        from id in MessageDeliveryId
        from route in MessageRoute
        from dispatchedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
        from receivedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
        where receivedTimestamp > dispatchedTimestamp
        select MessageDelivery
            .Create(id, route)
            .TrySetDispatched(dispatchedTimestamp)
            .TrySetReceived(receivedTimestamp);

    public static Gen<MessageDelivery> Failed =>
        from dispatchedMessageDelivery in Dispatched
        from failure in MessageDeliveryFailure
        where failure.Timestamp > dispatchedMessageDelivery.DispatchedTimestamp
        select dispatchedMessageDelivery.TrySetFailed(failure);

    public static Gen<MessageDelivery> NotFailed =>
        from messageDelivery in Gen.Frequency((1, Pending), (1, Dispatched), (1, Received))
        select messageDelivery;

    public static Gen<MessageDeliveryId> MessageDeliveryId =>
        Gen.Constant(Abstractions.Tracking.MessageDeliveryId.Create());

    public static Gen<MessageDeliveryFailureReason> MessageDeliveryFailureReason =>
        Gen.Elements(
            Abstractions.Tracking.MessageDeliveryFailureReason.DispatchError,
            Abstractions.Tracking.MessageDeliveryFailureReason.BrokerCustodyNotConfirmed,
            Abstractions.Tracking.MessageDeliveryFailureReason.Unroutable);

    public static Gen<MessageDeliveryFailure> MessageDeliveryFailureAt(DateTime failedTimestamp) =>
        from reason in MessageDeliveryFailureReason
        from description in ArbMap.Default.GeneratorFor<string>()
        select Abstractions.Tracking.MessageDeliveryFailure.Create(reason, description, failedTimestamp);

    public static Gen<MessageDeliveryFailure> MessageDeliveryFailure =>
        from failedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
        from failure in MessageDeliveryFailureAt(failedTimestamp)
        select failure;

    public static Gen<MessageRoute> MessageRoute =>
        from exchangeName in ArbMap.Default.GeneratorFor<NonWhiteSpaceString>()
        from queueName in ArbMap.Default.GeneratorFor<NonWhiteSpaceString>()
        from routingKey in ArbMap.Default.GeneratorFor<string>()
        select Abstractions.Tracking.MessageRoute.Create(
            new Abstractions.Tracking.NonEmptyString(exchangeName.Get),
            new Abstractions.Tracking.NonEmptyString(queueName.Get),
            routingKey);
}