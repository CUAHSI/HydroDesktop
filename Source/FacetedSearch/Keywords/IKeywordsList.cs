using System.Collections.Generic;
using FacetedSearch.Settings;

namespace FacetedSearch.Keywords
{
    interface IKeywordsList
    {
        void GetKeywordsAndOntology(out IList<string> keywords, out OntologyTree ontoloyTree);
    }
}