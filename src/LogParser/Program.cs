using System.IO;
using LogParser.Configuration;
using LogParser.LogParsers;
using LogParser.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LogParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
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

                    services.AddSingleton<Configuration.Configuration>();

                    services.AddSingleton<JsonRecordParser>();

                    services.Configure<ParsingOptions>(
                        context.Configuration.GetSection(nameof(ParsingOptions)));
                    services.Configure<FilterOptions>(
                        context.Configuration.GetSection(nameof(FilterOptions)));
                    services.Configure<RabbitConfig>(
                        context.Configuration.GetSection(nameof(RabbitConfig)));
                    services.Configure<InputConfig>(
                        context.Configuration.GetSection(nameof(InputConfig)));

                    // rabbitmq connection
                    var rcs = context.Configuration.GetConnectionString("rabbitmq");

                    services.AddSingleton<Publisher>();
                });

            var host = hostBuilder.Build();

            var app = host.Services.GetRequiredService<App>();
            app.Execute();
        }
    }
}
