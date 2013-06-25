using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Data;
using System.Linq;
using System.Diagnostics;
using Search3.Settings;
using Search3.Area;
using DotSpatial.Projections;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Controls.Header;
using DotSpatial.Topology;

namespace Search3.Settings.UI
{
    public partial class WebServicesUserControl : UserControl
    {
        #region Fields

        private WebServicesSettings _webServicesSettings;
        private CatalogSettings _catalogSettings;
        private AppManager App;
        private readonly SearchSettings _searchSettings = new SearchSettings();
        private RectangleDrawing _rectangleDrawing;

        #endregion

        #region Constructors

        public WebServicesUserControl(AppManager App)
        {
            this.App = App;
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
                colTB.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                gridViewWebServices.Columns.Add(colTB);

                DataGridViewTextBoxColumn colTB2 = new DataGridViewTextBoxColumn();
                colTB2.Name = "Service Code";
                colTB2.HeaderText = "Service Code";
                colTB2.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                gridViewWebServices.Columns.Add(colTB2);

                DataGridViewImageColumn dgvic = new DataGridViewImageColumn();
                dgvic.HeaderText = "View Extents";
                dgvic.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dgvic.ValuesAreIcons = true;
                dgvic.Icon = Search3.Properties.Resources.view_extents_16_16x16;
                gridViewWebServices.Columns.Add(dgvic);

                DataGridViewImageColumn dgvic2 = new DataGridViewImageColumn();
                dgvic2.HeaderText = "More Info";
                dgvic2.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                dgvic2.ValuesAreIcons = true;
                dgvic2.Icon = Search3.Properties.Resources.more_info;
                gridViewWebServices.Columns.Add(dgvic2);
             
               
              

                gridViewWebServices.AllowUserToAddRows = true;
                foreach (var webNode in webServiceNodeCollection)
                {
                   
                    DataGridViewRow row = (DataGridViewRow)gridViewWebServices.Rows[0].Clone();
                    row.Cells[0].Value = webNode.Checked;
                    row.Cells[1].Value = webNode.Title;
                    row.Cells[2].Value = webNode.ServiceCode;
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

        private double[] LatLonReproject(double x, double y)
        {
            double[] xy = new double[2] { x, y };

            //Change y coordinate to be less than 90 degrees to prevent a bug.
            if (xy[1] >= 90) xy[1] = 89.9;
            if (xy[1] <= -90) xy[1] = -89.9;

            //Need to convert points to proper projection. Currently describe WGS84 points which may or may not be accurate.
            bool isWgs84;

            String wgs84String = "GEOGCS[\"GCS_WGS_1984\",DATUM[\"D_WGS_1984\",SPHEROID[\"WGS_1984\",6378137,298.257223562997]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.0174532925199433]]";
            String mapProjEsriString = App.Map.Projection.ToEsriString();
            isWgs84 = (mapProjEsriString.Equals(wgs84String));

            //If the projection is not WGS84, then convert points to properly describe desired location.
            if (!isWgs84)
            {
                double[] z = new double[1];
                ProjectionInfo wgs84Projection = ProjectionInfo.FromEsriString(wgs84String);
                ProjectionInfo currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
                Reproject.ReprojectPoints(xy, z, wgs84Projection, currentMapProjection, 0, 1);
            }

            //Return array with 1 x and 1 y value.
            return xy;
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
            if (e.ColumnIndex == 3)
            {
                double[] minXY = new double[2];
                double[] maxXY = new double[2];

                //Now convert from Lat-Long to x,y coordinates that App.Map.ViewExtents can use to pan to the correct location.
                WebServiceNode node = (WebServiceNode)gridViewWebServices.Rows[e.RowIndex].Tag;
               
                minXY = LatLonReproject(node.ServiceBoundingBox.XMin, node.ServiceBoundingBox.YMin);
                maxXY = LatLonReproject(node.ServiceBoundingBox.XMax, node.ServiceBoundingBox.YMax);

                //Get extent where center is desired X,Y coordinate.
                App.Map.ViewExtents.MinX = minXY[0];
                App.Map.ViewExtents.MinY = minXY[1];
                App.Map.ViewExtents.MaxX = maxXY[0];
                App.Map.ViewExtents.MaxY = maxXY[1];

                Extent ex = App.Map.ViewExtents;

              //  _rectangleDrawing = new RectangleDrawing((Map)App.Map);
             //   _searchSettings.AreaSettings.SetAreaRectangle(ex, App.Map.Projection);
                //Set App.Map.ViewExtents to new extent that centers on desired LatLong.
                App.Map.ViewExtents = ex;
            }
            else if (e.ColumnIndex == 4)
            {
                string url;
                if ( (url = ((WebServiceNode)gridViewWebServices.Rows[e.RowIndex].Tag).DescriptionUrl) != null)
                {
                    Process.Start(url);
                }
            }
        }

  

        private void gridViewWebServices_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 3 || e.ColumnIndex == 4)
            {
                this.Cursor = Cursors.Hand;
            }
            else
            {
                this.Cursor = Cursors.Default;
            } 
        }

      
    }
}
