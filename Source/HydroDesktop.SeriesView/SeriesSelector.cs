using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.ComponentModel;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using System.Drawing;
using HydroDesktop.Interfaces;

namespace SeriesView
{
    public partial class SeriesSelector : UserControl, ISeriesSelector
    {
        #region Private Variables

        private DataTable _table;
        private DataView _dataView;

        //Private Six Criterion Tables
        private DataTable _themeTable;
        private DataTable _siteTable;
        private DataTable _variableTable;
        private DataTable _methodTable;
        private DataTable _sourceTable;
        private DataTable _qcLevelTable;

        //private clicked series and selected seriesID
        private int _clickedSeriesID = 0;
        
        //private uncheck all indicator
        private bool _uncheckAll = false;
        //private checkboxes visible indicator
        private bool _checkBoxesVisible = true;

        #endregion

        #region Constructor
        public SeriesSelector()
        {
            InitializeComponent();

            //to assign the events
            dgvSeries.CellMouseUp += dgvSeries_CellMouseUp;
            dgvSeries.CellMouseDown += dgvSeries_CellMouseDown;
            dgvSeries.CurrentCellDirtyStateChanged += dgvSeries_CurrentCellDirtyStateChanged;
            dgvSeries.CellValueChanged += dgvSeries_CellValueChanged;
            btnUncheckAll.Click += btnUncheckAll_Click;

            //filter option events
            radAll.Click += radAll_Click;
            radSimple.Click += radSimple_Click;
            radComplex.Click += radComplex_Click;
            cbBoxCriterion.SelectedIndexChanged +=cbBoxCriterion_SelectedIndexChanged;
            cbBoxContent.SelectedIndexChanged +=cbBoxContent_SelectedIndexChanged;

            Settings.Instance.DatabaseChanged += Instance_DatabaseChanged;
            Disposed += SeriesSelector_Disposed;
        }
       
        #endregion 

        #region Event Handlers

        void SeriesSelector_Disposed(object sender, EventArgs e)
        {
            Settings.Instance.DatabaseChanged -= Instance_DatabaseChanged;
        }

        void Instance_DatabaseChanged(object sender, EventArgs e)
        {
            RefreshSelection();
        }

