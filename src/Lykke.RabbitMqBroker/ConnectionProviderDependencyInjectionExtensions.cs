using Autofac;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Lykke.RabbitMqBroker
{
    public static class ConnectionProviderDependencyInjectionExtensions
    {
        /// <summary>
        /// Adds a connection provider to the service collection.
        /// Available as <see cref="IConnectionProvider"/>.
        /// Idempotent.
        /// </summary>
        /// <param name="services"></param>
        public static void AddRabbitMqConnectionProvider(this IServiceCollection services)
        {
            services.TryAddSingleton<IAutorecoveringConnectionFactory, AutorecoveringConnectionFactory>();
            services.TryAddSingleton<IConnectionProvider, ConnectionProvider>();
        }

        /// <summary>
        /// Adds a connection provider to the container
        /// Available as <see cref="IConnectionProvider"/>.
        /// Idempotent. 
        /// </summary>
        /// <param name="builder"></param>
        public static void AddRabbitMqConnectionProvider(this ContainerBuilder builder)
        {
            builder.RegisterType<AutorecoveringConnectionFactory>()
                .As<IAutorecoveringConnectionFactory>()
                .SingleInstance()
                .IfNotRegistered(typeof(IAutorecoveringConnectionFactory));
            builder.RegisterType<ConnectionProvider>()
                .As<IConnectionProvider>()
                .SingleInstance()
                .IfNotRegistered(typeof(IConnectionProvider));
        }
    }
}