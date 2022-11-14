using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qManager_DHCP_Agent
{
    public class WindowsService
    {
        //private string _url = "http://127.0.0.1";
        //private int _port = 8080;
        //private NancyHost _nancy;
        //private WebServiceHost _host;
        lib.ws.client wsclient = new lib.ws.client();

        public WindowsService()
        {
            //var builder = WebApplication.CreateBuilder(args);

            //var uri = new Uri($"{_url}:{_port}/");
            //_nancy = new NancyHost(uri);

            //_host = new WebServiceHost(new NancyWcfGenericService(), uri);

            //var _host = new WebServiceHost(new NancyWcfGenericService(new DefaultNancyBootstrapper()), uri);

            //var binding = new WebHttpBinding();
            //binding.Security.Mode = WebHttpSecurityMode.Transport;
            //binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            //_host.AddServiceEndpoint(typeof(NancyWcfGenericService), binding, "");

            //_host.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, "ea 45 c4 ad c1 2e 8e 2d e8 c5 92 c6 ef 66 0c 05 57 19 18 e0");
        }

        public void Start()
        {
            //_nancy.Start();
            wsclient.initiateWebSocket();
            //_host.Open();
        }

        public void Stop()
        {
            //_nancy.Stop();
            wsclient.closeSocket();
            //_host.Close();
        }

        /*static void Main(string[] args)
        {
            var p = new WebService();
            p.Start();
        }*/
    }
}
