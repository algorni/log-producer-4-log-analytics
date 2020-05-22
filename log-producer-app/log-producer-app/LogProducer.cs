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
            while (true)
            {
                _logger.LogInformation("Something Done");

                Task.Delay(1000).Wait();
            }
        }
    }

    
}
