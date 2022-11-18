using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace qManager_DHCP_Agent.lib.dhcp
{
    public class leaseobj
    {
        public string IPAddress { get; set; }
        public string ScopeId { get; set; }
        public string AddressState { get; set; }
        public string ClientId { get; set; }
        public string ClientType { get; set; }
        public string Description { get; set; }
        public string DnsRegistration { get; set; }
        public string DnsRR { get; set; }
        public string HostName { get; set; }
        public DateTime? LeaseExpiryTime { get; set; }
        public bool NapCapable { get; set; }
        public string PolicyName { get; set; }
        public DateTime? ProbationEnds { get; set; }
        public string ServerIP { get; set; }
    }
    class lease
    {
        private runspace psr = new runspace();
        private scope scopelib = new scope();
        private reservation reservationlib = new reservation();
        private Dictionary<string, Dictionary<string, List<leaseobj>>> cachedobjects = new Dictionary<string, Dictionary<string, List<leaseobj>>>();
        private static readonly List<string> allowedindexes = new List<string> { "ClientId", "IPAddress" };
        public string delete(Dictionary<string, dynamic> options)
        {
            Runspace psRunspace = psr.Get();

            try
            {
                //if lease is a reservation, call function to delete reservation instead
                if (options.ContainsKey("reservation"))
                {
                    if (options["reservation"] == true)
                    {
                        Console.WriteLine("Calling reservation deletion");
                        return reservationlib.delete(options);
                    }
                }

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
                            //if lease is a reservation, call function to delete reservation instead
                            if (obj1.Properties["AddressState"].Value.ToString().ToLower().IndexOf("reservation") >= 0)
                            {
                                Console.WriteLine("Calling reservation deletion");
                                return reservationlib.delete(options);
                            }
                            using (var ps2 = PowerShell.Create())
                            {
                                ps2.Runspace = psRunspace;
                                ps2.AddCommand("Remove-DhcpServerv4Lease").AddParameter("ScopeId", obj1.Properties["ScopeId"].Value).AddParameter("ClientId", obj1.Properties["ClientId"].Value);

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
                                        Console.WriteLine("No failover relationship found for " + options["scopeid"]);
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
        public string reserve(Dictionary<string, dynamic> options)
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
                    if (PSOutput1.Count <= 0) {
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
                            if (obj1.Properties["AddressState"].Value.ToString().ToLower().IndexOf("reservation") >= 0)
                            {
                                Console.WriteLine("The reservation already exists");
                                return "The reservation already exists";
                            }
                            using (var ps2 = PowerShell.Create())
                            {
                                ps2.Runspace = psRunspace;
                                ps2.AddCommand("Add-DhcpServerv4Reservation").AddParameter("ScopeId", obj1.Properties["ScopeId"].Value).AddParameter("ClientId", obj1.Properties["ClientId"].Value).AddParameter("IPAddress", obj1.Properties["IPAddress"].Value);

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
                                        Console.WriteLine("No failover relationship found for " + options["scopeid"]);
                                        return null;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                lib.log el = new lib.log();
                el.write(e.ToString(), "", "error");
                return e.ToString();
            }
            return null;
        }
        public Dictionary<string, List<leaseobj>> GetAll(bool updatecache, string index)
        {
            Dictionary<string, List<leaseobj>> objects = new Dictionary<string, List<leaseobj>>();
            if (allowedindexes.IndexOf(index) < 0)
            {
                lib.log el = new lib.log();
                el.write("An invalid index was specified (" + index + ") when requesting the list of leases. Only " + String.Join(",", allowedindexes) + " are valid indexes", Environment.StackTrace, "error");
                return objects;
            }
            if (updatecache == true || cachedobjects.ContainsKey(index) == false || cachedobjects[index] == null)
            {
                //Console.WriteLine(DateTime.Now);
                Runspace psRunspace = psr.Get();

                try
                {
                    using (var ps1 = PowerShell.Create())
                    {
                        ps1.Runspace = psRunspace;
                        ps1.AddCommand("Get-DhcpServerv4Scope");

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
                            return objects;
                        }

                        foreach (System.Management.Automation.PSObject obj1 in PSOutput1)
                        {
                            /*List<System.Management.Automation.PSPropertyInfo> properties = obj.Properties.ToList();
                            for (int i = 0; i < properties.Count; i++)
                            {
                                Console.WriteLine(properties[i].Name);
                            }*/
                            //scopes.Add(obj1.Properties["ScopeId"].Value.ToString());
                            //Console.WriteLine(obj.Properties["Name"].Value.ToString());
                            using (var ps2 = PowerShell.Create())
                            {
                                ps2.Runspace = psRunspace;
                                ps2.AddCommand("Get-DhcpServerv4Lease").AddParameter("ScopeId", obj1.Properties["ScopeId"].Value.ToString());

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
                                    return objects;
                                }

                                foreach (System.Management.Automation.PSObject obj2 in PSOutput2)
                                {
                                    /*List<System.Management.Automation.PSPropertyInfo> properties = obj.Properties.ToList();
                                    for (int i = 0; i < properties.Count; i++)
                                    {
                                        Console.WriteLine(properties[i].Name);
                                    }*/
                                    //scopes.Add(obj2.Properties["ClientId"].Value.ToString());
                                    leaseobj getobject = new leaseobj();
                                    if (obj2 != null)
                                    {
                                        List<System.Management.Automation.PSPropertyInfo> properties = obj2.Properties.ToList();
                                        for (int i = 0; i < properties.Count; i++)
                                        //for (int i = 0; i < 1; i++)
                                        {
                                            System.Management.Automation.PSPropertyInfo psobject = obj2.Properties[properties[i].Name];
                                            //Console.WriteLine(properties[i].Name + " - " + psobject.GetType().ToString());
                                            if (psobject.GetType().ToString() == "System.Management.Automation.PSAdaptedProperty")
                                            {
                                                if (properties[i].Name.IndexOf("_") != 0)
                                                {
                                                    var getproperty = getobject.GetType().GetProperty(properties[i].Name);
                                                    if (getproperty != null)
                                                    {
                                                        if (psobject.Value == null)
                                                        {
                                                            getproperty.SetValue(getobject, null);
                                                        }
                                                        else
                                                        {
                                                            getproperty.SetValue(getobject, psobject.Value);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    string key = obj2.Properties[index].Value.ToString();
                                    if (objects.ContainsKey(key))
                                    {
                                        objects[key].Add(getobject);
                                    }
                                    else
                                    {
                                        objects.Add(key, new List<leaseobj> { getobject });
                                    }
                                }
                                ps2.Dispose();
                            }
                        }
                        ps1.Dispose();
                    }
                    if (cachedobjects.ContainsKey(index))
                    {
                        cachedobjects[index] = objects;
                    }
                    else
                    {
                        cachedobjects.Add(index, objects);
                    }
                    //Console.WriteLine(JsonConvert.SerializeObject(new Hashtable(objects)));
                    //Console.WriteLine(DateTime.Now);
                    return objects;
                }
                catch (Exception e)
                {
                    lib.log el = new lib.log();
                    el.write(e.ToString(), "", "error");
                    return objects;
                }
            }
            else
            {
                return cachedobjects[index];
            }
        }
    }
}
