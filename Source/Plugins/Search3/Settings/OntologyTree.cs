using System;
using System.Collections.Generic;

namespace HydroDesktop.Plugins.Search.Settings
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

        public OntologyNode FindNode(string name)
        {
            return FindNode(name, Nodes);
        }

        public static OntologyNode FindNode(string name, IEnumerable<OntologyNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (string.Equals(node.Text, name, StringComparison.OrdinalIgnoreCase))
                    return node;

                var sub = FindNode(name, node.Nodes);
                if (sub != null)
                {
                    return sub;
                }
            }

            return null;
        }
    }
}