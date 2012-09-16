using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Search3.Settings;

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
                    var text = child.InnerText.Trim();
                    if (child.Name == "conceptID")
                    {
                        int conceptID;
                        if (Int32.TryParse(text, out conceptID))
                            ontoPath.ConceptID = conceptID;
                    }
                    else if (child.Name == "ConceptName")
                    {
                        ontoPath.ConceptName = text;
                    }
                    else if (child.Name == "ConceptPath")
                    {
                        ontoPath.ConceptPath = text;
                    }
                    else if (child.Name == "SearchableKeyword")
                    {
                        ontoPath.SearchableKeyword = text;
                    }
                }
                synonyms.Add(ontoPath);
                if (!String.IsNullOrWhiteSpace(ontoPath.SearchableKeyword))
                {
                    keywordsList.Add(ontoPath.SearchableKeyword);
                }
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
                var text = node.FirstChild.InnerText.Trim();
                if (text != string.Empty)
                {
                    tmptreenode = new OntologyNode(text);
                    keywordsList.Add(text);
                }
            }
            return tmptreenode ?? (new OntologyNode());
        }
    }
}