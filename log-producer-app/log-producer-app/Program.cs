using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace log_producer_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello logs!!");

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();

            logger.LogInformation("Application started.");


            var myClass = serviceProvider.GetService<LogProducer>();

            myClass.DoSomeStuffAndLogIt();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<LogProducer>();

            //we will configure logging here

            var serilogLogger = new LoggerConfiguration()
               .WriteTo.RollingFile("Logs/log-{HalfHour}.log",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
               .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddSerilog(logger: serilogLogger, dispose: true);
            });
        }
    }
}
