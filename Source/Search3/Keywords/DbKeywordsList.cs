using System.Collections.Generic;
using System.Linq;
using Search3.Searching;
using Search3.Settings;

namespace Search3.Keywords
{
    class DbKeywordsList : IKeywordsList
    {
        public KeywordListData GetKeywordsListData()
        {
            // Keywords
            var searcher = new MetadataCacheSearcher();
            var keywordsList = searcher.GetKeywords();
            keywordsList.Add("Hydrosphere");
            var sortedKeywords = new SortedSet<string>(keywordsList);

            // Ontology tree
            var tree = new OntologyTree();
            var parentNode = new OntologyNode("Hydrosphere");
            foreach (var keyword in keywordsList.Where(keyword => keyword != "Hydrosphere"))
            {
                parentNode.Nodes.Add(new OntologyNode(keyword));
            }
            tree.Nodes.Add(parentNode);

            //------
            var result = new KeywordListData
                             {
                                 Keywords = sortedKeywords.ToList(),
                                 OntoloyTree = tree,
                             };
            return result;
        }
    }
}