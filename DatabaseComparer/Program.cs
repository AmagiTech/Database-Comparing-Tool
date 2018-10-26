using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseComparer
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "database-comparasion-results.log";
            var clogger = new ConsoleLogger();          
            var logger = new Logger(clogger,new FileLogger(logFile));
            clogger.Log($"Log File Created:{logFile}");
            var response = string.Empty;
            do
            {
                Console.WriteLine("Choose one!:");
                Console.WriteLine("(1) Get database information from database.");
                Console.WriteLine("(2) Compare two database.");
                Console.WriteLine("(3) Quit.");
                response = Console.ReadLine().Trim();
                switch (response)
                {
                    case "1":
                        ComparingTools.GetDatabaseInformationFromDatabase(logger);
                        break;
                    case "2":
                        ComparingTools.CompareDatabases(logger);
                        break;
                }

            } while (response != "3");

        }
    }
}
