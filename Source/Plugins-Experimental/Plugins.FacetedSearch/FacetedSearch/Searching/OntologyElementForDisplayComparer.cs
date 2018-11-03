using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacetedSearch3
{
    public class OntologyElementForDisplayComparer : IEqualityComparer<FacetedSearch3.CUAHSIFacetedSearch.OntologyElement>
    {
        public bool Equals(FacetedSearch3.CUAHSIFacetedSearch.OntologyElement x, FacetedSearch3.CUAHSIFacetedSearch.OntologyElement y)
        {
            // return x.cVocabularyID == y.cVocabularyID;
            return x.cConceptID == y.cConceptID;
        }

        public int GetHashCode(FacetedSearch3.CUAHSIFacetedSearch.OntologyElement obj)
        {
            // return obj.cVocabularyID.GetHashCode();
            return obj.cConceptID.GetHashCode();
        }
    }
}
