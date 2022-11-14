using Newtonsoft.Json;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace qManager_DHCP_Agent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            lib.config confighandler = lib.config.Instance;
            confighandler.loadConfig(args);

            /*lib.dhcp.lease lease = new lib.dhcp.lease();
            lease.GetAll(false, "ClientId");
            lease.GetAll(false, "IPAddress");*/

            if (Environment.UserInteractive)
            {
                lib.ws.client wsclient = new lib.ws.client();
                wsclient.initiateWebSocket();

                Console.WriteLine("Opening socket");
                /*Console.WriteLine("Starting HTTP listener...");

                //var httpServer = new HttpServer();
                //httpServer.Start();
                WebService instance = new WebService();
                instance.Start();*/

                var autoResetEvent = new AutoResetEvent(false);
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    // cancel the cancellation to allow the program to shutdown cleanly
                    eventArgs.Cancel = true;
                    autoResetEvent.Set();
                };

                // main blocks here waiting for ctrl-C
                autoResetEvent.WaitOne();
                Console.WriteLine("Now shutting down");

                //wsclient.getSocket().Abort();
                //if (wsclient.getState() == "Open") {
                wsclient.closeSocket();
                //}

                //while (Program._keepRunning) { }

                //httpServer.Stop();
                //instance.Stop();

                //Console.WriteLine("Exiting gracefully...");
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new DHCPAgentService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
