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
        public static void AddRabbitMq(this IServiceCollection services)
        {
            services.AddRabbitMqConnectionProvider();
            services.AddListenersRegistry();
        }

        /// <summary>
        /// Adds RabbitMq infrastructure services to the container:
        /// - RabbitMq connection provider
        /// - Listeners registry
        /// </summary>
        public static void AddRabbitMq(this ContainerBuilder builder)
        {
            builder.AddRabbitMqConnectionProvider();
            builder.AddListenersRegistry();
        }
    }
}