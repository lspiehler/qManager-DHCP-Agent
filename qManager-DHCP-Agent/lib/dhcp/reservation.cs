using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections;
using System.Collections.ObjectModel;

namespace qManager_DHCP_Agent.lib.dhcp
{
    class reservation
    {
        private runspace psr = new runspace();
        private scope scopelib = new scope();
        public string delete(Dictionary<string, dynamic> options)
        {
            Runspace psRunspace = psr.Get();

            try
            {
                using (var ps1 = PowerShell.Create())
                {
                    ps1.Runspace = psRunspace;
                    ps1.AddCommand("Get-DhcpServerv4Lease");

                    Dictionary<string, string> allowedprops = new Dictionary<string, string>();
                    allowedprops.Add("clientid", "ClientId");
                    allowedprops.Add("scopeid", "ScopeId");
                    allowedprops.Add("ipaddress", "IPAddress");

                    var keys1 = options.Keys;

                    foreach (var k in keys1)
                    {
                        if (allowedprops.ContainsKey(k.ToLower()))
                        {
                            Console.WriteLine(allowedprops[k]);
                            Console.WriteLine(options[k]);
                            ps1.AddParameter(allowedprops[k], options[k]);
                        }
                    }

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
                        return errors[0];
                    }
                    if (PSOutput1.Count <= 0)
                    {
                        return "Failed to delete lease because more than one (" + PSOutput1.Count + ") was found for the given criteria. Only 1 was expected.";
                    }
                    else if (PSOutput1.Count > 1)
                    {
                        return "The lease could not be found on the server";
                    }
                    else
                    {
                        foreach (System.Management.Automation.PSObject obj1 in PSOutput1)
                        {
                            using (var ps2 = PowerShell.Create())
                            {
                                ps2.Runspace = psRunspace;
                                ps2.AddCommand("Remove-DhcpServerv4Reservation").AddParameter("ScopeId", obj1.Properties["ScopeId"].Value).AddParameter("ClientId", obj1.Properties["ClientId"].Value);

                                Collection<System.Management.Automation.PSObject> PSOutput2 = ps2.Invoke();

                                if (ps2.HadErrors)
                                {
                                    List<string> errors = new List<string>();
                                    for (int i = 0; i < ps2.Streams.Error.Count; i++)
                                    {
                                        errors.Add(ps2.Streams.Error[i].ToString());
                                    }
                                    lib.log el = new lib.log();
                                    el.write(String.Join("", errors), Environment.StackTrace, "error");
                                    return errors[0];
                                }
                                else
                                {
                                    if (scopelib.checkFailoverRelationship(options["scopeid"]))
                                    {
                                        Console.WriteLine("Must replicate");
                                        var repres = scopelib.replicate(options["scopeid"]);
                                        if (repres == null)
                                        {
                                            return null;
                                        }
                                        else
                                        {
                                            return repres;
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No failover relationship for " + options["scopeid"]);
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                lib.log el = new lib.log();
                el.write(e.ToString(), "", "error");
                return e.ToString();
            }
            return null;
        }
    }
}
