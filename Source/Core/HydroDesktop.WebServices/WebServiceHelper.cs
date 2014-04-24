using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace HydroDesktop.WebServices
{
    /// <summary>
    /// This is a helper class providing methods for accessing WaterML SOAP web services
    /// </summary>
    public static class WebServiceHelper
    {
        /// <summary>
        /// Creates a web request for the GetSites method
        /// </summary>
        /// <param name="url">the wsdl URL</param>
        /// <returns>the web request that can be sent to obtain the list of sites</returns>
        public static HttpWebRequest CreateGetSitesRequest(string url)
        {
            url = url.Trim().ToLower();
            //for http request, we need to remove the ?WSDL part from the url
            if (url.EndsWith("?wsdl"))
            {
                url = url.Replace("?wsdl", "");
            }

            //get the valid SOAP namespace
            var soapNamespace = GetCuahsiSoapNamespace(url);

            //create the SOAP Envelope
            var soapEnvelope = CreateSoapEnvelopeForGetSites(soapNamespace);

            //send the SOAP envelope to the service as a xml document
            var doc = new XmlDocument();
            doc.Load(new StringReader(soapEnvelope));

            var req = (HttpWebRequest)WebRequest.Create(url);

            //this is the valid SoapAction header for GetValues web method
            var soapAction = soapNamespace == Properties.Resources.CUAHSI_1_1_Namespace
                                    ? soapNamespace + "GetSitesObject"
                                    : soapNamespace + "GetSites";

            req.Headers.Add("SOAPAction", soapAction);
            req.ContentType = "text/xml";
            req.Method = "POST";
            var stm = req.GetRequestStream();
            doc.Save(stm);
            stm.Close();

            return req;
        }

        /// <summary>
        /// Creates the web request to get site information (which series are available at the site)
        /// </summary>
        /// <param name="url">the web service wsdl url</param>
        /// <param name="fullSiteCode">the full site code in Network:SiteCode format</param>
        /// <returns>the web request for getting the site info</returns>
        public static HttpWebRequest CreateGetSiteInfoRequest(string url, string fullSiteCode)
        {
            url = url.Trim().ToLower();
            //for http request, we need to remove the ?WSDL part from the url
            if (url.EndsWith("?wsdl"))
            {
                url = url.Replace("?wsdl", "");
            }

            //get the valid SOAP namespace
            var soapNamespace = GetCuahsiSoapNamespace(url);

            //create the SOAP Envelope
            var soapEnvelope = CreateSoapEnvelopeForGetSiteInfo(soapNamespace, fullSiteCode);

            //send the SOAP envelope to the service as a xml document
            var doc = new XmlDocument();
            doc.Load(new StringReader(soapEnvelope));

            var req = (HttpWebRequest)WebRequest.Create(url);

            //this is the valid SoapAction header for GetValues web method
            var soapAction = soapNamespace + "GetSiteInfoObject";

            req.Headers.Add("SOAPAction", soapAction);

            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";
            var stm = req.GetRequestStream();
            doc.Save(stm);
            stm.Close();

            return req;
        }


        /// <summary>
        /// Creates the HTTP SOAP web request for GetValues method
        /// </summary>
        /// <param name="url">URL of the web service</param>
        /// <param name="fullSiteCode">full site code (NetworkPrefix:Site)</param>
        /// <param name="fullVariableCode">full variable code (NetworkPrefix:Variable)</param>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <returns>Returns the fully initialized web request object</returns>
        public static HttpWebRequest CreateGetValuesRequest(string url, string fullSiteCode, string fullVariableCode, DateTime startDate, DateTime endDate)
        {
            url = url.Trim().ToLower();
            //for http request, we need to remove the ?WSDL part from the url
            if (url.EndsWith("?wsdl"))
            {
                url = url.Replace("?wsdl", "");
            }

            //get the valid SOAP namespace
            var soapNamespace = GetCuahsiSoapNamespace(url);

            //create the SOAP Envelope
            var soapEnvelope = CreateSoapEnvelope(soapNamespace, fullSiteCode, fullVariableCode, startDate, endDate);

            //send the SOAP envelope to the service as a xml document
            var doc = new XmlDocument();
            doc.Load(new StringReader(soapEnvelope));

            var req = (HttpWebRequest)WebRequest.Create(url);

            //this is the valid SoapAction header for GetValues web method
            var soapAction = soapNamespace + "GetValuesObject";
 
            req.Headers.Add("SOAPAction", soapAction);

            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";
            var stm = req.GetRequestStream();
            doc.Save(stm);
            stm.Close();

            return req;
        }

        /// <summary>
        /// Gets the WaterOneFlow service version (1.0 or 1.1)
        /// </summary>
        /// <param name="url">the asmx url</param>
        /// <returns>the web service version</returns>
        public static double GetWaterOneFlowVersion(string url)
        {
            if (url.Contains("cuahsi_1_0"))
            {
                return 1.0;
            }
            if (url.Contains("cuahsi_1_1"))
            {
                return 1.1;
            }
            return 1.0;
        }

        /// <summary>
        /// Finds out the SOAP namespace (1.0 or 1.1) from the 
        /// WSDL URL. This function assumes that the url is in format ".../cuahsi_1_0.asmx?wsdl" or
        /// ".../cuahsi_1_1.asmx?wsdl"
        /// </summary>
        /// <param name="url">the wsdl url</param>
        /// <returns>the correctly formatted SOAP namespace</returns>
        public static string GetCuahsiSoapNamespace(string url)
        {
            var soapNamespace = String.Empty;
            if (url.Contains("cuahsi_1_0"))
            {
                soapNamespace = Properties.Resources.CUAHSI_1_0_Namespace;
            }
            else if (url.Contains("cuahsi_1_1"))
            {
                soapNamespace = Properties.Resources.CUAHSI_1_1_Namespace;
            }
            else
            {
                soapNamespace = Properties.Resources.CUAHSI_1_0_Namespace;
            }
            
            return soapNamespace;
        }
       
        /// <summary>
        /// Creates the SOAP Envelope for the GetValues request
        /// </summary>
        /// <param name="soapNamespace">the SOAP namespace</param>
        /// <param name="fullSiteCode">full site code in Network:SiteCode format</param>
        /// <param name="fullVariableCode">full variable code in Vocabulary:VariableCode format</param>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <returns>The SOAP envelope that needs to be sent with the GetValues request</returns>
        public static string CreateSoapEnvelope(string soapNamespace, string fullSiteCode, string fullVariableCode,
            DateTime startDate, DateTime endDate)
        {

            //to format the beginDate and endDate
            var startDateStr = startDate.ToString("yyyy-MM-ddTHH:mm");
            var endDateStr = endDate.ToString("yyyy-MM-ddTHH:mm");

            //create the AuthToken (currently, only empty AuthToken is supported)
            var authToken = String.Empty;

            var webMethodName = "GetValuesObject";

            var soapEnv = new StringBuilder();
            soapEnv.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            soapEnv.AppendLine(@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">");
            soapEnv.AppendLine("<soap:Body>");
            soapEnv.AppendLine(String.Format(@"<{0} xmlns=""{1}"">", webMethodName, soapNamespace));
            soapEnv.AppendLine(String.Format(@"<location>{0}</location>", fullSiteCode));
            soapEnv.AppendLine(String.Format(@"<variable>{0}</variable>", fullVariableCode));
            soapEnv.AppendLine(String.Format(@"<startDate>{0}</startDate>", startDateStr));
            soapEnv.AppendLine(String.Format(@"<endDate>{0}</endDate>", endDateStr));
            soapEnv.AppendLine(String.Format(@"<authToken>{0}</authToken>", authToken));
            soapEnv.AppendLine(String.Format(@"</{0}>", webMethodName));
            soapEnv.AppendLine("</soap:Body>");
            soapEnv.AppendLine("</soap:Envelope>");

            return soapEnv.ToString();
        }

        /// <summary>
        /// Creates the SOAP Envelope for the GetSiteInfo method
        /// </summary>
        /// <param name="soapNamespace">SOAP Namespace</param>
        /// <param name="fullSiteCode">full site code parameter</param>
        /// <returns>the SOAP Envelope as a xml string</returns>
        public static string CreateSoapEnvelopeForGetSiteInfo(string soapNamespace, string fullSiteCode)
        {
            //create the AuthToken (currently, only empty AuthToken is supported)
            var authToken = String.Empty;

            const string webMethodName = "GetSiteInfoObject";

            var soapEnv = new StringBuilder();
            soapEnv.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            soapEnv.AppendLine(@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">");
            soapEnv.AppendLine("<soap:Body>");
            soapEnv.AppendLine(String.Format(@"<{0} xmlns=""{1}"">", webMethodName, soapNamespace));
            soapEnv.AppendLine(String.Format(@"<site>{0}</site>", fullSiteCode));
            soapEnv.AppendLine(String.Format(@"<authToken>{0}</authToken>", authToken));
            soapEnv.AppendLine(String.Format(@"</{0}>", webMethodName));
            soapEnv.AppendLine("</soap:Body>");
            soapEnv.AppendLine("</soap:Envelope>");

            return soapEnv.ToString();
        }

        /// <summary>
        /// Creates a SOAP envelope for the GetSites web method
        /// </summary>
        /// <param name="soapNamespace">the SOAP namespace (1.1 or 1.0)</param>
        /// <returns>The SOAP envelope that needs to be sent with the GetSites request</returns>
        public static string CreateSoapEnvelopeForGetSites(string soapNamespace)
        {
            var webMethodName = "GetSites";
            if (soapNamespace == Properties.Resources.CUAHSI_1_1_Namespace)
            {
                webMethodName = "GetSitesObject";
            }
            
            //create the AuthToken (currently, only empty AuthToken is supported)
            var authToken = String.Empty;

            var soapEnv = new StringBuilder();
            soapEnv.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>");
            soapEnv.AppendLine(@"<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">");
            soapEnv.AppendLine("<soap:Body>");
            soapEnv.AppendLine(String.Format(@"<{0} xmlns=""{1}"">", webMethodName, soapNamespace));
            soapEnv.Append(@"<site><string /></site>");
            soapEnv.Append(String.Format(@"<authToken>{0}</authToken>", authToken));
            soapEnv.AppendLine(String.Format(@"</{0}>", webMethodName));
            soapEnv.AppendLine("</soap:Body>");
            soapEnv.AppendLine("</soap:Envelope>");

            return soapEnv.ToString();
        }
    }
}
