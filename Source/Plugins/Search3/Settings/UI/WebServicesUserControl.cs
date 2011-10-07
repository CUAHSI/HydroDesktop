using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Search3.Settings.UI
{
    public partial class WebServicesUserControl : UserControl
    {
        #region Constructors

        public WebServicesUserControl()
        {
            InitializeComponent();

            treeViewWebServices.NodeMouseClick += treeViewWebServices_OpenUrl;
            treeViewWebServices.AfterCheck += treeViewWebServices_AfterCheck;
            
        }

        #endregion

        #region Private methods

        void treeViewWebServices_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var oldNode = (WebServiceNode)e.Node.Tag;
            e.Node.Tag = new WebServiceNode(oldNode.Title, oldNode.ServiceID, oldNode.DescriptionUrl, oldNode.ServiceUrl, e.Node.Checked);
        }

        private void treeViewWebServices_OpenUrl(Object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!e.Node.Bounds.Contains(e.Location)) return;

            var node = e.Node;
            var nodeInfo = node.Tag as WebServiceNode;
            if (nodeInfo == null || 
                nodeInfo.DescriptionUrl == null)
            {
                MessageBox.Show("The node [" + node.Text + "] don't have any ServiceDescriptionURL attribute.");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(nodeInfo.DescriptionUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't open url. Exception:" + ex.Message);
            }
        }

        private void RefreshWebServices(IEnumerable<WebServiceNode> webServiceNodeCollection)
        {
            treeViewWebServices.SuspendLayout();
            try
            {
                treeViewWebServices.Nodes.Clear();

                var parentNodes = treeViewWebServices.Nodes;
                var clrBule = Color.FromKnownColor(KnownColor.Blue);
                var prototype = treeViewWebServices.Font;
                var font = new Font(prototype, FontStyle.Underline);

                foreach (var webNode in webServiceNodeCollection)
                {
                    var node = new TreeNode
                    {
                        ForeColor = clrBule,
                        NodeFont = font,
                        Text = webNode.Title,
                        Name = webNode.ServiceID,
                        Checked = webNode.Checked,
                        Tag = webNode
                    };
                    parentNodes.Add(node);
                }
                treeViewWebServices.Sort();
            }
            finally
            {
                treeViewWebServices.ResumeLayout();
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Refresh all web services.
        /// </summary>
        public void RefreshWebServices()
        {
            RefreshWebServices(new WebServicesList().GetWebServicesCollection());
        }


        /// <summary>
        /// Check all web services.
        /// </summary>
        /// <param name="check">Check or uncheck all web services.</param>
        public void CheckAllWebServices(bool check)
        {
            if (treeViewWebServices.Nodes.Count <= 0) return;
            foreach (TreeNode tnode in treeViewWebServices.Nodes)
            {
                tnode.Checked = check;
            }
        }

        /// <summary>
        /// Get all web services as IEnumerable.
        /// </summary>
        /// <returns>Collection of all web services.</returns>
        public IEnumerable<WebServiceNode> GetWebServices()
        {
            var list = new List<WebServiceNode>(treeViewWebServices.Nodes.Count);
            list.AddRange(from TreeNode tnode in treeViewWebServices.Nodes select (WebServiceNode) tnode.Tag);
            return list;
        }
        
        /// <summary>
        /// Set web services.
        /// </summary>
        /// <param name="webServices">Web services to set.</param>
        public void SetWebServices(IEnumerable<WebServiceNode> webServices)
        {
            RefreshWebServices(webServices);
        }

        #endregion
    }
}
