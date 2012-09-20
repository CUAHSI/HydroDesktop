using System.Collections.Generic;
using Search3.Settings;

namespace Search3.Keywords
{
    public class KeywordListData
    {
        private SortedSet<string> _keywords;
        /// <summary>
        /// Keywords, not null.
        /// </summary>
        public SortedSet<string> Keywords
        {
            get { return _keywords?? (_keywords = new SortedSet<string>()); }
            set { _keywords = value; }
        }

        private OntologyTree _ontoloyTree;
        /// <summary>
        /// Ontology tree, not null.
        /// </summary>
        public OntologyTree OntoloyTree
        {
            get { return _ontoloyTree?? (_ontoloyTree = new OntologyTree()); }
            set { _ontoloyTree = value; }
        }

        /// <summary>
        /// Synonyms, may be null.
        /// </summary>
        public List<OntologyPath> Synonyms { get; set; }
    }
}