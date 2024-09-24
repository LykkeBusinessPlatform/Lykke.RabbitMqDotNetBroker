using System;
using System.Collections.Generic;

using Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class ChannelFactoryExtensions
{
    private static IConfigurationResult Execute(
        this Func<IModel> channelFactory,
        Action<IModel> action)
    {
        using var channel = channelFactory();
        try
        {
            action(channel);
            return ConfigurationResult.Success();
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason is not null)
            {
                var errorCode = new ConfigurationErrorCode(ex.ShutdownReason.ReplyCode);
                return ConfigurationResult.Failure(
                    new ConfigurationError(errorCode, ex.ShutdownReason.ReplyText));
            }

            throw;
        }
    }

    private static IConfigurationResult<T> Execute<T>(
        this Func<IModel> channelFactory,
        Func<IModel, T> action)
    {
        using var channel = channelFactory();
        try
        {
            var response = action(channel);
            return ConfigurationResult<T>.Success(response);
        }
        catch (OperationInterruptedException ex)
        {
            if (ex.ShutdownReason is not null)
            {
                var errorCode = new ConfigurationErrorCode(ex.ShutdownReason.ReplyCode);
                return ConfigurationResult<T>.Failure(
                    new ConfigurationError(errorCode, ex.ShutdownReason.ReplyText));
            }

            throw;
        }
    }

    /// <summary>
    /// Safe deletion of the queue meaning there are no consumers and 
    /// no messages in the queue.
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="queueName"></param>
    /// <returns></returns>
    /// <remarks>
    /// This method is intended to be used in case of precondition failure.
    /// It works only for classic queues since for quorum queues `ifUnused` 
    /// and `ifEmpty` parameters are not supported so far.
    /// It can't be used conditionally since there is no information about
    /// queue type about to be deleted. Thereore it will always fail
    /// for quorum queues.
    /// </remarks>
    public static IConfigurationResult<uint> SafeDeleteQueue(this Func<IModel> channelFactory, string queueName) =>
        channelFactory.Execute(ch => ch.QueueDelete(queueName, ifUnused: true, ifEmpty: true));

    public static IConfigurationResult<QueueDeclareOk> DeclareQueue(
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

    public static IConfigurationResult DeclareExchange(
        this Func<IModel> channelFactory,
        ExchangeConfigurationOptions options)
    {
        return channelFactory.Execute(ch =>
            ch.ExchangeDeclare(
            exchange: options.ExchangeName.ToString(),
            type: options.ExchangeType,
            durable: options.Durable,
            autoDelete: options.AutoDelete)
        );
    }

    /// <summary>
    /// Binds a queue to an exchange
    /// </summary>
    /// <param name="channelFactory"></param>
    /// <param name="options"></param>
    /// <returns>
    /// The name of the bound queue
    /// </returns>
    public static IConfigurationResult<QueueName> BindQueue(
        this Func<IModel> channelFactory,
        QueueConfigurationOptions options)
    {
        return channelFactory.Execute(ch =>
        {
            ch.QueueBind(
                queue: options.QueueName.ToString(),
                exchange: options.ExistingExchangeName.ToString(),
                routingKey: options.RoutingKey.ToString());
            return options.QueueName;
        });
    }
}