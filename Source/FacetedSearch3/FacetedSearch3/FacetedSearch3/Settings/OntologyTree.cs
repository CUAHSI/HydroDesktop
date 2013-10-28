using System.Collections.Generic;

namespace FacetedSearch3.Settings
{
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
}