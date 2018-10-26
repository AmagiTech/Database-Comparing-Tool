using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseComparer
{
    public class Logger : ILogger
    {
        readonly List<ILogger> loggers;
        public Logger(params ILogger[] loggers)
        {
            if (loggers != null)
                this.loggers = loggers.ToList();
        }
        public void Log(string log)
        {
            if (loggers != null)
                loggers.ForEach(q => q.Log(log));
        }
    }
}
