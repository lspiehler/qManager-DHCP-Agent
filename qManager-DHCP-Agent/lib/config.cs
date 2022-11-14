using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace qManager_DHCP_Agent.lib
{
    public class Options
    {
        [Option('p', "proxy", Required = false, HelpText = "Use a proxy for the connection to qManager")]
        public string Proxy { get; set; }

        [Option('s', "server", Required = false, HelpText = "The FQDN for a qManager server")]
        public string Server { get; set; }

        [Option('c', "cert", Required = false, HelpText = "The certificate used for authentication to qManager")]
        public string Certificate { get; set; }
    }
    public sealed class config
    {
        private static readonly config instance = new config();
        private static Dictionary<string, string> conf;

        static config()
        {
        }
        private config()
        {
        }
        public static config Instance
        {
            get
            {
                return instance;
            }
        }

        public Dictionary<string, string> getConfig()
        {
            return conf;
        }
        public void loadConfig(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       /*if (o.Proxy == null)
                       {
                           //Console.WriteLine($"Verbose output enabled. Current Arguments: -v {o.Verbose}");
                           //Console.WriteLine("Quick Start Example! App is in Verbose mode!");
                           Console.WriteLine("Proxy is null");
                       }
                       else
                       {
                           //Console.WriteLine($"Current Arguments: -v {o.Verbose}");
                           //Console.WriteLine("Quick Start Example!");
                           Console.WriteLine(o.Proxy);
                       }*/
                       conf = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(o));
                   });
        }
    }
}
