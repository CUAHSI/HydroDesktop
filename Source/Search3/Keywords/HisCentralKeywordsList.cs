using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Search3.Settings;
using System;

namespace Search3.Keywords
{
    class HisCentralKeywordsList : IKeywordsList
    {
        private SortedSet<string> keywordsList;

        public KeywordListData GetKeywordsListData()
        {
            // Synonyms and keywords
            var tmpsyndoc = HdSearchOntologyHelper.ReadOntologySynonymsXmlFile();
            keywordsList = new SortedSet<string>();
            var synonyms = new ArrayOfOntologyPath();
            var root = tmpsyndoc.DocumentElement;
            foreach (XmlElement elem in root.ChildNodes)
            {
                var ontoPath = new OntologyPath();
                foreach (XmlElement child in elem.ChildNodes)
                {
                    if (child.Name == "conceptID")
                    {
                        int conceptID;
                        if (Int32.TryParse(child.InnerText, out conceptID))
                            ontoPath.ConceptID = conceptID;
                    }
                    else if (child.Name == "ConceptName")
                    {
                        ontoPath.ConceptName = child.InnerText;
                    }
                    else if (child.Name == "ConceptPath")
                    {
                        ontoPath.ConceptPath = child.InnerText;
                    }
                    else if (child.Name == "SearchableKeyword")
                    {
                        ontoPath.SearchableKeyword = child.InnerText;
                    }
                }
                synonyms.Add(ontoPath);
                keywordsList.Add(ontoPath.SearchableKeyword);
            }

            // Ontology tree
            var tree = new OntologyTree();
            var tmpxmldoc = HdSearchOntologyHelper.ReadOntologyXmlFile();
            FillTree(tmpxmldoc.DocumentElement, tree.Nodes);

            //------
            var result = new KeywordListData
                             {
                                 OntoloyTree = tree, 
                                 Keywords = keywordsList.ToList(), 
                                 Synonyms = synonyms,
                             };
            return result;
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