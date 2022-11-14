﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

namespace qManager_DHCP_Agent.lib.ws.responder
{
    public class pendingResponse
    {
        public System.Net.WebSockets.Managed.ClientWebSocket ws { get; set; }
        public ArraySegment<byte> message { get; set; }
    }
    class responsehandler
    {
        private static List<pendingResponse> sendqueue = new List<pendingResponse>();
        private static bool sending = false;
        public async Task Send(System.Net.WebSockets.Managed.ClientWebSocket ws, ArraySegment<byte> message)
        {
            pendingResponse pr = new pendingResponse();
            pr.ws = ws;
            pr.message = message;
            sendqueue.Add(pr);
            //Console.WriteLine("Send queue is now " + sendqueue.Count);
            if (sending == false)
            {
                await sendHandler();
            }
            return;
            /*await ws.SendAsync(
                message,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None
            );*/
        }

        private async Task sendHandler()
        {
            try
            {
                sending = true;
                for (int i = sendqueue.Count - 1; i >= 0; i--)
                {
                    //Thread.Sleep(4000);
                    //Console.WriteLine("sending message");
                    await sendqueue[i].ws.SendAsync(
                        sendqueue[i].message,
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                    Console.WriteLine("Removing index:");
                    Console.WriteLine(i);
                    sendqueue.RemoveAt(i);
                }
                sending = false;
                //Console.WriteLine("Send queue is now " + sendqueue.Count);
            }
            catch (Exception e)
            {
                sending = false;
                lib.log el = new lib.log();
                el.write(e.ToString(), Environment.StackTrace, "error");
            }
        }
    }
}
