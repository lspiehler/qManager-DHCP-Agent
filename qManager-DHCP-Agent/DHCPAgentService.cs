using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace qManager_DHCP_Agent
{
    public partial class DHCPAgentService : ServiceBase
    {
        WindowsService instance = new WindowsService();
        public DHCPAgentService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            instance.Start();
        }

        protected override void OnStop()
        {
            instance.Stop();
        }
    }
}
