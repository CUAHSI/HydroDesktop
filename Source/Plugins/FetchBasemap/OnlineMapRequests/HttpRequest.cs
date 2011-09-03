using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;

namespace FetchBasemap.OnlineMapRequests
{
    abstract class HttpRequest
    {
        public string EndpointUrl { get; set; }

        public Hashtable paramTable { get; set; }

        public HttpRequest(string endpointUrl)
        {
            this.EndpointUrl = endpointUrl;
        }

        abstract public HttpWebResponse IssueRequest();

    }
}
