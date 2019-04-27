using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceGenerator.Services;

namespace PriceGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureAppConfiguration((hc, configApp) =>
                {
                    configApp.SetBasePath(Directory.GetCurrentDirectory());
                    configApp.AddJsonFile("appsettings.json", optional: false);

                    var environment = args?.ElementAtOrDefault(0) ?? "Local";
                    Console.WriteLine(environment);

                    configApp.AddJsonFile($"appsettings.{environment}.json", optional: true);
                })
                .ConfigureServices((hc, services) =>
                {
                    services.AddLogging();
                    services.AddTransient<StockService>();
                    services.AddSingleton<StockPriceManager>();
                    services.AddTransient<MessagePublisherService>();
                    services.AddHostedService<ApplicationService>();
                })
                .ConfigureLogging((HostBuilderContext hc, ILoggingBuilder logConfig) =>
                {
                    logConfig.AddConsole();
                })
                .UseConsoleLifetime()
                .Build();

                await host.RunAsync();  
        }
    }
}
