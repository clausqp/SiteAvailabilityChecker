using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using log4net;

namespace SiteAvailabilityChecker
{
    internal class Program
    {
        private static ILog logger = log4net.LogManager.GetLogger("Program");


        static void Main(string[] args)
        {
            Program.ServiceStartup(args);
        }


        public static void ServiceStartup(string[] args)
        {
            if ((args.Count() >= 1 && args[0] == "/Run") || Environment.UserInteractive)
            {
                CheckerCore scheduler = new CheckerCore();

                scheduler.Start(args);

                Console.WriteLine("Press enter to stop");
                Console.ReadKey();

                scheduler.Stop();
            }
            else if (Debugger.IsAttached)
            {
                CheckerCore scheduler = new CheckerCore();

                scheduler.Start(args);

                Console.ReadKey();

                scheduler.Stop();
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { new CheckerCore() };
                ServiceBase.Run(ServicesToRun);
            }
        }
            

    }
}
