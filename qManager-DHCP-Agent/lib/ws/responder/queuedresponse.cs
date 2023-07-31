using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qManager_DHCP_Agent.lib.ws.responder
{
    class queuedresponse
    {
        lib.dhcp.lease leaselib = new lib.dhcp.lease();
        Dictionary<string, ResponderLibCache> cachedtypes = new Dictionary<string, ResponderLibCache>();
        private lib.ws.responder.responsehandler wsresponder = new lib.ws.responder.responsehandler();
        private lib.common commonlib = new lib.common();
        private class ResponderLibCache
        {
            //public string path { get; set; }
            public Dictionary<string, System.Net.WebSockets.Managed.ClientWebSocket> clients { get; set; }
            //public HashTableResponseBody cachedresponse { get; set; }
        }
        public async Task ProcessResponse(System.Net.WebSockets.Managed.ClientWebSocket ws, dynamic rm)
        {
            string path = rm.body.path;
            bool updatecache = false;
            if (commonlib.PropertyExists(rm.body.options, "updatecache"))
            {
                updatecache = rm.body.options.updatecache;
            }

            if (cachedtypes.ContainsKey(path) == false)
            {
                Console.WriteLine(DateTime.Now.ToString() + " Creating key " + path);
                ResponderLibCache rlc = new ResponderLibCache();
                //rlc.clients = new List<System.Net.WebSockets.Managed.ClientWebSocket>();
                //rlc.clients.Add(rm.id, ws);
                rlc.clients = new Dictionary<string, System.Net.WebSockets.Managed.ClientWebSocket>();
                //rlc.cachedresponse = null;
                cachedtypes.Add(path, rlc);
            }

            if (cachedtypes[path].clients.Count > 0)
            {
                // just add pending request to the queue
                Console.WriteLine(DateTime.Now.ToString() + " Adding pending request to the queue");
                cachedtypes[path].clients.Add(rm.id, ws);
            }
            else
            {
                cachedtypes[path].clients.Add(rm.id, ws);

                HashTableResponseBody body = new HashTableResponseBody();
                if (path == "/dhcp/lease/list")
                {
                    //PMPrintQueues pmprintqueues = new PMPrintQueues();

                    //Hashtable myPrintQueues = pmprintqueues.getQueues();
                    //pslib.getPrinter getprinter = new pslib.getPrinter();
                    try
                    {
                        string reqindex = "ClientId";
                        if (commonlib.PropertyExists(rm.body.options, "index"))
                        {
                            reqindex = rm.body.options.index;
                        }
                        body.result = "success";
                        body.message = null;
                        body.data = new Hashtable(leaselib.GetAll(updatecache, reqindex));
                        //Console.WriteLine(JsonConvert.SerializeObject(rm, Formatting.Indented));
                    }
                    catch (Exception e)
                    {
                        body.result = "error";
                        body.message = e.ToString();
                        body.data = null;

                        lib.log el = new lib.log();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }
                }
                else if (path == "/printer/port/list")
                {

                }
                else
                {
                    body.result = "error";
                    body.message = "Unrecognized request to " + path;
                    body.data = null;

                }

                Console.WriteLine(DateTime.Now.ToString() + " Responding to all queued requests");

                //for (int i = cachedtypes[path].clients.Count - 1; i >= 0; i--)
                while (cachedtypes[path].clients.Count > 0)
                {
                    List<string> clientkeys = new List<string>(cachedtypes[path].clients.Keys);
                    int i = clientkeys.Count - 1;

                    HashTableResponse resp = new HashTableResponse();

                    //cachedtypes[path].cachedresponse = body;

                    resp.id = clientkeys[i];
                    resp.type = "response";
                    resp.body = body;

                    string jsonresp = JsonConvert.SerializeObject(resp, Formatting.Indented);

                    //Console.WriteLine(jsonresp);

                    ArraySegment<byte> bytestosend = new ArraySegment<byte>(
                        Encoding.UTF8.GetBytes(jsonresp)
                    );

                    //Console.WriteLine("Sending response to message id " + clientkeys[i]);
                    /*try
                    {
                        await cachedtypes[path].clients[clientkeys[i]].SendAsync(
                            bytestosend,
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );

                        //cachedtypes[path].clients[clientkeys[i]].Dispose();
                    }
                    catch(Exception e)
                    {
                        errorlog el = new errorlog();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }*/
                    try
                    {
                        await wsresponder.Send(cachedtypes[path].clients[clientkeys[i]], bytestosend);
                        cachedtypes[path].clients.Remove(clientkeys[i]);
                    }
                    catch (Exception e)
                    {
                        cachedtypes[path].clients.Remove(clientkeys[i]);
                        lib.log el = new lib.log();
                        el.write(e.ToString(), Environment.StackTrace, "error");
                    }
                }

                Console.WriteLine(DateTime.Now.ToString() + " " + cachedtypes[path].clients.Count + " remaining");
            }
        }
    }
}
