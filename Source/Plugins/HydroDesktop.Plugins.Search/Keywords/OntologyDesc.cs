using System.Collections.Generic;
using HydroDesktop.Plugins.Search.Settings;

namespace HydroDesktop.Plugins.Search.Keywords
{
    public class OntologyDesc
    {
        #region Fields

        private IEnumerable<string> _keywords;
        private OntologyTree _ontoloyTree;

        #endregion

        /// <summary>
        /// Keywords, not null.
        /// </summary>
        public IEnumerable<string> Keywords
        {
            get { return _keywords?? (_keywords = new List<string>(0)); }
            set { _keywords = value; }
        }
        
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
        public IEnumerable<OntologyPath> Synonyms { get; set; }
    }
}