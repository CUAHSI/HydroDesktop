using System.Collections.Generic;
using Search3.Settings;

namespace Search3.Keywords
{
    interface IKeywordsList
    {
        void GetKeywordsAndOntology(out SortedSet<string> keywords, out OntologyTree ontoloyTree);
    }
}