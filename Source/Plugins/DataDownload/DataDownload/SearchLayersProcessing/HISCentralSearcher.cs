using System;
using System.Net;
using System.IO;
using HydroDesktop.Configuration;

namespace HydroDesktop.DataDownload.SearchLayersProcessing
{
    public interface IHISCentralSearcher
    {
        void GetWebServicesXml(string xmlFileName);
    }
  

    /// <summary>
    /// Search for data series using HIS Central 
    /// </summary>
    public class HISCentralSearcher :  IHISCentralSearcher
    {
        /* I (valentine) honestly have no idea why this cals exsits. Jiri added it
               * it did not inherit from HydroDesktop.Data.Search.HISCentralSearcher
               * even though it had many of the same methods.
               * I added HydroDesktop.Data.Search.HISCentralSearcher
               * */
        #region Constructor

        public HISCentralSearcher()
        {
            _hisCentralUrl = Settings.Instance.DefaultHISCentralURL;
        }

        /// <summary>
        /// Create a new HIS Central Searcher which connects to the HIS Central web
        /// services
        /// </summary>
        /// <param name="hisCentralURL">The URL of HIS Central</param>
        public HISCentralSearcher(string hisCentralURL)
        {
            _hisCentralUrl = hisCentralURL;
        }

        #endregion

        #region Variables

        protected string _hisCentralUrl = "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";
        
        #endregion

        #region ISearcher Members

        /// <summary>
        /// Gets or sets the HIS Central URL
        /// </summary>
        public string HISCentralUrl
        {
            get
            {
                return _hisCentralUrl;
            }
            set
            {
                _hisCentralUrl = value;
            }
        }

        public void GetWebServicesXml(string xmlFileName)
        {
            HttpWebResponse response = null;
            try
            {
                var url = HISCentralUrl + "/GetWaterOneFlowServiceInfo";

                var request = (HttpWebRequest) WebRequest.Create(url);
                //Endpoint is the URL to which u are making the request.
                request.Method = "GET";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "text/xml";

                request.Timeout = 5000;

                // send the request and get the response
                response = (HttpWebResponse) request.GetResponse();

                using (var responseStream = response.GetResponseStream())
                {
                    using (var localFileStream = new FileStream(xmlFileName, FileMode.Create))
                    {
                        var buffer = new byte[255];

                        int bytesRead;
                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            localFileStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //todo: exceptions logging
            }
            finally
            {
                if (response != null) response.Close();
            }
        }

        #endregion
    }
}
