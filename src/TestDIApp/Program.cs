using Autofac.Extensions.DependencyInjection;
using Lykke.RabbitMqBroker;
using Lykke.RabbitMqBroker.Subscriber;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestDIApp;
using TestDIApp.Handlers;
using TestDIApp.Messages;

var builder = Host.CreateDefaultBuilder(args);

await builder
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((_, configurationBuilder) =>
    {
        configurationBuilder.AddEnvironmentVariables();
        configurationBuilder.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddRabbitMqConnectionProvider();
        services.AddSingleton<RandomPrefetchCountGenerator>();
        
        // Add Mars messages listener
        var marsSubscriptionSettings = ctx
            .Configuration
            .GetSection("MarsSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<MarsMessage, MarsMessageHandler>(
            marsSubscriptionSettings,
            opt =>
            {
                opt.SerializationFormat = SerializationFormat.Json;
                opt.ShareConnection = true;
                opt.SubscriptionTemplate = SubscriptionTemplate.NoLoss;
            });

        // Add Jupiter messages listener
        var jupiterSubscriptionSettings = ctx
            .Configuration
            .GetSection("JupiterSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<JupiterMessage, JupiterMessageHandler>(
            jupiterSubscriptionSettings,
            opt =>
            {
                opt.SerializationFormat = SerializationFormat.Json;
                opt.ShareConnection = true;
                opt.SubscriptionTemplate = SubscriptionTemplate.LossAcceptable;
            });
        
        // Additional Jupiter messages handler
        services.AddSingleton<IMessageHandler<JupiterMessage>, AnotherJupiterMessageHandler>();
        
        // empty options, defaults will be used
        var venusSubscriptionSettings = ctx
            .Configuration
            .GetSection("VenusSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<VenusMessage, VenusMessageHandler>(
            venusSubscriptionSettings, _ => { });

        // subscriber additional manual re-configuration example
        var mercurySubscriptionSettings = ctx
            .Configuration
            .GetSection("MercurySubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<MercuryMessage, MercuryMessageHandler>(
            mercurySubscriptionSettings,
            _ => { },
            (s, p) => 
                s.SetPrefetchCount(p.GetService<RandomPrefetchCountGenerator>()?.Generate() ?? 1)
                    //.UseMiddleware()
                    //.SetReadHeadersAction())
                    // ...
                    );
        
        // Multiple subscribers example
        var plutoSubscriptionSettings = ctx
            .Configuration
            .GetSection("PlutoSubscription")
            .Get<RabbitMqSubscriptionSettings>();
        services.AddRabbitMqListener<PlutoMessage, PlutoMessageHandler>(
            plutoSubscriptionSettings,
            opt =>
            {
                opt.SerializationFormat = SerializationFormat.Json;
                opt.ShareConnection = true;
                opt.SubscriptionTemplate = SubscriptionTemplate.LossAcceptable;
                opt.ConsumerCount = 5;
            });

    })
    .RunConsoleAsync();
