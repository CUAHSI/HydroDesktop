using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacetedSearch
{
    public class OntologyElementForDisplayComparer : IEqualityComparer<FacetedSearch.CUAHSIFacetedSearch.OntologyElement>
    {
        public bool Equals(FacetedSearch.CUAHSIFacetedSearch.OntologyElement x, FacetedSearch.CUAHSIFacetedSearch.OntologyElement y)
        {
            // return x.cVocabularyID == y.cVocabularyID;
            return x.cConceptID == y.cConceptID;
        }

        public int GetHashCode(FacetedSearch.CUAHSIFacetedSearch.OntologyElement obj)
        {
            // return obj.cVocabularyID.GetHashCode();
            return obj.cConceptID.GetHashCode();
        }
    }
}
