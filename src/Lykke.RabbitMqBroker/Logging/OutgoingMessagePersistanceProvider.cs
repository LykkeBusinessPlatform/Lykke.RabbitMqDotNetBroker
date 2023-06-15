// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Lykke.RabbitMqBroker.Logging
{
    internal static class OutgoingMessagePersistanceProvider
    {
        public static IOutgoingMessagePersister Get()
        {
            if (EnvironmentVariables.DisableOutgoingMessagePersistence)
                return new NullOutgoingMessagePersister();
            
            return new OutgoingMessageLogPersister();
        }
    }
}
