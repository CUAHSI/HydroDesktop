using System.Collections.Generic;
using Search3.Settings;

namespace Search3.Keywords
{
    public class KeywordListData
    {
        private IList<string> _keywords;
        /// <summary>
        /// Keywords, not null.
        /// </summary>
        public IList<string> Keywords
        {
            get { return _keywords?? (_keywords = new List<string>()); }
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
        public ArrayOfOntologyPath Synonyms { get; set; }
    }
}