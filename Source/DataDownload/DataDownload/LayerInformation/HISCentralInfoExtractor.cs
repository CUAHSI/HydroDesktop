using System;

namespace HydroDesktop.DataDownload.LayerInformation
{
    /// <summary>
    /// Provides methods to extract some service info
    /// </summary>
    interface IServiceInfoExtractor
    {
        /// <summary>
        /// Get service description URL by serviceUrl
        /// </summary>
        /// <param name="serviceUrl">ServiceUrl</param>
        /// <returns>ServiceDesciptionUrl</returns>
        string GetServiceDesciptionUrl(string serviceUrl);
    }


    class HISCentralInfoExtractor : IServiceInfoExtractor
    {
        private readonly System.Collections.Generic.Dictionary<string, string> _services;
        public HISCentralInfoExtractor(System.Collections.Generic.Dictionary<string, string> services)
        {
            if (services == null) throw new ArgumentNullException("services");
            
            _services = services;
        }

        public string GetServiceDesciptionUrl(string serviceUrl)
        {
            if (serviceUrl == null) return null;
            string res;
            return _services.TryGetValue(serviceUrl, out res) ? res : serviceUrl;
        }
    }
}