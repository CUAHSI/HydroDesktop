using System.Collections.ObjectModel;

namespace FacetedSearch.Settings
{
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