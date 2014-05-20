using System;
using System.Collections.Generic;
using HydroDesktop.Plugins.Search.Settings;

namespace HydroDesktop.Plugins.Search.WebServices
{
    static class WebServicesReader
    {
        /// <summary>
        /// Get collection of web services for given catalogSettings.
        /// </summary>
        /// <param name="catalogSettings">Catalog settings</param>
        /// <returns>Collection of web services.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="catalogSettings"/> should be not null.</exception>
        /// <exception cref="Exception">Any exception may raised during load process.</exception>
        public static IEnumerable<WebServiceNode> GetWebServices(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");

            IWebServicesList webServicesList;
            switch (catalogSettings.TypeOfCatalog)
            {
                case TypeOfCatalog.HisCentral:
                    webServicesList = new HisCentralWebServicesList(catalogSettings.HISCentralUrl);
                    break;
                case TypeOfCatalog.LocalMetadataCache:
                    webServicesList = new DbWebServicesList();
                    break;
                default:
                    throw new Exception("Unsupported TypeOfCatalog");
            }

            return webServicesList.GetWebServices();
        }
    }
}
