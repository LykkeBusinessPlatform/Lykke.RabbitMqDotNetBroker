using System;
using System.Collections.Generic;
using System.Linq;
using Lykke.RabbitMqBroker.Logging;

namespace LogParser.Filters
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<OutgoingMessage> FromUtcDate(this IEnumerable<OutgoingMessage> source, DateTime? from)
        {
            if (!from.HasValue) return source;

            return source.Where(x => x.Timestamp.ToUniversalTime() >= from.Value);
        }

        public static IEnumerable<OutgoingMessage> ToUtcDate(this IEnumerable<OutgoingMessage> source, DateTime? to)
        {
            if (!to.HasValue) return source;

            return source.Where(x => x.Timestamp.ToUniversalTime() <= to.Value);
        }

        public static IEnumerable<OutgoingMessage> ExcludeMessageTypes(this IEnumerable<OutgoingMessage> source,
            List<string> messageTypes)
        {
            if (messageTypes.Count == 0) return source;

            return source.Where(x => !messageTypes.Contains(x.MessageTypeName));
        }
    }
}
