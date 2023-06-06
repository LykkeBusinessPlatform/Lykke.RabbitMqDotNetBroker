// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Linq;

namespace Lykke.RabbitMqBroker
{
    public static class PublisherBufferTypes
    {
        public const string Default = "default";
        public const string LockFree = "lock-free";
        public const string Experimental = "experimental";
        
        public static readonly string[] All = {Default, LockFree, Experimental};
        
        public static bool IsValid(string type)
        {
            return All.Any(x => x == type);
        }
    }
}
