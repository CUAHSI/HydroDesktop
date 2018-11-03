using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HydroDesktop.Plugins.Search.Keywords;

namespace HydroDesktop.Plugins.Search.Settings
{
    public class KeywordsSettings
    {
        #region Fields

        private readonly SearchSettings _parent;
        private OntologyDesc _ontologyDesc;
        private IEnumerable<string> _selectedKeywords;

        #endregion

        /// <summary>
        /// Fires when Keywords/OntologyTree/Synonyms changed
        /// </summary>
        public event EventHandler KeywordsChanged;

        public KeywordsSettings(SearchSettings parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            _parent = parent;
        }

        
        public IEnumerable<string> SelectedKeywords
        {
            get { return _selectedKeywords ?? (_selectedKeywords = new string[]{}); }
            set
            {
                _selectedKeywords = value;
            }
        }
        
        public IEnumerable<string> Keywords
        {
            get
            {
                if (_ontologyDesc == null)
                {
                    UpdateKeywordsAndOntology();
                }
                Debug.Assert(_ontologyDesc != null);
                return _ontologyDesc.Keywords;
            }
        }

        public OntologyTree OntologyTree
        {
            get
            {
                if (_ontologyDesc == null)
                {
                    UpdateKeywordsAndOntology();
                }
                Debug.Assert(_ontologyDesc != null);
                return _ontologyDesc.OntoloyTree;
            }
        }

        public OntologyDesc OntologyDesc
        {
            get { return _ontologyDesc; }
        }
        

        /// <summary>
        /// Returns synonym for keyword.
        /// </summary>
        /// <param name="keyword">Keyword to find synonym.</param>
        /// <returns>Synonym for keyword, or keyword, if synonym not found.</returns>
        public string FindSynonym(string keyword)
        {
            if (_ontologyDesc == null) return keyword;
            var synonyms = _ontologyDesc.Synonyms;
            if (synonyms != null)
            {
                foreach (var ontoPath in synonyms)
                {
                    if (string.Equals(ontoPath.SearchableKeyword, keyword, StringComparison.OrdinalIgnoreCase))
                    {
                        keyword = ontoPath.ConceptName;
                        break;
                    }
                }
            }

            return keyword;
        }

        public void UpdateKeywordsAndOntology(CatalogSettings catalogSettings = null)
        {
            var desc = KeywordsServicesFactory.GetKeywordsList(catalogSettings ?? _parent.CatalogSettings).GetOntologyDesc();

            // Select root of OntoloyTree
            if (_selectedKeywords == null && 
                desc.OntoloyTree.Nodes.Count > 0)
            {
                _selectedKeywords = new[] { desc.OntoloyTree.Nodes[0].Text };
            }

            _ontologyDesc = desc;
            RaiseKeywordsChanged();
        }

        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public KeywordsSettings Copy()
        {
            var result = new KeywordsSettings(_parent);
            result.Copy(this);
            return result;
        }

        /// <summary>
        /// Create deep from source into current instance.
        /// </summary>
        /// <param name="source">Source.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>must be not null.</exception>
        public void Copy(KeywordsSettings source)
        {
            if (source == null) throw new ArgumentNullException("source");

            var selectedKeywords = new List<string>(source.SelectedKeywords.Count());
            selectedKeywords.AddRange(source.SelectedKeywords.Select(s => s));
            SelectedKeywords = selectedKeywords;

            _ontologyDesc = source._ontologyDesc;
            RaiseKeywordsChanged();
        }

        #region Private methods

        private void RaiseKeywordsChanged()
        {
            var handler = KeywordsChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}