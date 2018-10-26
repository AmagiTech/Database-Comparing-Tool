using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseComparer
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string log)
        {
            Console.WriteLine(log);
        }
    }
}
