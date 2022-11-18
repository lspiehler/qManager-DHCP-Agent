using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace qManager_DHCP_Agent.lib.dhcp
{
    class scope
    {
        private runspace psr = new runspace();
        public bool checkFailoverRelationship(string scopeid)
        {
            Runspace psRunspace = psr.Get();

            try
            {
                using (var ps1 = PowerShell.Create())
                {
                    ps1.Runspace = psRunspace;
                    ps1.AddCommand("Get-DhcpServerv4Failover").AddParameter("ScopeId", scopeid);

                    if (ps1.HadErrors)
                    {
                        /*List<string> errors = new List<string>();
                        for (int i = 0; i < ps1.Streams.Error.Count; i++)
                        {
                            errors.Add(ps1.Streams.Error[i].ToString());
                        }
                        lib.log el = new lib.log();
                        el.write(String.Join("", errors), Environment.StackTrace, "error");
                        return String.Join("", errors);*/
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                lib.log el = new lib.log();
                el.write(e.ToString(), "", "error");
                return false;
                //return e.ToString();
            }
        }

        public string replicate(string scopeid)
        {
            Runspace psRunspace = psr.Get();

            try
            {
                using (var ps1 = PowerShell.Create())
                {
                    ps1.Runspace = psRunspace;
                    ps1.AddCommand("Invoke-DhcpServerv4FailoverReplication").AddParameter("ScopeId", scopeid).AddParameter("-Force");

                    Collection<System.Management.Automation.PSObject> PSOutput1 = ps1.Invoke();

                    if (ps1.HadErrors)
                    {
                        List<string> errors = new List<string>();
                        for (int i = 0; i < ps1.Streams.Error.Count; i++)
                        {
                            errors.Add(ps1.Streams.Error[i].ToString());
                        }
                        lib.log el = new lib.log();
                        el.write(String.Join("", errors), Environment.StackTrace, "error");
                        return String.Join("", errors);
                        //return null;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                lib.log el = new lib.log();
                el.write(e.ToString(), "", "error");
                //return false;
                return e.ToString();
            }
        }
    }
}
