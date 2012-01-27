using System;
using Search3.Settings;

namespace Search3.Keywords
{
    class KeywordsList
    {
        public KeywordListData GetKeywordsListData(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");

            IKeywordsList concreteList;
            switch (catalogSettings.TypeOfCatalog)
            {
                case TypeOfCatalog.LocalMetadataCache:
                    concreteList = new DbKeywordsList();
                    break;
                case TypeOfCatalog.HisCentral:
                    concreteList = new HisCentralKeywordsList();
                    break;
                default:
                    throw new Exception("Unknown TypeOfCatalog");
            }

            return concreteList.GetKeywordsListData();
        }
    }
}
