using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseComparer
{
    public class FileLogger : ILogger
    {
        readonly string fileName;
        public FileLogger(string fileName)
        {
            this.fileName = fileName;
            File.AppendAllText(fileName, Environment.NewLine);
            File.AppendAllText(fileName, $"########## {DateTime.Now} ##########");

        }
        public void Log(string log)
        {
            File.AppendAllText(fileName,Environment.NewLine);
            File.AppendAllText(fileName,log);

        }

    }
}
