using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace log_producer_app
{
    public class LogProducer
    {
        private readonly ILogger _logger;

        public LogProducer(ILogger<LogProducer> logger)
        {
            _logger = logger;
        }

        public void DoSomeStuffAndLogIt()
        {
            Random rnd = new Random();

            while (true)
            {
                var correlationID = Guid.NewGuid().ToString();

                _logger.LogInformation($"{correlationID},Begin Something");

                var processingTime = rnd.Next(0, 10000);
                Task.Delay(processingTime).Wait();

                _logger.LogInformation($"{correlationID},Completed Something");

                var waitBeforeNext = rnd.Next(0, 30000);
                Task.Delay(waitBeforeNext).Wait();
            }
        }
    }

    
}
