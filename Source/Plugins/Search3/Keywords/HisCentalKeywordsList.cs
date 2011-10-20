using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Search3.Settings;

namespace Search3.Keywords
{
    class HisCentalKeywordsList : IKeywordsList
    {
        private SortedSet<string> keywordsList;

        public void GetKeywordsAndOntology(out IList<string> keywords, out OntologyTree ontoloyTree)
        {
            // Keywords
            var tmpsyndoc = HdSearchOntologyHelper.ReadOntologySymbologyXmlFile();
            var nList = tmpsyndoc.GetElementsByTagName("SearchableKeyword");
            keywordsList = new SortedSet<string>();
            foreach (var elem in nList.Cast<XmlElement>().Where(elem => !keywordsList.Contains(elem.InnerText)))
            {
                keywordsList.Add(elem.InnerText);
            }

            // Ontology tree
            var tree = new OntologyTree();
            var tmpxmldoc = HdSearchOntologyHelper.ReadOntologyXmlFile();
            FillTree(tmpxmldoc.DocumentElement, tree.Nodes);

            //------
            ontoloyTree = tree;
            keywords = keywordsList.ToList();
        }

        private void FillTree(XmlNode node, ICollection<OntologyNode> parentnode)
        {
            // End recursion if the node is a text type
            if (node == null || node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.CDATA)
                return;

            var tmptreenodecollection = AddNodeToTree(node, parentnode);

            // Add all the children of the current node to the treeview
            foreach (XmlNode tmpchildnode in node.ChildNodes)
            {
                if (tmpchildnode.Name == "childNodes")
                {
                    foreach (XmlNode tmpchildnode2 in tmpchildnode.ChildNodes)
                    {
                        FillTree(tmpchildnode2, tmptreenodecollection);
                    }
                }
            }
        }
        private ICollection<OntologyNode> AddNodeToTree(XmlNode node, ICollection<OntologyNode> parentnode)
        {
            var newchildnode = CreateTreeNodeFromXmlNode(node);
            if (parentnode != null) parentnode.Add(newchildnode);
            return newchildnode.Nodes;
        }
        private OntologyNode CreateTreeNodeFromXmlNode(XmlNode node)
        {
            OntologyNode tmptreenode = null;
            if (node.HasChildNodes)
            {
                if (node.FirstChild.InnerText != string.Empty)
                {
                    tmptreenode = new OntologyNode(node.FirstChild.InnerText);

                    if (!keywordsList.Contains(node.FirstChild.InnerText))
                    {
                        keywordsList.Add(node.FirstChild.InnerText);
                    }
                }
            }
            return tmptreenode ?? (new OntologyNode());
        }
    }
}