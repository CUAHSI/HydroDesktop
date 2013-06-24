using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Diagnostics;

namespace Search3.Settings.UI
{
    public partial class WebServicesUserControl : UserControl
    {
        #region Fields

        private WebServicesSettings _webServicesSettings;
        private CatalogSettings _catalogSettings;
      
        #endregion

        #region Constructors

        public WebServicesUserControl()
        {
            InitializeComponent();

            gridViewWebServices.CellContentClick += gridViewWebServices_OpenUrl;
            gridViewWebServices.CellValueChanged += gridViewWebServices_AfterCheck;  
        }

     

        #endregion

        #region Private methods

        void gridViewWebServices_AfterCheck(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            
            var webNode = (WebServiceNode)gridViewWebServices.Rows[e.RowIndex].Tag;
            if (webNode == null) return;

            DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)gridViewWebServices.Rows[e.RowIndex].Cells[e.ColumnIndex];
           if((bool)cell.Value == true)
           {
               webNode.Checked = true;
           }
           else 
           {
               webNode.Checked = false;
           }
          
           // webNode.Checked = gridViewWebServices.Rows[e.RowIndex].Cells[e.ColumnIndex].c

           // var webNode = (WebServiceNode)e.Node.Tag;
           // webNode.Checked = e.Node.Checked;
        }

        private void gridViewWebServices_OpenUrl(object sender, DataGridViewCellEventArgs e)
        {
             gridViewWebServices.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (gridViewWebServices.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
            {
                //do your stuff here
                string link = gridViewWebServices[e.ColumnIndex, e.RowIndex].Value.ToString();

                try
                {
                    System.Diagnostics.Process.Start(link);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Can't open url. Exception:" + ex.Message);
                }
            }
            
           // if (e.ColumnIndex != 0) return;

            
            /*
            var node = e.Node;
            var nodeInfo = node.Tag as WebServiceNode;
            if (nodeInfo == null || 
                nodeInfo.DescriptionUrl == null)
            {
                MessageBox.Show("The node [" + node.Text + "] don't have any ServiceDescriptionURL attribute.");
                return;
            }
            */  
        }

        private void RefreshWebServicesGridView(IEnumerable<WebServiceNode> webServiceNodeCollection)
        {
            gridViewWebServices.SuspendLayout();
            try
            {
                gridViewWebServices.Columns.Clear();
               
                DataGridViewCheckBoxColumn colCB = new DataGridViewCheckBoxColumn();
                colCB.Name = "chkcol";
                colCB.HeaderText = "";
                colCB.Width = 20;
                colCB.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridViewWebServices.Columns.Add(colCB);

                DataGridViewTextBoxColumn colTB = new DataGridViewTextBoxColumn();
                colTB.Name = "Service Name";
                colTB.HeaderText = "Service Name";
                gridViewWebServices.Columns.Add(colTB);

                DataGridViewTextBoxColumn colTB2 = new DataGridViewTextBoxColumn();
                colTB2.Name = "Service Code";
                colTB2.HeaderText = "Service Code";
                gridViewWebServices.Columns.Add(colTB2);

                DataGridViewTextBoxColumn colTB3 = new DataGridViewTextBoxColumn();
                colTB3.HeaderText = "Organization";
                gridViewWebServices.Columns.Add(colTB3);

                DataGridViewImageColumn dgvic = new DataGridViewImageColumn();
                dgvic.HeaderText = "More Info";
                dgvic.Image = Search3.Properties.Resources.option_16;
                gridViewWebServices.Columns.Add(dgvic);

                //row.Cells[4].Value = webNode.DescriptionUrl;

             
                gridViewWebServices.AllowUserToAddRows = true;
                foreach (var webNode in webServiceNodeCollection)
                {
                    DataGridViewRow row = (DataGridViewRow)gridViewWebServices.Rows[0].Clone();
                    row.Cells[0].Value = webNode.Checked;
                    row.Cells[1].Value = webNode.Title;
                    row.Cells[2].Value = webNode.ServiceCode;
                    row.Cells[3].Value = webNode.ServiceBoundingBox.XMax;
                    row.Tag = webNode;
                    gridViewWebServices.Rows.Add(row);
                }
                gridViewWebServices.AllowUserToAddRows = false;

              //  gridViewWebServices.Rows[0].Cells["Web Service"].Tag;

                //gridViewWebServices.Data
                /*
                var Rows = gridViewWebServices.Rows;
                var clrBule = Color.FromKnownColor(KnownColor.Blue);
                var prototype = gridViewWebServices.Font;
                var font = new Font(prototype, FontStyle.Underline);

                foreach (var webNode in webServiceNodeCollection)
                {
              
                    var node = new DataRow
                    {
                        ForeColor = clrBule,
                        NodeFont = font,
                        Text = webNode.Title,
                        Name = webNode.ServiceID.ToString(CultureInfo.InvariantCulture),
                        Checked = webNode.Checked,
                        Tag = webNode
                    };
                    parentRows.Add(node);
                }
                gridViewWebServices.Sort();*/
            }
            finally
            {
                gridViewWebServices.ResumeLayout();
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

            _webServicesSettings.RefreshWebServices(_catalogSettings);
            RefreshWebServicesGridView(_webServicesSettings.WebServices);
        }


        /// <summary>
        /// Check all web services.
        /// </summary>
        /// <param name="check">Check or uncheck all web services.</param>
        public void CheckAllWebServices(bool check)
        {
            if (gridViewWebServices.Rows.Count <= 0) return;
           
            foreach (DataGridViewRow row in gridViewWebServices.Rows)
            {
                DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)row.Cells[0];
                chk.Value = check;
            }
            
        }

        /// <summary>
        /// Set settings into control.
        /// </summary>
        /// <param name="webServicesSettings">WebServices settings to set.</param>
        /// <param name="catalogSettings">Catalog settings </param>
        /// <exception cref="ArgumentNullException">Throws if <paramref name="webServicesSettings"/> is null.</exception>
        public void SetSettings(WebServicesSettings webServicesSettings, CatalogSettings catalogSettings)
        {
            if (webServicesSettings == null) throw new ArgumentNullException("webServicesSettings");

            _catalogSettings = catalogSettings;
            _webServicesSettings = webServicesSettings;
            RefreshWebServicesGridView(webServicesSettings.WebServices);
        }

        #endregion

        private void gridViewWebServices_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 4)
            {    
                string url;
                if((url = ((WebServiceNode)gridViewWebServices.Rows[e.RowIndex].Tag).DescriptionUrl) != null)
                {
                    Process.Start(url);
                }
                  
            }
        }

    }
}
