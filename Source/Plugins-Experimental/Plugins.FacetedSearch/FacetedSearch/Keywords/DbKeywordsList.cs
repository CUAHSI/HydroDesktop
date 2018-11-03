using System.Collections.Generic;
using System.Linq;
using FacetedSearch3.Searching;
using FacetedSearch3.Settings;

namespace FacetedSearch3.Keywords
{
    class DbKeywordsList : IKeywordsList
    {
        public void GetKeywordsAndOntology(out IList<string> keywords, out OntologyTree ontoloyTree)
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
            keywords = sortedKeywords.ToList();
            ontoloyTree = tree;
        }
    }
}