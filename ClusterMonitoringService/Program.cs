using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.EventFlow;
using Microsoft.Diagnostics.EventFlow.ServiceFabric;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ClusterMonitoringService
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        //private static void Main()
        //{
        //    using (var pipeline = ServiceFabricDiagnosticPipelineFactory.CreatePipeline("eventFlowConfig.json"))
        //    {
        //        ServiceRuntime.RegisterServiceAsync("ClusterMonitoringServiceType",
        //            context => new ClusterMonitoringService(context)).GetAwaiter().GetResult();

        //        // Prevents this host process from terminating so services keep running.
        //        Thread.Sleep(Timeout.Infinite);

        //    }
        //}

        public static void Main(string[] args)
        {
            try
            {
                using (ManualResetEvent terminationEvent = new ManualResetEvent(initialState: false))
                using (var diagnosticsPipeline = ServiceFabricDiagnosticPipelineFactory.CreatePipeline("MyApplication-MyService-DiagnosticsPipeline"))
                {
                    Console.CancelKeyPress += (sender, eventArgs) => Shutdown(diagnosticsPipeline, terminationEvent);

                    AppDomain.CurrentDomain.UnhandledException += (sender, unhandledExceptionArgs) =>
                    {
                        ServiceEventSource.Current.UnhandledException(unhandledExceptionArgs.ExceptionObject?.ToString() ?? "(no exception information)");
                        Shutdown(diagnosticsPipeline, terminationEvent);
                    };

                    ServiceRuntime.RegisterServiceAsync("ClusterMonitoringServiceType",
                                context => new ClusterMonitoringService(context)).GetAwaiter().GetResult();

                    terminationEvent.WaitOne();
                }
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void Shutdown(IDisposable disposable, ManualResetEvent terminationEvent)
        {
            try
            {
                disposable.Dispose();
            }
            finally
            {
                terminationEvent.Set();
            }
        }
    }
}
