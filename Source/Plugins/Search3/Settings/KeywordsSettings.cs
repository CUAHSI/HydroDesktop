using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Search3.Keywords;

namespace Search3.Settings
{
    public class KeywordsSettings
    {
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

        private IEnumerable<string> _keywords;
        public IEnumerable<string> Keywords
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
            private set { _keywords = value; }
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

        public void UpdateKeywordsAndOntology()
        {
            IEnumerable<string> keywords;
            OntologyTree ontologyTree;
            new KeywordsList().GetKeywordsAndOntology(out keywords, out ontologyTree);

            Keywords = keywords;
            OntologyTree = ontologyTree;
        }

        /// <summary>
        /// Create deep copy of current instance.
        /// </summary>
        /// <returns>Deep copy.</returns>
        public KeywordsSettings Copy()
        {
            var result = new KeywordsSettings();
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
        }
    }

    public class OntologyTree
    {
        private readonly List<OntologyNode> _nodes = new List<OntologyNode>();
        public List<OntologyNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }
    }

    public class OntologyNode
    {
        private readonly ObservableCollection<OntologyNode> _childs = new ObservableCollection<OntologyNode>();

        public OntologyNode()
            :this(null)
        {
            
        }

        public OntologyNode(string text)
        {
            Text = text;
            _childs.CollectionChanged += _childs_CollectionChanged;
        }

        void _childs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (OntologyNode item in e.NewItems)
                    item.Parent = this;
            }
        }

        public OntologyNode Parent { get; set; }
        public string Text { get; set; }

        public ObservableCollection<OntologyNode> Nodes
        {
           get { return _childs; }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}