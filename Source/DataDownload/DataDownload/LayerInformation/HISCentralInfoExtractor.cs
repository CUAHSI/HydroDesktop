using System;
using System.Collections.Generic;
using DotSpatial.Controls;
using HydroDesktop.Common.Tools;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.PluginContracts;

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
        private readonly Lazy<Dictionary<string, string>> _services;
        public HISCentralInfoExtractor(Lazy<Dictionary<string, string>> services)
        {
            if (services == null) throw new ArgumentNullException("services");
            
            _services = services;
        }

        public string GetServiceDesciptionUrl(string serviceUrl)
        {
            if (serviceUrl == null) return null;
            string res;
            return _services.Value.TryGetValue(serviceUrl, out res) ? res : serviceUrl;
        }
    }

    internal class HisCentralServices
    {
        private readonly AppManager _appManager;

        public HisCentralServices(AppManager appManager)
        {
            _appManager = appManager;
        }

        private Dictionary<string, string> _services;
        public Dictionary<string, string> Services
        {
            get
            {
                if (_services == null || _services.Count == 0)
                {
                    _services = new Dictionary<string, string>();

                    var wss = _appManager.GetExtension<ISearchPlugin>();
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