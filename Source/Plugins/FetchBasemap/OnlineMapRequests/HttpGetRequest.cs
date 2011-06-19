﻿using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;

namespace FetchBasemap.OnlineMapRequests
{
    class HttpGetRequest : HttpRequest
    {

        public HttpGetRequest(string endpointUrl)
            : base(endpointUrl)
        {
        }

        public override HttpWebResponse IssueRequest()
        {

            // Build a string with all the params, properly encoded.
            var p = new StringBuilder();
            foreach (var key in paramTable.Keys.Cast<string>().Where(key => paramTable[key] != null))
            {
                p.Append(key);
                p.Append("=");
                p.Append(HttpUtility.UrlEncode(paramTable[key].ToString()));
                p.Append("&");
            }

            var req = WebRequest.Create(EndpointUrl+'?'+p) as HttpWebRequest;

            try
            {
                if (req != null)
                {
                    var response = req.GetResponse() as HttpWebResponse;
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return null;
            }

            return null;
        }
    }
}
