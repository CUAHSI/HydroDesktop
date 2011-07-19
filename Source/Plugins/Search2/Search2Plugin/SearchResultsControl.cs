using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Search.Download;

namespace HydroDesktop.Search
{
    public partial class SearchResultsControl : UserControl
    {
        #region Constructors

        public SearchResultsControl()
        {
            InitializeComponent();

            searchDataGridView.SelectionChanged += searchDataGridView1_SelectionChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Count of selected row in the dataGridView
        /// </summary>
        public int SelectedRowsCount
        {
            get { return searchDataGridView.SelectedRows.Count; }
        }

        #endregion

        #region Public methods

        public void SetDataSource(IMapFeatureLayer dataSource)
        {
            searchDataGridView.SetDataSource(dataSource);
        }

        public void ClearSelectionInGridView()
        {
            searchDataGridView.ClearSelection();
        }

        public IList<OneSeriesDownloadInfo> GetSelectedSeriesAsDownloadInfo(DateTime startDate, DateTime endDate)
        {
            var downloadList = new List<OneSeriesDownloadInfo>();
            var fileNameList = new List<String>(); 
            foreach (var selFeature in searchDataGridView.MapLayer.Selection.ToFeatureList())
            {
                var row = selFeature.DataRow;

                var di = new OneSeriesDownloadInfo
                {
                    SiteName = row["SiteName"].ToString(),
                    FullSiteCode = row["SiteCode"].ToString(),
                    FullVariableCode = row["VarCode"].ToString(),
                    Wsdl = row["ServiceURL"].ToString(),
                    StartDate = startDate,
                    EndDate = endDate,
                    VariableName = row["VarName"].ToString(),
                    Latitude = Convert.ToDouble(row["Latitude"]),
                    Longitude = Convert.ToDouble(row["Longitude"])
                };

                var fileBaseName = di.FullSiteCode + "|" + di.FullVariableCode;
                if (fileNameList.Contains(fileBaseName)) continue;

                fileNameList.Add(fileBaseName);
                downloadList.Add(di);
            }

            return downloadList;
        }

        #endregion

        #region Private methods

        private void searchDataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            var numSelected = searchDataGridView.SelectedRows.Count;
            lblDataSeries.Text = String.Format("{0} out of {1} series selected", numSelected, searchDataGridView.RowCount);
        }

        #endregion
    }
}
