using System;
using System.Collections.Generic;
using Search3.Settings;

namespace Search3.WebServices
{
    class WebServicesList
    {
        public IEnumerable<WebServiceNode> GetWebServices(CatalogSettings catalogSettings)
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
