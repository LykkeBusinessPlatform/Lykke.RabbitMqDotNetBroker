using Autofac;

using Microsoft.Extensions.DependencyInjection;

namespace Lykke.RabbitMqBroker
{

    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds RabbitMq infrastructure services to the service collection:
        /// - RabbitMq connection provider
        /// - Listeners registry
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddRabbitMq(
            this IServiceCollection services,
            RabbitMqConfiguration configuration)
        {
            services.AddRabbitMqConnectionProvider();
            services.AddListenersRegistry(configuration.ListenersRegistryHandleIntervalMs);

            return services;
        }

        /// <summary>
        /// Adds RabbitMq infrastructure services to the container:
        /// - RabbitMq connection provider
        /// - Listeners registry
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        public static void AddRabbitMq(
            this ContainerBuilder builder,
            RabbitMqConfiguration configuration)
        {
            builder.AddRabbitMqConnectionProvider();
            builder.AddListenersRegistry(configuration.ListenersRegistryHandleIntervalMs);
        }
    }
}