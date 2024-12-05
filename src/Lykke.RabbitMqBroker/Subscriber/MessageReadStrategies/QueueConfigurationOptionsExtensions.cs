using System.Collections.Generic;

namespace Lykke.RabbitMqBroker.Subscriber.MessageReadStrategies;

internal static class QueueConfigurationOptionsExtensions
{
    public static Dictionary<string, object> BuildArguments(this QueueConfigurationOptions options)
    {
        var argsBuilder = new QueueDeclarationArgumentsBuilder();
        if (options.QueueType == QueueType.Quorum)
        {
            argsBuilder.UseQuorumQueue();
        }

        if (!options.Ttl.IsInfinite)
        {
            argsBuilder.UseTimeToLive(options.Ttl);
        }

        if (options.DeadLetterExchangeName is not null)
        {
            var dlxArgsBuilder = argsBuilder.AddDeadLetterExchange(options.DeadLetterExchangeName);
            if (options.QueueType == QueueType.Quorum)
            {
                dlxArgsBuilder.WithAtLeastOnceStrategy();
            }
            return dlxArgsBuilder.Build();
        }

        return argsBuilder.Build();
    }
}
