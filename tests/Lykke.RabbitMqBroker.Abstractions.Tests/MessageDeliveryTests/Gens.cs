using FsCheck;
using FsCheck.Fluent;

using Lykke.RabbitMqBroker.Abstractions.Tracking;

namespace Lykke.RabbitMqBroker.Abstractions.Tests.MessageDeliveryTests;

internal static class Gens
{
    public static class MessageDelivery
    {
        public static Gen<Tracking.MessageDelivery> Pending =>
            from id in MessageDeliveryId
            from route in MessageRoute
            select Tracking.MessageDelivery.Create(id, route);

        public static Gen<Tracking.MessageDelivery> Dispatched =>
            from id in MessageDeliveryId
            from route in MessageRoute
            from dispatchedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
            select Tracking.MessageDelivery
                .Create(id, route)
                .TrySetDispatched(dispatchedTimestamp);

        public static Gen<Tracking.MessageDelivery> Received =>
            from id in MessageDeliveryId
            from route in MessageRoute
            from dispatchedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
            from receivedTimestamp in ArbMap.Default.GeneratorFor<DateTime>()
            where receivedTimestamp > dispatchedTimestamp
            select Tracking.MessageDelivery
                .Create(id, route)
                .TrySetDispatched(dispatchedTimestamp)
                .TrySetReceived(receivedTimestamp);

        public static Gen<Tracking.MessageDelivery> Failed =>
            from dispatchedMessageDelivery in Dispatched
            from failure in MessageDeliveryFailure
            where failure.Timestamp > dispatchedMessageDelivery.DispatchedTimestamp
            select dispatchedMessageDelivery.TrySetFailed(failure);

        public static Gen<Tracking.MessageDelivery> NotFailed =>
            from messageDelivery in Gen.Frequency((1, Pending), (1, Dispatched), (1, Received))
            select messageDelivery;
    }

    public static Gen<MessageDeliveryId> MessageDeliveryId =>
        Gen.Constant(Tracking.MessageDeliveryId.Create());

    public static Gen<MessageDeliveryFailureReason> MessageDeliveryFailureReason =>
        Gen.Elements(
            Tracking.MessageDeliveryFailureReason.DispatchError,
            Tracking.MessageDeliveryFailureReason.BrokerCustodyNotConfirmed,
            Tracking.MessageDeliveryFailureReason.Unroutable);

    public static Gen<MessageDeliveryFailure> MessageDeliveryFailure =>
        from reason in MessageDeliveryFailureReason
        from description in ArbMap.Default.GeneratorFor<string>()
        from timestamp in ArbMap.Default.GeneratorFor<DateTime>()
        select Tracking.MessageDeliveryFailure.Create(reason, description, timestamp);

    public static Gen<MessageRoute> MessageRoute =>
        from exchangeName in ArbMap.Default.GeneratorFor<NonWhiteSpaceString>()
        from queueName in ArbMap.Default.GeneratorFor<NonWhiteSpaceString>()
        from routingKey in ArbMap.Default.GeneratorFor<string>()
        select Tracking.MessageRoute.Create(
            new Tracking.NonEmptyString(exchangeName.Get),
            new Tracking.NonEmptyString(queueName.Get),
            routingKey);
}
