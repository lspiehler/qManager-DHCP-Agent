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
        private InitialSessionState sessionState;
        private Runspace psRunspace;

        public Runspace Get()
        {
            if (sessionState == null)
            {
                sessionState = InitialSessionState.CreateDefault();
                //Console.WriteLine(sessionState.ApartmentState);
                sessionState.ImportPSModule(new string[] { "DHCPServer" });
                psRunspace = RunspaceFactory.CreateRunspace(sessionState);
                psRunspace.Open();
                return psRunspace;
            }
            else
            {
                return psRunspace;
            }
        }
    }
}
