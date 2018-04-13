using System.ServiceProcess;

namespace ClusterMonitor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new clusterMonitoringService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
