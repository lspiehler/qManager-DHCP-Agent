using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qManager_DHCP_Agent.lib.ws.responder
{
    class actionresponse
    {
        lib.dhcp.lease leaselib = new lib.dhcp.lease();
        private lib.ws.responder.responsehandler wsresponder = new lib.ws.responder.responsehandler();
        private lib.common commonlib = new lib.common();

        public async Task ProcessResponse(System.Net.WebSockets.Managed.ClientWebSocket ws, dynamic rm)
        {
            string path = rm.body.path;
            HashTableResponseBody body = new HashTableResponseBody();

            if (path == "/dhcp/lease/delete")
            {
                try
                {
                    string result = leaselib.delete(new Dictionary<string, dynamic>(rm.body.options));
                    //string result = printport.Create(rm.body.options.ip, 1, rm.body.options.ip, 9100, true);
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "The lease was deleted successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    lib.log el = new lib.log();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/dhcp/lease/reserve")
            {
                try
                {
                    string result = leaselib.reserve(new Dictionary<string, dynamic>(rm.body.options));
                    //string result = printport.Create(rm.body.options.ip, 1, rm.body.options.ip, 9100, true);
                    if (result == null)
                    {
                        body.result = "success";
                        body.message = "The lease was reserved successfully";
                    }
                    else
                    {
                        body.result = "error";
                        body.message = result;
                    }
                    body.data = new Hashtable()
                    {
                        {"rowId", rm.body.options.rowId },
                        {"server", rm.body.options.server }
                    };
                }
                catch (Exception e)
                {
                    lib.log el = new lib.log();
                    el.write(e.ToString(), Environment.StackTrace, "error");
                }
            }
            else if (path == "/register")
            {
                body.result = "success";
                body.message = null;
                body.data = new Hashtable()
                {
                    {"hostname", commonlib.GetLocalhostFqdn()},
                    {"agentType", "dhcp" },
                    {"agentVersion", "0.04" }
                };
            }
            else
            {
                body.result = "error";
                body.message = "Unrecognized request to " + path;
                body.data = null;
            }

            HashTableResponse resp = new HashTableResponse();

            resp.id = rm.id;
            resp.type = "response";
            resp.body = body;

            string jsonresp = JsonConvert.SerializeObject(resp);

            ArraySegment<byte> bytestosend = new ArraySegment<byte>(
                Encoding.UTF8.GetBytes(jsonresp)
            );

            //Console.WriteLine(jsonresp);

            try
            {
                //Console.WriteLine(DateTime.Now.ToString() + " - Begin");
                /*await ws.SendAsync(
                    bytestosend,
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );*/
                await wsresponder.Send(ws, bytestosend);
                //Console.WriteLine(DateTime.Now.ToString() + " - End");

                //ws.Dispose();
            }
            catch (Exception e)
            {
                lib.log el = new lib.log();
                el.write(e.ToString(), Environment.StackTrace, "error");
            }
        }
    }
}
