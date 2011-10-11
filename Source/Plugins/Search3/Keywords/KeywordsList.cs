using System;
using System.Collections.Generic;
using Search3.Settings;

namespace Search3.Keywords
{
    class KeywordsList : IKeywordsList
    {
        public void GetKeywordsAndOntology(out SortedSet<string> keywords, out OntologyTree ontoloyTree)
        {
            var catalogSettings = PluginSettings.Instance.CatalogSettings;
            IKeywordsList concreteList;
            switch (catalogSettings.TypeOfCatalog)
            {
                case TypeOfCatalog.LocalMetadataCache:
                    concreteList = new DbKeywordsList();
                    break;
                case TypeOfCatalog.HisCentral:
                    concreteList = new HisCentalKeywordsList();
                    break;
                default:
                    throw new Exception("Unknown TypeOfCatalog");
            }

            concreteList.GetKeywordsAndOntology(out keywords, out ontoloyTree);
        }
    }
}
