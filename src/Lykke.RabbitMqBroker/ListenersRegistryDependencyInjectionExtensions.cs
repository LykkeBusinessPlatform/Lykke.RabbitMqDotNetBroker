using System;

using Autofac;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Lykke.RabbitMqBroker
{
    public static class ListenersRegistryDependencyInjectionExtensions
    {
        private const int DefaultHandleIntervalMs = 60000;

        /// <summary>
        /// Adds a listeners registry to the service collection
        /// along with special worker to handle every listener
        /// registration at a specified interval.
        /// Available as <see cref="IListenersRegistry"/>.
        /// Idempotent.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="handleIntervalMs">
        /// Period of time in milliseconds to handle every listener registration.
        /// </param>
        /// <param name="autoStart">
        /// Whether to start worker automatically.
        /// If not, you should start it manually by resolving <see cref="ListenersRegistryTimer"/>
        /// and calling <see cref="IHostedService.StartAsync"/>.
        /// </param>
        public static void AddListenersRegistry(
            this IServiceCollection services,
            int handleIntervalMs = DefaultHandleIntervalMs,
            bool autoStart = true)
        {
            services.TryAddSingleton<IListenersRegistry, ListenersRegistry>();
            services.TryAddSingleton<IListenersRegistryWorker, ListenersRegistrySequentialWorker>();

            if (autoStart)
            {
                services.AddHostedService(p =>
                    new ListenersRegistryTimer(
                        p.GetRequiredService<IListenersRegistryWorker>(),
                        TimeSpan.FromMilliseconds(handleIntervalMs)));
            }
            else
            {
                services.TryAddSingleton(p =>
                    new ListenersRegistryTimer(
                        p.GetRequiredService<IListenersRegistryWorker>(),
                        TimeSpan.FromMilliseconds(handleIntervalMs)));
            }
        }

        /// <summary>
        /// Adds a listeners registry to the service collection
        /// along with special worker to handle every listener
        /// registration at a specified interval.
        /// Available as <see cref="IListenersRegistry"/>.
        /// Idempotent.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="handleIntervalMs">
        /// Period of time in milliseconds to handle every listener registration.
        /// </param>
        /// <param name="autoStart">
        /// Whether to start worker automatically.
        /// If not, you should start it manually by resolving <see cref="ListenersRegistryTimer"/>
        /// and calling <see cref="IHostedService.StartAsync"/>.
        /// </param>
        public static void AddListenersRegistry(
            this ContainerBuilder builder,
            int handleIntervalMs = DefaultHandleIntervalMs,
            bool autoStart = true)
        {
            builder.RegisterType<ListenersRegistry>()
                .As<IListenersRegistry>()
                .SingleInstance()
                .IfNotRegistered(typeof(IListenersRegistry));
            builder.RegisterType<ListenersRegistrySequentialWorker>()
                .As<IListenersRegistryWorker>()
                .SingleInstance()
                .IfNotRegistered(typeof(IListenersRegistryWorker));

            if (autoStart)
            {
                builder.RegisterType<ListenersRegistryTimer>()
                    .As<IHostedService>()
                    .WithParameter(TypedParameter.From(TimeSpan.FromMilliseconds(handleIntervalMs)))
                    .SingleInstance();
            }
            else
            {
                builder.Register(c => new ListenersRegistryTimer(
                        c.Resolve<IListenersRegistryWorker>(),
                        TimeSpan.FromMilliseconds(handleIntervalMs)))
                    .AsSelf()
                    .SingleInstance();
            }
        }
    }
}