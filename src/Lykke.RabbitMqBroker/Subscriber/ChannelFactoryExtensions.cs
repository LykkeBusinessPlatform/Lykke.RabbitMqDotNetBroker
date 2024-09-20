using System;
using System.Collections.Generic;

using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class ChannelFactoryExtensions
{
    private static QueueConfigurationResult<TResponse> Execute<TResponse>(this Func<IModel> channelFactory, Func<IModel, TResponse> action)
    {
        using var channel = channelFactory();
        try
        {
            var response = action(channel);
            return QueueConfigurationResult<TResponse>.Success(response);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason is not null)
            {
                return QueueConfigurationResult<TResponse>.Failure(
                    new QueueConfigurationError(ex.ShutdownReason.ReplyCode, ex.ShutdownReason.ReplyText));
            }

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
    public static QueueConfigurationResult<uint> SafeDeleteClassicQueue(this Func<IModel> channelFactory, string queueName) =>
        channelFactory.Execute(ch => ch.QueueDelete(queueName, ifUnused: true, ifEmpty: true));

    public static QueueConfigurationResult<QueueDeclareOk> DeclareQueue(
        this Func<IModel> channelFactory,
        QueueConfigurationOptions options,
        Dictionary<string, object> args)
    {
        return channelFactory.Execute(ch => ch.QueueDeclare(
            queue: options.QueueName.ToString(),
            durable: options.Durable,
            exclusive: false,
            autoDelete: options.AutoDelete,
            arguments: args));
    }

    public static QueueConfigurationResult<QueueName> BindQueue(
        this Func<IModel> channelFactory,
        QueueName queueName,
        QueueConfigurationOptions options)
    {
        return channelFactory.Execute(ch =>
        {
            ch.QueueBind(
                queue: queueName.ToString(),
                exchange: options.ExchangeName.ToString(),
                routingKey: options.RoutingKey.ToString());
            return queueName;
        });
    }
}