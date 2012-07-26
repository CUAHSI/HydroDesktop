using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Search3.Keywords;

namespace Search3.Settings
{
    public class KeywordsSettings
    {
        private readonly SearchSettings _parent;

        public KeywordsSettings(SearchSettings parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            _parent = parent;
        }

        /// <summary>
        /// Fires when Keywords/OntologyTree/Synonyms changed
        /// </summary>
        public event EventHandler KeywordsChanged;

        private IEnumerable<string> _selectedKeywords;
        public IEnumerable<string> SelectedKeywords
        {
            get { return _selectedKeywords ?? (_selectedKeywords = new string[]{}); }
            set
            {
                Debug.Assert(value != null);
                _selectedKeywords = value;
            }
        }

        private IList<string> _keywords;
        public IList<string> Keywords
        {
            get
            {
                if (_keywords == null)
                {
                    UpdateKeywordsAndOntology();
                }
                Debug.Assert(_keywords != null);
                return _keywords;
            }
            private set
            {
                _keywords = value;
            }
        }

        private OntologyTree _ontologyTree;
        public OntologyTree OntologyTree
        {
            get
            {
                if (_ontologyTree == null)
                {
                    UpdateKeywordsAndOntology();
                }
                Debug.Assert(_ontologyTree != null);
                return _ontologyTree;
            }
            private set { _ontologyTree = value; }
        }
    
        public ArrayOfOntologyPath Synonyms { get;private set;}
    

        public void UpdateKeywordsAndOntology(CatalogSettings catalogSettings = null)
        {
            var keywordsData = new KeywordsList().GetKeywordsListData(catalogSettings ?? _parent.CatalogSettings);

            Keywords = keywordsData.Keywords;
            OntologyTree = keywordsData.OntoloyTree;
            Synonyms = keywordsData.Synonyms;

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

            Keywords = source.Keywords;
            OntologyTree = source.OntologyTree;
            Synonyms = source.Synonyms;

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