using LogParser;
using LogParser.Configuration;
using LogParser.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddJsonFile("appsettings.json");
    })
    .ConfigureLogging(x => x.AddConsole())
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<App>();

        services.AddSingleton<Configuration>();

        services.Configure<ParsingOptions>(
            context.Configuration.GetSection(nameof(ParsingOptions)));
        services.Configure<FilterOptions>(
            context.Configuration.GetSection(nameof(FilterOptions)));

        // rabbitmq connection
        var rcs = context.Configuration.GetConnectionString("rabbitmq");

        services.AddSingleton(provider =>
            ActivatorUtilities.CreateInstance<Publisher>(provider,
                rcs,
                provider.GetRequiredService<ILogger<Publisher>>()));
    });

var host = hostBuilder.Build();

var app = host.Services.GetRequiredService<App>();
app.Execute();
