using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using Search3.Settings;

namespace Search3.Keywords
{
    class HisCentralKeywordsList : IOntologyReader
    {
        private static readonly string _ontologyFilename = ShaleDataNetwork.Properties.Settings.Default.OntologyFilename;
        private static readonly string _ontologySynonymsFilename = ShaleDataNetwork.Properties.Settings.Default.SynonymsFilename;
        private SortedSet<string> keywordsList;

        public OntologyDesc GetOntologyDesc()
        {
            // Synonyms and keywords
            var tmpsyndoc = ReadOntologySynonymsXmlFile();
            keywordsList = new SortedSet<string>();
            var synonyms = new List<OntologyPath>();
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
                // Add to sysnonyms, only if SearchableKeyword != ConceptName
                if (!string.Equals(ontoPath.SearchableKeyword, ontoPath.ConceptName) &&
                    !string.IsNullOrEmpty(ontoPath.SearchableKeyword))
                {
                    synonyms.Add(ontoPath);
                }
                if (!String.IsNullOrWhiteSpace(ontoPath.SearchableKeyword))
                {
                    keywordsList.Add(ontoPath.SearchableKeyword);
                }
            }

            // Ontology tree
            var tree = new OntologyTree();
            var tmpxmldoc = ReadOntologyXmlFile();
            FillTree(tmpxmldoc.DocumentElement, tree.Nodes);

            // Replace Hydroshpere with All
            keywordsList.Remove("Hydrosphere");
            keywordsList.Add(Constants.RootName);
            if (tree.Nodes.Count > 0)
            {
                tree.Nodes[0].Text = Constants.RootName;
            }

            // Return result
            var result = new OntologyDesc
                             {
                                 OntoloyTree = tree, 
                                 Keywords = keywordsList, 
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

        private static XmlDocument ReadOntologyXmlFile()
        {
            return ReadXmlFile(_ontologyFilename);
        }

        private static XmlDocument ReadOntologySynonymsXmlFile()
        {
            return ReadXmlFile(_ontologySynonymsFilename);
        }

        private static XmlDocument ReadXmlFile(string filename)
        {
            var tmpxmldoc = new XmlDocument();
            var assemblyFolder = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Debug.Assert(assemblyFolder != null, "assemblyFolder != null");
            tmpxmldoc.Load(Path.Combine(assemblyFolder, filename));
            return tmpxmldoc;
        }

        internal static List<string> RefineKeywordList(OntologyDesc desc, List<string> keywords)
        {
            var ontologyTree = desc.OntoloyTree;
            var allKeywords = desc.Keywords;

            // If searching 1st tier keywords, clear the list.
            var tier1Keywords = ontologyTree.Nodes.Select(d => d.Text);
            if (tier1Keywords.Any(keywords.Contains))
            {
                keywords.Clear();
                return keywords;
            }

            // Remove duplicates
            keywords = keywords.Distinct().ToList();

            // Remove invalid keywords
            var toDelete = new HashSet<string>(keywords.Where(k => !allKeywords.Contains(k)));

            // Remove keywords if their ancestors are also in the list.
            foreach (var keyword in keywords)
            {
                if (toDelete.Contains(keyword)) continue;
                var node = ontologyTree.FindNode(keyword);
                if (node == null) continue;

                foreach (var other in keywords)
                {
                    if (other == keyword) continue;
                    if (toDelete.Contains(other)) continue;
                    if (node.HasChild(other))
                    {
                        toDelete.Add(other);
                    }
                }
            }
            foreach (var del in toDelete)
            {
                keywords.Remove(del);
            }

            // Replace 2nd tier keywords with their 3rd tier child keywords.
            // 2nd tier keywords cannot be searched at HIS Central.
            foreach (var tier2Node in ontologyTree.Nodes.SelectMany(node => node.Nodes))
            {
                var tier2keyword = tier2Node.Text;
                if (!keywords.Contains(tier2keyword)) continue;

                // Remove 2nd tier keyword
                keywords.Remove(tier2keyword);

                // Add 3rd tier keywords that are children of the removed 2nd tier keyword.
                var tier3Keywords = tier2Node.Nodes.Select(d => d.Text);
                foreach (var tier3keyword in tier3Keywords.Where(tier3keyword => !keywords.Contains(tier3keyword)))
                {
                    keywords.Add(tier3keyword);
                }
            }

            return keywords;
        }
    }
}