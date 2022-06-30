using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace lastDatabase
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(x =>
            {
                x.Service<databaseUpload>(s =>
                {
                    s.ConstructUsing(database => new databaseUpload());
                    s.WhenStarted(database => database.Start());
                    s.WhenStopped(database => database.Stop());
                });

                x.RunAsLocalSystem();

                x.SetServiceName("ExcelToDatabase");
                x.SetDisplayName("Excel to Database");
                x.SetDescription("A service which uptades the database according to Excel");
            });

            int exitCodeValue = (int)Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
