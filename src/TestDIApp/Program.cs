using Autofac;
using Autofac.Extensions.DependencyInjection;

using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Monitoring;
using Lykke.RabbitMqBroker.Subscriber;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using TestDIApp;
using TestDIApp.Handlers;
using TestDIApp.Messages;

var builder = Host.CreateDefaultBuilder(args);

#region Using service collection as a container

await builder
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((_, configurationBuilder) =>
    {
        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddLogging(lb => lb.ClearProviders().AddConsole().SetMinimumLevel(LogLevel.Debug));
        var configuration = new RabbitMqConfiguration();
        ctx.Configuration.GetSection("RabbitMq").Bind(configuration);
        services.AddRabbitMq(configuration);

        var connectionString = ctx.Configuration.GetConnectionString("RabbitMq");
        services.AddRabbitMqMonitoring<MessageDeliveryInMemoryStorage, MonitoringIssueLogger>(configuration.Monitoring, connectionString);
        services.AddRabbitMqMonitoringRetentionPolicy<MessageDeliveryInMemoryStorage>(configuration.Monitoring);
        services.AddSingleton<RandomPrefetchCountGenerator>();

        // Add Mars messages listener
        var marsSubscriptionSettings = ctx
            .Configuration
            .GetSection("MarsSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services
            .AddRabbitMqListener<MarsMessage, MarsMessageHandler>(marsSubscriptionSettings)
            .AddOptions(RabbitMqListenerOptions<MarsMessage>.Json.NoLoss)
            .AutoStart();

        // Add Jupiter messages listener with additional messages handler
        var jupiterSubscriptionSettings = ctx
            .Configuration
            .GetSection("JupiterSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services
            .AddRabbitMqListener<JupiterMessage, JupiterMessageHandler>(jupiterSubscriptionSettings)
            .AddMessageHandler<AnotherJupiterMessageHandler>()
            .AddOptions(RabbitMqListenerOptions<JupiterMessage>.Json.LossAcceptable)
            .AutoStart();

        // empty options, defaults will be used
        var venusSubscriptionSettings = ctx
            .Configuration
            .GetSection("VenusSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<VenusMessage, VenusMessageHandler>(venusSubscriptionSettings)
            .AddOptions(_ => { })
            .AutoStart();

        // subscriber additional manual re-configuration example
        var mercurySubscriptionSettings = ctx
            .Configuration
            .GetSection("MercurySubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<MercuryMessage, MercuryMessageHandler>(
                mercurySubscriptionSettings,
                (s, p) =>
                    s.SetPrefetchCount(p.GetService<RandomPrefetchCountGenerator>()?.Generate() ?? 1)
                //.UseMiddleware()
                //.SetReadHeadersAction())
                // ...
            )
            .AddOptions(_ => { })
            .AutoStart();

        // Multiple subscribers example
        var plutoSubscriptionSettings = ctx
            .Configuration
            .GetSection("PlutoSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<PlutoMessage, PlutoMessageHandler>(plutoSubscriptionSettings)
            .AddOptions(
                opt =>
                {
                    opt.SerializationFormat = SerializationFormat.Json;
                    opt.ShareConnection = true;
                    opt.SubscriptionTemplate = SubscriptionTemplate.LossAcceptable;
                    opt.ConsumerCount = 5;
                })
            .AutoStart();

        services.AddHostedService<ListenerRegistryLogger>();
    })
    .RunConsoleAsync();

# endregion


// # region Using Autofac as a container

// await builder
//     .UseServiceProviderFactory(new AutofacServiceProviderFactory())
//     .ConfigureAppConfiguration((_, configurationBuilder) =>
//     {
//         configurationBuilder.AddEnvironmentVariables();
//         configurationBuilder.AddJsonFile("appsettings.json");
//     })
//     .ConfigureContainer<ContainerBuilder>((ctx, bld) =>
//     {
//         var configuration = new RabbitMqConfiguration();
//         ctx.Configuration.GetSection("RabbitMq").Bind(configuration);
//         bld.AddRabbitMq(configuration);

//         var connectionString = ctx.Configuration.GetConnectionString("RabbitMq");
//         bld.AddRabbitMqMonitoring<MessageDeliveryInMemoryStorage>(configuration.Monitoring, connectionString);

//         bld.RegisterType<RandomPrefetchCountGenerator>();

//         // Add Mars messages listener
//         var marsSubscriptionSettings = ctx
//             .Configuration
//             .GetSection("MarsSubscription")
//             .Get<RabbitMqSubscriptionSettings>();
//         bld.AddRabbitMqListener<MarsMessage, MarsMessageHandler>(marsSubscriptionSettings)
//             .AddOptions(RabbitMqListenerOptions<MarsMessage>.Json.NoLoss)
//             .AutoStart();

//         // Add Jupiter messages listener with additional messages handler
//         var jupiterSubscriptionSettings = ctx
//             .Configuration
//             .GetSection("JupiterSubscription")
//             .Get<RabbitMqSubscriptionSettings>();
//         bld.AddRabbitMqListener<JupiterMessage, JupiterMessageHandler>(jupiterSubscriptionSettings)
//             .AddMessageHandler<AnotherJupiterMessageHandler>()
//             .AddOptions(RabbitMqListenerOptions<JupiterMessage>.Json.LossAcceptable)
//             .AutoStart();

//         bld.RegisterType<ListenerRegistryLogger>().As<IHostedService>().SingleInstance();

//     }).RunConsoleAsync();

// #endregion
