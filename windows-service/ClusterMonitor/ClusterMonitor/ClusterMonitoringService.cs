using Microsoft.Diagnostics.EventFlow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ClusterMonitor
{
    public partial class clusterMonitoringService : ServiceBase
    {
        public clusterMonitoringService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            using (var pipeline = DiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json"))
            {
                System.Diagnostics.Trace.TraceWarning("EventFlow is working!");
                Console.ReadLine();
            }
        }

        protected override void OnStop()
        {
            //TODO: Implement shutdown logic? 
        }
    }
}
