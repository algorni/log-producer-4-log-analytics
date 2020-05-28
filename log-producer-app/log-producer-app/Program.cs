using System;
using System.Threading.Tasks;
 using Microsoft.Extensions.Configuration;
 using Microsoft.Extensions.DependencyInjection;
 using Microsoft.Extensions.Hosting;
 using Microsoft.Extensions.Logging;
using Serilog;

namespace log_producer_app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello logs!!");

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables();

                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    var serilogLogger = new LoggerConfiguration()
                         .WriteTo.RollingFile("Logs/log-{HalfHour}.log",
                          outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ssK},{Timestamp:fff},{Level},{Message}{NewLine}")
                         .CreateLogger();

                    services.AddLogging(builder =>
                     {
                         builder.SetMinimumLevel(LogLevel.Information);
                         builder.AddSerilog(logger: serilogLogger, dispose: true);
                     });

                    services.AddOptions();

                    services.Configure<LogProducerConfig>(hostContext.Configuration.GetSection("LogProducerConfig"));

                    services.AddSingleton<IHostedService, LogProducer>();
                })
                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }
    }
}
