using Microsoft.Diagnostics.EventFlow;
using System.ServiceProcess;
using System.Threading;

namespace ClusterMonitor
{
    public class ClusterMonitoringService : ServiceBase
    {
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        private Thread thread;

        private static void Main()
        {
            Run(new ClusterMonitoringService());
        }

        protected override void OnStart(string[] args)
        {
            this.thread = new Thread(this.StartEventFlowPipeline)
            {
                Name = "Eventflow Worker Thread",
                IsBackground = true
            };
            this.thread.Start();
        }

        protected override void OnStop()
        {
            this.shutdownEvent.Set();
            // Gives the diagnostics pipeline a 30 sec. timeout to shut down
            if (!this.thread.Join(30000))
            {
                this.thread.Abort();
            }
        }

        private void StartEventFlowPipeline()
        {
            using (DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json"))
                this.shutdownEvent.WaitOne();
        }
    }
}
