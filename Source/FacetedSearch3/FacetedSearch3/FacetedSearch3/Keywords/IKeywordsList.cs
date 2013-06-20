using System.Collections.Generic;
using FacetedSearch3.Settings;

namespace FacetedSearch3.Keywords
{
    interface IKeywordsList
    {
        void GetKeywordsAndOntology(out IList<string> keywords, out OntologyTree ontoloyTree);
    }
}