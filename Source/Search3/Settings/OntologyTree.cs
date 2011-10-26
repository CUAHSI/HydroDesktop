using System.Collections.Generic;

namespace Search3.Settings
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