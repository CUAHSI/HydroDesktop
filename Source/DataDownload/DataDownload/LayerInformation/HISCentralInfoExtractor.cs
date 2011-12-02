using System;
using System.Collections.Generic;
using System.Linq;
using HydroDesktop.Interfaces;

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
        private readonly Dictionary<string, string> _services;
        public HISCentralInfoExtractor(Dictionary<string, string> services)
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

    internal static class HisCentralServices
    {
        private static Dictionary<string, string> _services;
        public static Dictionary<string, string> Services
        {
            get
            {
                if (_services == null || _services.Count == 0)
                {
                    _services = new Dictionary<string, string>();

                    var wss = Global.PluginEntryPoint.App.Extensions.OfType<IWebServicesStore>().FirstOrDefault();
                    if (wss != null)
                    {
                        var infos = wss.GetWebServices();
                        if (infos != null)
                        {
                            foreach (var info in infos)
                            {
                                _services.Add(info.EndpointURL, info.DescriptionURL);
                            }
                        }
                    }
                }
                return _services;
            }
        }
    }
}