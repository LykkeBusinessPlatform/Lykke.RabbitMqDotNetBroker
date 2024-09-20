using System;
using System.Collections.Generic;

using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Subscriber;

using Success = QueueConfigurationSuccess;
using Failure = QueueConfigurationPreconditionFailure;

internal static class ChannelFactoryExtensions
{
    private static IQueueConfigurationResult Execute<TResponse>(this Func<IModel> channelFactory, Func<IModel, TResponse> action)
    {
        using var channel = channelFactory();
        try
        {
            var response = action(channel);
            return new QueueConfigurationSuccess<TResponse>(response);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason is { ReplyCode: Constants.PreconditionFailed })
                return new Failure(ex.ShutdownReason?.ReplyText ?? ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Safe deletion of classic queue meaning there are no consumers and 
    /// no messages in the queue.
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="queueName"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method is intended to be used in case of precondition failure.
    /// It works only for classic queues since for quorum queues `ifUnused` 
    /// and `ifEmpty` parameters are not supported so far.
    /// </remarks>
    public static IQueueConfigurationResult SafeDeleteClassicQueue(this Func<IModel> channelFactory, string queueName) =>
        channelFactory.Execute(ch => ch.QueueDelete(queueName, ifUnused: true, ifEmpty: true));

    public static IQueueConfigurationResult DeclareQueue(
        this Func<IModel> channelFactory,
        QueueConfigurationOptions options,
        Dictionary<string, object> args)
    {
        return channelFactory.Execute(ch => ch.QueueDeclare(
            queue: options.QueueName,
            durable: options.Durable,
            exclusive: false,
            autoDelete: options.AutoDelete,
            arguments: args));
    }

    public static IQueueConfigurationResult BindQueue(
        this Func<IModel> channelFactory,
        string queueName,
        QueueConfigurationOptions options)
    {
        return channelFactory.Execute(ch =>
        {
            ch.QueueBind(
                queue: queueName,
                exchange: options.ExchangeName,
                routingKey: options.RoutingKey);
            return queueName;
        });
    }

    /// <summary>
    /// Tries to fix precondition failure by deleting the queue.
    /// Usually the one wants to run follow up action to configure new queue after that.
    /// For this purpose, the nextAction is provided though it's optional.
    /// Works only for classic queues.
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <param name="nextAction"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Success TryFixPreconditionFailureOrThrow(
        this Func<IModel> channelFactory,
        QueueConfigurationOptions options,
        Func<IQueueConfigurationResult> nextAction) =>
        channelFactory.SafeDeleteClassicQueue(options.QueueName) switch
        {
            Success deleteSuccess => nextAction?.Invoke() switch
            {
                null => deleteSuccess,
                Success nextActionSuccess => nextActionSuccess,
                _ => throw new InvalidOperationException($"Failed to configure queue [{options.QueueName}] after precondition failure")
            },
            Failure failure => throw new InvalidOperationException($"Tried to safely delete queue [{options.QueueName}] due to configuration mismatch but failed: {failure.Message}"),
            _ => throw new InvalidOperationException("Unexpected queue deletion result")
        };
}