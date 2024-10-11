using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.RabbitMqBroker.Subscriber;

internal static class TaskExtensions
{
    public static async Task RunSequentially(this IEnumerable<Task> tasks)
    {
        foreach (var task in tasks)
        {
            await task;
        }
    }
}