using System;
using System.Windows.Forms;

namespace HydroDesktop.Search.LayerInformation
{
    class HISCentralServiceInfoExtractor : IServiceInfoExtractor
    {
        private readonly TreeNodeCollection _nodesToSearch;

        public HISCentralServiceInfoExtractor(TreeNodeCollection nodesToSearch)
        {
            if (nodesToSearch == null) throw new ArgumentNullException("nodesToSearch");
            _nodesToSearch = nodesToSearch;
        }

        public string GetServiceDesciptionUrlByServiceUrl(string serviceUrl)
        {
            var getValue = (Func<TreeNode, string>) (n =>
                                                         {
                                                             var valueNode = n.Nodes["Value"];
                                                             return valueNode == null ? null : valueNode.Text;
                                                         });

            foreach(TreeNode node in _nodesToSearch)
            {
                var servUrlNode = node.Nodes["servURL"];
                if (servUrlNode == null) continue;
                if (getValue(servUrlNode) != serviceUrl) continue;

                var serviceDescriptionNode = node.Nodes["ServiceDescriptionURL"];
                return serviceDescriptionNode == null ? null : getValue(serviceDescriptionNode);
            }

            return null;
        }
    }
}