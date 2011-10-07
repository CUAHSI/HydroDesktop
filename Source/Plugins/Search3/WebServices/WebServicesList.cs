using System;
using System.Collections.Generic;
using Search3.Settings;

namespace Search3.WebServices
{
    class WebServicesList : IWebServicesList
    {
        public IEnumerable<WebServiceNode> GetWebServices()
        {
            IWebServicesList webServicesList;
            var catalogSettings = PluginSettings.Instance.CatalogSettings;
            switch(catalogSettings.TypeOfCatalog)
            {
                case TypeOfCatalog.HisCentral:
                    webServicesList = new HisCentralWebServicesList();
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
