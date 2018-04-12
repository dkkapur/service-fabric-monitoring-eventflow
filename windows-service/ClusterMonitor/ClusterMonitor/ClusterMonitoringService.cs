using Microsoft.Diagnostics.EventFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
