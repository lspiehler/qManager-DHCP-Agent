using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Dynamic;
using Newtonsoft.Json;

namespace qManager_DHCP_Agent.lib.ws
{
    class handler
    {
        //responderlib.CachedResponse cr = new responderlib.CachedResponse();
        lib.ws.responder.actionresponse ar = new lib.ws.responder.actionresponse();
        lib.ws.responder.queuedresponse qr = new lib.ws.responder.queuedresponse();

        public async Task wsRequestHandler(System.Net.WebSockets.Managed.ClientWebSocket clientWebSocket, ArraySegment<byte> bytesReceived, WebSocketReceiveResult result)
        {
            processMessage(clientWebSocket, bytesReceived, result);
        }

        private async Task processMessage(System.Net.WebSockets.Managed.ClientWebSocket clientWebSocket, ArraySegment<byte> bytesReceived, WebSocketReceiveResult result)
        {
            String response = Encoding.UTF8.GetString(bytesReceived.Array, 0, result.Count);

            Console.WriteLine(DateTime.Now.ToString() + " " + response);

            //dynamic message = JsonConvert.DeserializeObject<dynamic>(response);
            dynamic message = JsonConvert.DeserializeObject<ExpandoObject>(response);

            Console.WriteLine(DateTime.Now.ToString() + " " + message.type);
            Console.WriteLine(DateTime.Now.ToString() + " " + message.body.path);

            if (message.type == "request")
            {
                if (message.body.path == "/register")
                {
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/dhcp/lease/delete")
                {
                    //Console.WriteLine(DateTime.Now.ToString() + " got to lease handler");
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/dhcp/lease/list")
                {
                    //Console.WriteLine(DateTime.Now.ToString() + " got to lease handler");
                    qr.ProcessResponse(clientWebSocket, message);
                }
                else if (message.body.path == "/dhcp/lease/reserve")
                {
                    //Console.WriteLine(DateTime.Now.ToString() + " got to lease handler");
                    ar.ProcessResponse(clientWebSocket, message);
                }
                else
                {
                    //do nothing
                }
            }
            else if (message.type == "response")
            {

            }
            else if (message.type == "ping")
            {

            }
            else
            {
                //do nothing
            }
        }
    }
}
