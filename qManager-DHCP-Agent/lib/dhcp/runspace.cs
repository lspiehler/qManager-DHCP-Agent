using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Runspaces;

namespace qManager_DHCP_Agent.lib.dhcp
{
    class runspace
    {
        private static InitialSessionState sessionState;
        private static Runspace psRunspace;

        public Runspace Get()
        {
            if (sessionState == null)
            {
                Console.WriteLine(DateTime.Now.ToString() + " CREATING NEW RUNSPACE");
                sessionState = InitialSessionState.CreateDefault();
                //Console.WriteLine(sessionState.ApartmentState);
                sessionState.ImportPSModule(new string[] { "DHCPServer" });
                psRunspace = RunspaceFactory.CreateRunspace(sessionState);
                psRunspace.Open();
                return psRunspace;
            }
            else
            {
                Console.WriteLine(DateTime.Now.ToString() + " USING EXISTING RUNSPACE");
                return psRunspace;
            }
        }
    }
}
