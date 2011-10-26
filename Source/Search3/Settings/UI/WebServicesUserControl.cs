using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class WebServicesUserControl : UserControl
    {
        #region Fields

        private WebServicesSettings _webServicesSettings;

        #endregion

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
            var webNode = (WebServiceNode)e.Node.Tag;
            webNode.Checked = e.Node.Checked;
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
            if (_webServicesSettings == null)
            {
                return;
            }
            
            _webServicesSettings.RefreshWebServices();
            RefreshWebServices(_webServicesSettings.WebServices);
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
        /// Set settings into control.
        /// </summary>
        /// <param name="webServicesSettings">WebServices settings to set.</param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="webServicesSettings"/> is null.</exception>
        public void SetSettings(WebServicesSettings webServicesSettings)
        {
            if (webServicesSettings == null) throw new ArgumentNullException("webServicesSettings");

            _webServicesSettings = webServicesSettings;
            RefreshWebServices(webServicesSettings.WebServices);
        }

        #endregion
    }
}
