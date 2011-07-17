using System;
using System.Windows.Forms;

namespace HydroDesktop.Search.LayerInformation
{
    class HISCentralInfoExtractor : IServiceInfoExtractor
    {
        private readonly TreeNodeCollection _nodesToSearch;

        public HISCentralInfoExtractor(TreeNodeCollection nodesToSearch)
        {
            if (nodesToSearch == null) throw new ArgumentNullException("nodesToSearch");
            _nodesToSearch = nodesToSearch;
        }
/*
      // used in version with sub-nodes
        public string GetServiceDesciption(string serviceUrl)
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
        */
        public string GetServiceDesciptionUrl(string serviceUrl)
        {
            foreach (TreeNode node in _nodesToSearch)
            {
                var nodeInfo = node.Tag as SearchControl.NodeInfo;
                if (nodeInfo == null ||
                    nodeInfo.ServiceUrl != serviceUrl
                    ) continue;

                return nodeInfo.DescritionUrl;
            }

            return null;
        }
    }
}