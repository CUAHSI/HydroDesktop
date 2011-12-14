using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces;
using HydroDesktop.Database;
using HydroDesktop.Configuration;

namespace TableView
{
    public partial class cTableView : UserControl
    {
        #region Fields

        private readonly ISeriesSelector _seriesSelector;

        #endregion

        #region Constructor

        public cTableView(ISeriesSelector seriesSelector)
        {
            if (seriesSelector == null) throw new ArgumentNullException("seriesSelector");

            InitializeComponent();

            _seriesSelector = seriesSelector;

            dataGridViewNavigator1.PageChanged += dataGridViewNavigator1_PageChanged;

            _seriesSelector.SeriesCheck += seriesSelector_Refreshed;
            _seriesSelector.Refreshed += seriesSelector_Refreshed;
        }

        #endregion

        #region Properties

        private TableViewMode _viewMode;
        public TableViewMode ViewMode
        {
            get { return _viewMode; }
            set
            {
                _viewMode = value;
                UpdateViewMode();
            }
        }

        /// <summary>
        /// Path to current database
        /// </summary>
        public string DatabasePath { get; private set; }

        #endregion

        #region Private methods

        private void UpdateViewMode()
        {
            if (String.IsNullOrEmpty(Settings.Instance.DataRepositoryConnectionString)) return;

            switch (ViewMode)
            {
                case TableViewMode.SequenceView:
                    ShowAllFieldsinSequence();
                    break;
                case TableViewMode.JustValuesInParallel:
                    ShowJustValuesinParallel();
                    break;
            }
        }

        private void UpdateDatabasePath()
        {
            if (Settings.Instance.DataRepositoryConnectionString != null)
            {
                DatabasePath = SQLiteHelper.GetSQLiteFileName(Settings.Instance.DataRepositoryConnectionString);
            }
        }

        private void dataGridViewNavigator1_PageChanged(object sender, PageChangedEventArgs e)
        {
            dataViewSeries.DataSource = e.DataTable;
        }

        private static DbOperations GetDbOperations()
        {
            return new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
        }

        private string GetWhereClauseForIds()
        {
            string whereClause;
            if (_seriesSelector.CheckedIDList.Length == 0)
            {
                whereClause = "1 = 0";
            }
            else
            {
                var sb = new StringBuilder("SeriesID in (");
                foreach (var id in _seriesSelector.CheckedIDList)
                    sb.AppendFormat(" {0},", id);
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                whereClause = sb.ToString();
            }
            return whereClause;
        }

        private void ShowAllFieldsinSequence()
        {
            var whereClause = GetWhereClauseForIds();
            var dbTools = GetDbOperations();
            var dataQuery =
                "SELECT ValueID, SeriesID, DataValue, LocalDateTime, UTCOffset, CensorCode FROM DataValues WHERE " +
                whereClause;
            var countQuery = "select count(*) from DataValues WHERE " + whereClause;
            dataGridViewNavigator1.Initialize(dbTools, dataQuery, countQuery);
        }

        private void ShowJustValuesinParallel()
        {
            /*
             Example of builded query:            
            
             select
                 A.LocalDateTime as DateTime, 
                 (select  DV1.DataValue from DataValues DV1 where DV1.LocalDateTime = A.LocalDateTime and DV1.seriesId = 1 limit 1) as D1,
                 (select  DV2.DataValue from DataValues DV2 where DV2.LocalDateTime = A.LocalDateTime and DV2.seriesId = 2 limit 1) as D2
             from
                 (select distinct LocalDateTime from DataValues where seriesId in (1,2)) A
             order by LocalDateTime
            
             */

            var whereClause = GetWhereClauseForIds();
            var dataQueryBuilder = new StringBuilder();
            dataQueryBuilder.Append("select A.LocalDateTime as DateTime");
            foreach (var id in _seriesSelector.CheckedIDList)
            {
                dataQueryBuilder.AppendFormat(
                    ", (select DV{0}.DataValue from DataValues DV{0} where DV{0}.LocalDateTime = A.LocalDateTime and DV{0}.seriesId = {0} limit 1) as D{0}",
                    id);
            }
            dataQueryBuilder.AppendFormat(" from (select distinct LocalDateTime  from DataValues where {0}) A",
                                          whereClause);
            dataQueryBuilder.Append(" order by LocalDateTime");

            var countQuery =
                string.Format("select count(*) from (select distinct LocalDateTime from DataValues where {0}) A",
                              whereClause);

            var dbTools = GetDbOperations();
            dataGridViewNavigator1.Initialize(dbTools, dataQueryBuilder.ToString(), countQuery);

            // Update columns headers
            var columnDateTime = dataViewSeries.Columns["DateTime"];
            Debug.Assert(columnDateTime != null);
            columnDateTime.HeaderText = "DateTime" + Environment.NewLine + "Unit";
            foreach (var id in _seriesSelector.CheckedIDList)
            {
                var sqlQuery = string.Format("SELECT UnitsName, SiteName, VariableName FROM DataSeries " +
                                             "INNER JOIN Variables ON Variables.VariableID = DataSeries.VariableID " +
                                             "INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID " +
                                             "INNER JOIN Sites ON Sites.SiteID = DataSeries.SiteID WHERE SeriesID = {0} limit 1",
                                             id);

                var seriesNameTable = dbTools.LoadTable("table", sqlQuery);
                var row1 = seriesNameTable.Rows[0];
                var unitsName = Convert.ToString(row1[0]);
                var siteName = Convert.ToString(row1[1]);
                var variableName = Convert.ToString(row1[2]);

                var columnD_id = dataViewSeries.Columns["D" + id];
                Debug.Assert(columnD_id != null);
                columnD_id.HeaderText = siteName + " * " + id + Environment.NewLine +
                                        variableName + Environment.NewLine +
                                        unitsName;
            }
        }

        private void seriesSelector_Refreshed(object sender, EventArgs e)
        {
            UpdateViewMode();
            UpdateDatabasePath();
        }

        private void cTableView_Load(object sender, EventArgs e)
        {
            dataViewSeries.ColumnHeadersVisible = true;
            dataViewSeries.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle;
           
            ViewMode = TableViewMode.SequenceView;
            UpdateDatabasePath();
        }

        /// <summary>
        /// Remove the column header border in the Aero theme in Vista,
        /// but keep it for other themes such as standard and classic.
        /// </summary>
        private static DataGridViewHeaderBorderStyle ProperColumnHeadersBorderStyle
        {
            get
            {
                return (SystemFonts.MessageBoxFont.Name == "Segoe UI")
                           ? DataGridViewHeaderBorderStyle.None
                           : DataGridViewHeaderBorderStyle.Raised;
            }
        }

        #endregion
    }

    public enum TableViewMode
    {
        [Description("Show All Fields in Sequence")]
        SequenceView,
        [Description("Show Just Values in Parallel")]
        JustValuesInParallel
    }
}