        void dgvSeries_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _clickedSeriesID = Convert.ToInt32(dgvSeries.Rows[e.RowIndex].Cells["SeriesID"].Value);

                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {                  
                    dgvSeries.Rows[e.RowIndex].Selected = true;
                }
            }       
        }

        void btnUncheckAll_Click(object sender, EventArgs e)
        {
            UncheckAll();
        }

        void radAll_Click(object sender, EventArgs e)
        {
            SetFilterOption(FilterTypes.All);
            _dataView.RowFilter = "";
        }

        void radComplex_Click(object sender, EventArgs e)
        {
            SetFilterOption(FilterTypes.Complex);
        }

        void radSimple_Click(object sender, EventArgs e)
        {
            SetFilterOption(FilterTypes.Simple);
        }

        void dgvSeries_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_uncheckAll) return;
            
            DataGridViewRow row = dgvSeries.Rows[e.RowIndex];
            int seriesID = Convert.ToInt32(row.Cells["SeriesID"].Value);
            bool isChecked = Convert.ToBoolean(row.Cells["Checked"].Value);         
            OnSeriesCheck(seriesID, isChecked);
        }

        void dgvSeries_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvSeries.IsCurrentCellDirty)
            {
                dgvSeries.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        void dgvSeries_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //contextMenuStrip1.Show();
            }
            OnSelectedSeriesChanged();
        }

        private void cbBoxCriterion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string criterionType = cbBoxCriterion.Text;
            switch (criterionType)
            {
                case "Themes":
                    cbBoxContent.DataSource = _themeTable;
                    cbBoxContent.DisplayMember = "ThemeName";
                    cbBoxContent.ValueMember = "ThemeID";
                    break;

                case "Site":
                    cbBoxContent.DataSource = _siteTable;
                    cbBoxContent.DisplayMember = "SiteName";
                    cbBoxContent.ValueMember = "SiteID";
                    break;

                case "Variable":
                    cbBoxContent.DataSource = _variableTable;
                    cbBoxContent.DisplayMember = "VariableName";
                    cbBoxContent.ValueMember = "VariableID";
                    break;

                case "Method":
                    cbBoxContent.DataSource = _methodTable;
                    cbBoxContent.DisplayMember = "MethodDescription";
                    cbBoxContent.ValueMember = "MethodID";
                    break;

                case "Source":
                    cbBoxContent.DataSource = _sourceTable;
                    cbBoxContent.DisplayMember = "Organization";
                    cbBoxContent.ValueMember = "SourceID";
                    break;

                case "QCLevel":
                    cbBoxContent.DataSource = _qcLevelTable;
                    cbBoxContent.DisplayMember = "Definition";
                    cbBoxContent.ValueMember = "QualityControlLevelID";
                    break;

                default:
                    Console.WriteLine("Default case");
                    break;
            }
            dgvSeries.ClearSelection();
        }

        private void cbBoxContent_SelectedIndexChanged(object sender, EventArgs e)
        {
            //when the user didn't select the criterion
            if (cbBoxContent.SelectedIndex <= 0) return;

            if (radSimple.Checked == true)
            {
                //simple filter
                DataRowView selectedRow = cbBoxContent.SelectedItem as DataRowView;
                string selectedID = selectedRow[0].ToString();
                string criterionType = cbBoxCriterion.Text;
                string filter = "";

                switch (criterionType)
                {
                    case "Themes":
                        filter = "ThemeID=" + selectedID;
                        break;
                    case "Site":
                        filter = "SiteID=" + selectedID;
                        break;
                    case "Variable":
                        filter = "VariableID=" + selectedID;
                        break;
                    case "Method":
                        filter = "MethodID=" + selectedID;
                        break;
                    case "Source":
                        filter = "SourceID=" + selectedID;
                        break;
                    case "QCLevel":
                        filter = "QualityControlLevelID=" + selectedID;
                        break;
                }

                _dataView.RowFilter = filter;
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow[] tableRows = _table.Select("SeriesID=" + _clickedSeriesID.ToString());
            if (tableRows.Length > 0)
            {
                frmProperty f = new frmProperty(tableRows[0]);
                f.Show();
            }
        }


        void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this series (ID: "+_clickedSeriesID+")?", 
                "Confirm", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
            {
                RepositoryManagerSQL manager = new RepositoryManagerSQL(DatabaseTypes.SQLite, Settings.Instance.DataRepositoryConnectionString);
                manager.DeleteSeries(_clickedSeriesID);
                RefreshSelection();
            }   
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            string currentFilter = FilterExpression;

            if (String.IsNullOrEmpty(txtFilter.Text.Trim()))
            {
                MessageBox.Show("Please enter a valid filter expression.");
                return;
            }

            try
            {
                FilterExpression = txtFilter.Text;
            }
            catch
            {
                FilterExpression = currentFilter;
                MessageBox.Show("Unable to apply filter. Please change the filter expression.");
            }
        }

        #endregion

        #region ISeriesSelector Members

        /// <summary>
        /// Get the array of all checked series IDs
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] CheckedIDList
        {
            get { return GetCheckedIDs(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int[] VisibleIDList
        {
            get { return GetVisibleIDs(); }
        }

        /// <summary>
        /// Get the context menu that appears on right-click of a series
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CheckBoxesVisible
        {
            get
            {
                return _checkBoxesVisible;
            }
            set
            {
                _checkBoxesVisible = value;
                dgvSeries.Columns["Checked"].Visible = _checkBoxesVisible;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedSeriesID
        {
            get
            {
                return _clickedSeriesID;
                
                //if (dgvSeries.SelectedRows.Count > 0)
                //{   
                //    int seriesID = Convert.ToInt32(dgvSeries.SelectedRows[0].Cells["SeriesID"].Value);
                //    return seriesID;
                //}
                //else
                //{
                //    return _clickedSeriesID;
                //}
            }
            set
            {
                dgvSeries.ClearSelection();
                foreach (DataGridViewRow dr in dgvSeries.Rows)
                {
                    int rowSeriesID = Convert.ToInt32(dr.Cells["SeriesID"].Value);
                    if (rowSeriesID == value)
                    {
                        dr.Selected = true;
                        _clickedSeriesID = rowSeriesID;
                        break;
                    }          
                }
                //if no match found, don't select any rows
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FilterExpression
        {
            get
            {
                if (_dataView != null)
                {
                    return _dataView.RowFilter;
                }
                else
                {
                    return String.Empty;
                }
            }
            set
            {
                if (_dataView != null)
                {
                    _dataView.RowFilter = value;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterTypes FilterType
        {
            get { return GetFilterType(FilterExpression); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Windows.Forms.ContextMenuStrip ContextMenuStrip
        {
            get { return contextMenuStrip1; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public void RefreshSelection()
        {
            //refresh all the series, according to the new database.
            //before  refreshing, any check boxes are unchecked.
            UncheckAll();
            SetupDatabase();
        }

        public event SeriesEventHandler SeriesCheck;

        public event EventHandler SeriesSelected;

        public event EventHandler Refreshed;

        #endregion

        #region Methods

        /// <summary>
        /// Unchecks all series check boxes
        /// </summary>
        public void UncheckAll()
        {
            _uncheckAll = true;
            
            foreach (DataGridViewRow row in dgvSeries.Rows)
            {
                bool isChecked = Convert.ToBoolean(row.Cells["Checked"].Value);
                if (isChecked)
                {
                    row.Cells["Checked"].Value = false;
                    int seriesID = Convert.ToInt32(row.Cells["SeriesID"].Value);
                    _clickedSeriesID = seriesID;
                    OnSeriesCheck(seriesID, false);
                }
            }

            _uncheckAll = false;

            //foreach (DataGridViewRow row in dgvSeries.Rows)
            //{
            //    bool isChecked = Convert.ToBoolean(row.Cells["Checked"].Value);
            //    if (isChecked)
            //    {
            //        row.Cells["Checked"].Value = false;
            //    }
            //}
        }

        public void SetupDatabase()
        {
            //Settings.Instance.Load();
            string conString = Settings.Instance.DataRepositoryConnectionString;
            
            //if the connection string is not set, exit
            if (String.IsNullOrEmpty(conString)) return;
            
            RepositoryManagerSQL manager = new RepositoryManagerSQL(DatabaseTypes.SQLite, conString);
            DataTable tbl = manager.GetSeriesTable2();
            tbl.Columns.Add("Checked", typeof(bool));
            //set value of 'checked' initially to 'false'
            foreach (DataRow row in tbl.Rows)
            {
                row["Checked"] = false;
            }


            _dataView = new DataView(tbl);
            _table = tbl;
            dgvSeries.DataSource = _dataView;
            //datagridview representation
            foreach (DataGridViewColumn col in dgvSeries.Columns)
            {
                if (col.Name != "Checked" && col.Name != "SiteName" && col.Name != "VariableName" && col.Name != "SeriesID")
                {
                    col.Visible = false;
                }
            }
            dgvSeries.Columns["Checked"].DisplayIndex = 0;
            dgvSeries.Columns["Checked"].Width = 25;
            dgvSeries.Columns["Checked"].ReadOnly = false;

            dgvSeries.Columns["VariableName"].DisplayIndex = 2;
            dgvSeries.Columns["VariableName"].ReadOnly = true;

            dgvSeries.Columns["SiteName"].DisplayIndex = 3;
            dgvSeries.Columns["SiteName"].ReadOnly = true;
            dgvSeries.Columns["SiteName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvSeries.Columns["SeriesID"].DisplayIndex = 1;
            dgvSeries.Columns["SeriesID"].Width = 35;
            dgvSeries.Columns["SeriesID"].ReadOnly = true;

            //setup the filter option to "default all"
            SetFilterOption(FilterType);

            //to populate the 'Simple filter' criteria combo boxes
            AddSimpleFilterOptions();

            _dataView.RowFilter = "";

            OnSelectionRefreshed();
        }


        /// <summary>
        /// Resets the filter options
        /// </summary>
        private void SetFilterOption(FilterTypes newFilterType)
        {
            if (newFilterType == FilterTypes.All)
            {
                //show all
                Point listLocation = new Point();
                listLocation.X = 6;
                listLocation.Y = cbBoxCriterion.Top;
                dgvSeries.Location = listLocation; //new Point(6, 34);
                dgvSeries.Height = groupBox1.Height - 25;
                //dgvSeries.HorizontalScrollbar = true;
                radSimple.Checked = false;
                radComplex.Checked = false;
                radAll.Checked = true;
                panelComplexFilter.Visible = false;
            }
            else if (newFilterType == FilterTypes.Simple)
            {
                dgvSeries.Location = new System.Drawing.Point(6, 90); ;
                dgvSeries.Height = groupBox1.Bottom - cbBoxContent.Bottom - 10;
                radSimple.Checked = true;
                radComplex.Checked = false;
                radAll.Checked = false;
                panelComplexFilter.Visible = false;

                //re-set the filter options
                if (newFilterType != FilterType)
                {
                    cbBoxCriterion.SelectedIndex = 0;
                    cbBoxContent.SelectedIndex = -1;
                }
            }
            else if (newFilterType == FilterTypes.Complex)
            {
                dgvSeries.Location = new System.Drawing.Point(6, 90); ;
                dgvSeries.Height = groupBox1.Bottom - cbBoxContent.Bottom - 10;
                radSimple.Checked = false;
                radComplex.Checked = true;
                radAll.Checked = false;
                panelComplexFilter.Visible = true;
                txtFilter.Text = this.FilterExpression;
            }
        }

        private void AddSimpleFilterOptions()
        {
            //Fill the cbBoxCriterion with 6 items
            cbBoxCriterion.Items.Clear();
            cbBoxCriterion.Items.Add("Please select a filter criterion");
            cbBoxCriterion.Items.Add("Themes");
            cbBoxCriterion.Items.Add("Site");
            cbBoxCriterion.Items.Add("Variable");
            cbBoxCriterion.Items.Add("Method");
            cbBoxCriterion.Items.Add("Source");
            cbBoxCriterion.Items.Add("QCLevel");
            cbBoxCriterion.SelectedIndex = 0;

            string conString = Settings.Instance.DataRepositoryConnectionString;
            DbOperations db = new DbOperations(conString, DatabaseTypes.SQLite);

            string sqlTheme = "SELECT ThemeID, ThemeName FROM DataThemeDescriptions";
            string sqlSite = "SELECT SiteID, SiteName FROM Sites";
            string sqlVariable = "SELECT VariableID, VariableName, UnitsAbbreviation " +
                "FROM Variables INNER JOIN Units ON Variables.VariableUnitsID = Units.UnitsID";
            string sqlMethod = "SELECT MethodID, MethodDescription FROM Methods";
            string sqlSource = "SELECT SourceID, Organization FROM Sources";
            string sqlQcLevel = "SELECT QualityControlLevelID, Definition FROM QualityControlLevels";

            _themeTable = db.LoadTable(sqlTheme);
            _siteTable = db.LoadTable(sqlSite);
            _variableTable = db.LoadTable(sqlVariable);
            _sourceTable = db.LoadTable(sqlSource);
            _methodTable = db.LoadTable(sqlMethod);
            _qcLevelTable = db.LoadTable(sqlQcLevel);

            //set variable unit names
            foreach (DataRow row in _variableTable.Rows)
            {
                row["VariableName"] = row["VariableName"] + " (" + row["UnitsAbbreviation"] + ")";
            }

            AddFilterOptionRow(_themeTable);
            AddFilterOptionRow(_siteTable);
            AddFilterOptionRow(_variableTable);
            AddFilterOptionRow(_sourceTable);
            AddFilterOptionRow(_methodTable);
            AddFilterOptionRow(_qcLevelTable);
        }

        //adds the 'please select filter option' item to the ComboBox
        private void AddFilterOptionRow(DataTable table)
        {
            string filterText = "Please select filter option";

            DataRow row = table.NewRow();
            row[0] = 0;
            row[1] = filterText;
            table.Rows.InsertAt(row, 0);
        }

        //gets the filter type (all, simple, complex) based on the filter
        //expression
        private FilterTypes GetFilterType(string filterExpression)
        {
            //empty filter expression means 'filter all'.
            if (String.IsNullOrEmpty(filterExpression)) return FilterTypes.All;

            //other filter types --> simple uses the ID and has only one '='.
            string[] parts = filterExpression.Split(new char[] { '=' });
            int numParts = parts.Length;
            if (numParts == 2)
            {
                string firstPart = parts[0].Trim();
                if (firstPart == "ThemeID" || firstPart == "SiteID" || firstPart == "VariableID" ||
                    firstPart == "SourceID" || firstPart == "MethodID" || firstPart == "QualityControlLevelID")
                {
                    return FilterTypes.Simple;
                }
            }

            //otherwise: the filter is 'Complex Filter'
            return FilterTypes.Complex;
        }

        private void OnSeriesCheck(int seriesID, bool checkState)
        {
            if (SeriesCheck != null)
            {
                SeriesCheck(this, new SeriesEventArgs(seriesID, checkState));
            }
        }

        private void OnSelectedSeriesChanged()
        {
            if (SeriesSelected != null)
            {
                SeriesSelected(this, null);
            }
        }

        private void OnSelectionRefreshed()
        {
            if (Refreshed != null)
            {
                Refreshed(this, null);
            }
        }

        private int[] GetCheckedIDs()
        {
            var seriesIDs = new List<int>();
            foreach (DataGridViewRow dr in dgvSeries.Rows)
            {
                var isChecked = Convert.ToBoolean(dr.Cells["Checked"].Value);
                if (isChecked)
                {
                    int seriesID = Convert.ToInt32(dr.Cells["SeriesID"].Value);
                    seriesIDs.Add(seriesID);
                }
            }
            return seriesIDs.ToArray();
        }

        private int[] GetVisibleIDs()
        {
            var seriesIDs = new List<int>();
            foreach (DataGridViewRow dr in dgvSeries.Rows)
            {
                if (dr.Visible)
                {
                    int seriesID = Convert.ToInt32(dr.Cells["SeriesID"].Value);
                    seriesIDs.Add(seriesID);
                }
            }
            return seriesIDs.ToArray();
        }

        #endregion

        #region Data Export

        private void bgwTable2Txt_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            e.Result = RunDataExport((DataTable)e.Argument, worker, e);
        }

        private void bgwTable2Txt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Export Failed" + e.Error.Message, "Hint",
                MessageBoxButtons.OK, MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button2);
            }
        }

        /// <summary>
        /// Export the Selected Series to *.txt File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (bgwTable2Txt.IsBusy)
            {
                MessageBox.Show("The background worker is busy now, please try later.");
                return;
            }

            //All the Series Table
            DataTable allSeriesList = _table;


            //Build select strings
            StringBuilder SQLString = new StringBuilder();

            //Get the checked IDs
            int[] checkedIDs = new int[this.CheckedIDList.Length];
            if (checkedIDs.Length > 0)
            {
                Array.Copy(this.CheckedIDList, checkedIDs, checkedIDs.Length);
            }

            if (checkedIDs.Length == 0)
            {
                //If no series are checked, export the clicked series only.
                _clickedSeriesID = Convert.ToInt32(dgvSeries.SelectedRows[0].Cells["SeriesID"].Value);
                if (_clickedSeriesID > 0)
                {
                    checkedIDs = new int[] { _clickedSeriesID };
                }
                else
                {
                    MessageBox.Show("Please select at least one series");
                    return;
                }
            }

            DbOperations dboperation;
            dboperation = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);

            //Export DataValues of the selected series instead of just series list!!
            string sql = "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.LocalDateTime, dv.DataValue, U1.UnitsName As VarUnits, v.DataType, s.SiteID, s.SiteCode, v.VariableID, v.VariableCode, " +
                      "S.Organization, S.SourceDescription, S.SourceLink, v.ValueType, v.TimeSupport, U2.UnitsName As TimeUnits, v.IsRegular, v.NoDataValue, " +
                      "dv.UTCOffset, dv.DateTimeUTC, s.Latitude, s.Longitude, dv.ValueAccuracy, m.MethodDescription, q.QualityControlLevelCode, v.SampleMedium, v.GeneralCategory " +
                "FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U1, Units U2, Methods m, QualityControlLevels q, Sources S " +
                "WHERE v.VariableID = ds.VariableID " +
                "AND s.SiteID = ds.SiteID " +
                "AND m.MethodID = ds.MethodID " +
                "AND q.QualityControlLevelID = ds.QualityControlLevelID " +
                "AND S.SourceID = ds.SourceID " +
                "AND dv.SeriesID = ds.SeriesID " +
                "AND U1.UnitsID = v.VariableUnitsID " +
                "AND U2.UnitsID = v.TimeUnitsID " +
                "AND ds.SeriesID = ";
            //initial table to get the schema
            DataTable table = dboperation.LoadTable(sql + "-1");

            for (int i = 0; i < checkedIDs.Length; i++)
            {
                String list;

                list = sql + checkedIDs[i].ToString();

                DataTable exportTable = dboperation.LoadTable("seriesTable", list);

                foreach (DataRow row in exportTable.Rows)
                {
                    table.ImportRow(row);
                }
            }

            HydroDesktop.ImportExport.ExportDataTableToTextFileDialog exportForm = new HydroDesktop.ImportExport.ExportDataTableToTextFileDialog(table);
            exportForm.ShowDialog();

            //bgwTable2Txt.RunWorkerAsync(allSeriesList);

        }

        private string RunDataExport(DataTable SeriesList, BackgroundWorker exportdlg_worker, DoWorkEventArgs e)
        {
            if (CheckedIDList.Length == 0)
            {
                MessageBox.Show("No series are checked. Please check the series to export.");
                return "series are exported.";
            }

            ///<summary>
            /// Complete Data Export codes here if "GetExportOptionsDialog" is used.
            ///</summary>
            return "series are exported.";
        }
        #endregion

        private void btnEditFilter_Click(object sender, EventArgs e)
        {
            frmComplexSelection frm = new frmComplexSelection(_table);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                txtFilter.Text = frm.FilterExpression;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshSelection();
        }        
    }
}
