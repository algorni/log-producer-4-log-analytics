using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log_producer_app
{


    public class LogProducer : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<LogProducerConfig> _config;

        private readonly Random _rnd = new Random();

        public LogProducer(ILogger<LogProducer> logger, IOptions<LogProducerConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting LogProducer with Max Operation Time: " + _config.Value.MaxOperationDurationInMillieconds);

            while (!cancellationToken.IsCancellationRequested)
            {
                //ok add some kind of parallelization of works...  just to spread start / end time into the logs
                //to showcase log analytics capability the do something task now is asyncronouse in this method
                //and we are not waiting for it

                Task.Factory.StartNew(() => doSomething());

                //ok now just want some time before the next shot of execution  
                //eventually if the MaxTimeBetweenOperationInMillieconds is short enought we could have parallel executions!
                var waitBeforeNext = _rnd.Next(0, _config.Value.MaxTimeBetweenOperationInMillieconds);
                Task.Delay(waitBeforeNext).Wait();
            }


            return Task.CompletedTask;
        }


        private Task doSomething()
        {
            var correlationID = Guid.NewGuid().ToString();

            _logger.LogInformation($"{correlationID},Begin Something");

            //just wait a random time between 0 and tot milliseconds
            var processingTime = _rnd.Next(0, _config.Value.MaxOperationDurationInMillieconds);
            Task.Delay(processingTime).Wait();

            _logger.LogInformation($"{correlationID},Completed Something");

            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping LogProducer.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("LogProducer....");

        }
    }    
}
