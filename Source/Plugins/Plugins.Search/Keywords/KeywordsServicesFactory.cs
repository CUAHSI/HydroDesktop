using System;
using HydroDesktop.Plugins.Search.Settings;

namespace HydroDesktop.Plugins.Search.Keywords
{
    static class KeywordsServicesFactory
    {
        public static IOntologyReader GetKeywordsList(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");

            IOntologyReader reader;
            switch (catalogSettings.TypeOfCatalog)
            {
                case TypeOfCatalog.LocalMetadataCache:
                    reader = new DbKeywordsList();
                    break;
                case TypeOfCatalog.HisCentral:
                    reader = new HisCentralKeywordsList();
                    break;
                default:
                    throw new Exception("Unknown TypeOfCatalog");
            }

            return reader;
        }
    }
}
