// Copyright (c) 2024 Lykke Corp.
// See the LICENSE file in the project root for more information.

using RabbitMQ.Client;

namespace Lykke.RabbitMqBroker;

public interface IMonitoringMessageChannelProvider
{
    /// <summary>
    /// Creates new channel for monitoring messages.
    /// </summary>
    /// <returns></returns>
    public IModel Create();
}
