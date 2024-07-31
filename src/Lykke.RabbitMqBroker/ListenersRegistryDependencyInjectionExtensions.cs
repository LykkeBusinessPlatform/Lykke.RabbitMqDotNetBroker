using Autofac;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lykke.RabbitMqBroker
{
    public static class ListenersRegistryDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds a listeners registry to the service collection.
        /// Available as <see cref="IListenersRegistry"/>.
        /// Idempotent.
        /// </summary>
        /// <param name="services"></param>
        public static void AddListenersRegistry(this IServiceCollection services)
        {
            services.TryAddSingleton<IListenersRegistry, ListenersRegistry>();
        }

        /// <summary>
        /// Adds a listeners registry to the container
        /// Available as <see cref="IListenersRegistry"/>.
        /// Idempotent.
        /// </summary>
        public static void AddListenersRegistry(this ContainerBuilder builder)
        {
            builder.RegisterType<ListenersRegistry>()
                .As<IListenersRegistry>()
                .SingleInstance()
                .IfNotRegistered(typeof(IListenersRegistry));
        }
    }
}