using System.Collections.Generic;
using System.Linq;
using Search3.Searchers;
using Search3.Settings;

namespace Search3.Keywords
{
    class DbKeywordsList : IKeywordsList
    {
        public void GetKeywordsAndOntology(out IEnumerable<string> keywords, out OntologyTree ontoloyTree)
        {
            // Keywords
            var searcher = new MetadataCacheSearcher();
            var keywordsList = searcher.GetKeywords();
            keywordsList.Add("Hydrosphere");
            keywordsList.Sort();

            // Ontology tree
            var tree = new OntologyTree();
            var parentNode = new OntologyNode("Hydrosphere");
            foreach (var keyword in keywordsList.Where(keyword => keyword != "Hydrosphere"))
            {
                parentNode.Nodes.Add(new OntologyNode(keyword));
            }
            tree.Nodes.Add(parentNode);

            //------
            keywords = keywordsList;
            ontoloyTree = tree;
        }
    }
}