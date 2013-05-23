using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// Call SOAP request to the URL over HTTP
    /// </summary>
    public class SoapHelper
    {
        /// <summary>
        /// Gets the web response from the SOAP web service
        /// </summary>
        /// <param name="soapEnvelope">The xml SOAP Envelope</param>
        /// <param name="soapAction">The SOAP Action</param>
        /// <returns></returns>
        public static WebResponse HttpSOAPRequest(string soapEnvelope, string soapAction)
        {
            string url = "http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";

            XmlDocument doc = new XmlDocument();
            doc.Load(new System.IO.StringReader(soapEnvelope));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            //if (proxy != null) req.Proxy = new WebProxy(proxy, true);

            // if SOAPAction header is required, add it here...
            if (!string.IsNullOrEmpty(soapAction))
            {
                req.Headers.Add("SOAPAction", soapAction);
            }

            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";
            Stream stm = req.GetRequestStream();

            doc.Save(stm);
            stm.Close();
            WebResponse resp = req.GetResponse();
            //stm = resp.GetResponseStream();
            //StreamReader r = new StreamReader(stm);
            //// process SOAP return doc here. For now, we'll just send the XML out to the browser ...
            //string result = r.ReadToEnd();

            return resp;
        }

        /// <summary>
        /// Gets the web response from the SOAP web service
        /// </summary>
        /// <param name="url">the url of the WSDL file</param>
        /// <param name="soapEnvelope">The xml SOAP Envelope</param>
        /// <param name="soapAction">The SOAP Action</param>
        /// <param name="outFileName">The output file name</param>
        /// <returns></returns>
        public static void HttpSOAPToFile(string url, string soapEnvelope, string soapAction, string outFileName)
        {
            //string url = "http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";

            XmlDocument doc = new XmlDocument();
            doc.Load(new System.IO.StringReader(soapEnvelope));

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            //if (proxy != null) req.Proxy = new WebProxy(proxy, true);

            // if SOAPAction header is required, add it here...
            if (!string.IsNullOrEmpty(soapAction))
            {
                req.Headers.Add("SOAPAction", soapAction);
            }

            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";
            Stream stm = req.GetRequestStream();
            doc.Save(stm);
            stm.Close();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            try
            {
                // Hope GetEncoding() knows how to parse the CharacterSet
                Encoding encoding = Encoding.GetEncoding(resp.CharacterSet);
                StreamReader reader = new StreamReader(resp.GetResponseStream(), encoding);
                using (StreamWriter sw = new StreamWriter(outFileName, false, encoding))
                {
                    sw.Write(reader.ReadToEnd());
                    sw.Flush();
                    sw.Close();
                }
            }
            finally
            {
                resp.Close();
            }
        }
    }
}
