using System;
using System.Collections.Generic;
using System.Text;

namespace log_producer_app
{
    public class LogProducerConfig
    {
        public int MaxOperationDurationInMillieconds { get; set; }
        public int MaxTimeBetweenOperationInMillieconds { get; set; }
    }
}
