using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace qManager_DHCP_Agent.lib
{
    class common
    {
        public bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }

        public string GetLocalhostFqdn()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            return string.IsNullOrWhiteSpace(ipProperties.DomainName) ? ipProperties.HostName : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }
    }
}
