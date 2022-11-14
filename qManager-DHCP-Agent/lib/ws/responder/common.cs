﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qManager_DHCP_Agent.lib.ws.responder
{
    public class RequestMessage
    {
        public string id { get; set; }
        public string type { get; set; }
        public string body { get; set; }
    }

    public class ResponseMessage
    {
        public string id { get; set; }
        public string type { get; set; }
        public string body { get; set; }
    }

    public class HashTableResponseBody
    {
        public string result { get; set; }
        public string message { get; set; }
        //public Dictionary<string, printerlib.GetPrinter> data { get; set; }
        public Hashtable data = new Hashtable();
    }
    public class HashTableResponse
    {
        public string id { get; set; }
        public string type { get; set; }
        public HashTableResponseBody body { get; set; }
    }
    class common
    {
    }
}
