using Microsoft.Diagnostics.EventFlow;
using System.ServiceProcess;
using System.Threading;

namespace ClusterMonitor
{
    public partial class clusterMonitoringService : ServiceBase
    {
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;

        public clusterMonitoringService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(StartEventFlowPipeline);
            _thread.Name = "Eventflow Worker Thread";
            _thread.IsBackground = true;
            _thread.Start();
        }

        protected override void OnStop()
        {
            _shutdownEvent.Set();
            // Gives the diagnostics pipeline a 30 sec. timeout to shut down
            if (!_thread.Join(30000))
            {
                _thread.Abort();
            }
        }

        private void StartEventFlowPipeline()
        {
            using (var pipeline = DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json"))
            {
                _shutdownEvent.WaitOne();
            }
        }
    }
}
