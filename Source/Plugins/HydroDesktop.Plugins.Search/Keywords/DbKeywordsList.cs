using System.Collections.Generic;
using System.Linq;
using HydroDesktop.Plugins.Search.Searching;
using HydroDesktop.Plugins.Search.Settings;

namespace HydroDesktop.Plugins.Search.Keywords
{
    class DbKeywordsList : IOntologyReader
    {
        public OntologyDesc GetOntologyDesc()
        {
            // Keywords
            var searcher = new MetadataCacheSearcher();
            var keywordsList = searcher.GetKeywords();
            keywordsList.Add(Constants.RootName);
            var sortedKeywords = new SortedSet<string>(keywordsList);

            // Ontology tree
            var tree = new OntologyTree();
            var parentNode = new OntologyNode(Constants.RootName);
            foreach (var keyword in keywordsList.Where(keyword => keyword != Constants.RootName))
            {
                parentNode.Nodes.Add(new OntologyNode(keyword));
            }
            tree.Nodes.Add(parentNode);

            // Return result
            var result = new OntologyDesc
                             {
                                 Keywords = sortedKeywords,
                                 OntoloyTree = tree,
                             };
            return result;
        }
    }
}