using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.PluginContracts;

namespace SeriesView
{
    public partial class SeriesSelector : UserControl, ISeriesSelector
    {
        #region Fields

        //Private Six Criterion Tables
        private DataTable _themeTable;
        private DataTable _siteTable;
        private DataTable _variableTable;
        private DataTable _methodTable;
        private DataTable _sourceTable;
        private DataTable _qcLevelTable;

        //private clicked series and selected seriesID
        private int _clickedSeriesID;

        private bool _checkedAllChanging; //checked all indicator
        private bool _checkBoxesVisible = true;

        private bool _needShowVariableNameWithDataType;

        private const string Column_Checked = "Checked";
        private const string Column_VariableName = "VariableName";
        private const string Column_SeriesID = "SeriesID";

        #endregion

        internal SeriesViewPlugin ParentPlugin { get; set; }

        #region Constructor

        public SeriesSelector()
        {
            InitializeComponent();

            //to assign the events
            dgvSeries.CellMouseDown += dgvSeries_CellMouseDown;
            dgvSeries.CurrentCellDirtyStateChanged += dgvSeries_CurrentCellDirtyStateChanged;
            dgvSeries.CellValueChanged += dgvSeries_CellValueChanged;
            dgvSeries.CellFormatting += dgvSeries_CellFormatting;

            //filter option events
            radAll.Click += radAll_Click;
            radSimple.Click += radSimple_Click;
            radComplex.Click += radComplex_Click;
            cbBoxCriterion.SelectedIndexChanged += cbBoxCriterion_SelectedIndexChanged;
            cbBoxContent.SelectedIndexChanged += cbBoxContent_SelectedIndexChanged;

            contextMenuStrip1.Opening += contextMenuStrip1_Opening;

            Settings.Instance.DatabaseChanged += Instance_DatabaseChanged;
            Disposed += SeriesSelector_Disposed;
        }

        #endregion

        #region Event Handlers

        void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // We shouldn't show the context menu when a series has not been loaded.
            e.Cancel = (_clickedSeriesID == 0);
        }

        private void dgvSeries_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_needShowVariableNameWithDataType &&
                dgvSeries.Columns[e.ColumnIndex].Name == Column_VariableName)
            {
                e.Value = string.Format("{0}, {1}",
                                        dgvSeries.Rows[e.RowIndex].Cells[Column_VariableName].Value,
                                        dgvSeries.Rows[e.RowIndex].Cells["DataType"].Value);
                e.FormattingApplied = true;
            }
        }

        private void SeriesSelector_Disposed(object sender, EventArgs e)
        {
            Settings.Instance.DatabaseChanged -= Instance_DatabaseChanged;
        }

        private void Instance_DatabaseChanged(object sender, EventArgs e)
        {
            RefreshSelection();
        }

        private void dgvSeries_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                _clickedSeriesID = Convert.ToInt32(dgvSeries.Rows[e.RowIndex].Cells[Column_SeriesID].Value);

                if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                {
                    dgvSeries.Rows[e.RowIndex].Selected = true;
                }
            }
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            SetEnableToButtons(false);
            SetChecked(false);
            SetEnableToButtons(true);
        }

        private void SetEnableToButtons(bool enable)
        {
            btnCheckAll.Enabled = enable;
            btnUncheckAll.Enabled = enable;
            btnRefresh.Enabled = enable;
            btnOptions.Enabled = enable;
            btnDelete.Enabled = enable;
            radAll.Enabled = enable;
            radSimple.Enabled = enable;
            radComplex.Enabled = enable;
            panelComplexFilter.Enabled = enable;
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            SetEnableToButtons(false);
            SetChecked(true);
            SetEnableToButtons(true);
        }

        private void radAll_Click(object sender, EventArgs e)
        {
            SetFilterOption(FilterTypes.All);
            MainView.RowFilter = "";
        }

        private void radComplex_Click(object sender, EventArgs e)
        {
            SetFilterOption(FilterTypes.Complex);
        }

        private void radSimple_Click(object sender, EventArgs e)
        {
            SetFilterOption(FilterTypes.Simple);
        }

        private void dgvSeries_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; // -1 - when column changed
            if (_checkedAllChanging) return;

            DataGridViewRow row = dgvSeries.Rows[e.RowIndex];
            var seriesID = Convert.ToInt32(row.Cells[Column_SeriesID].Value);
            var isChecked = Convert.ToBoolean(row.Cells[Column_Checked].Value);
            dgvSeries.Refresh();

            OnSeriesCheck(seriesID, isChecked);
        }

        private void dgvSeries_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvSeries.IsCurrentCellDirty)
            {
                dgvSeries.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void cbBoxCriterion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string criterionType = cbBoxCriterion.Text;
            switch (criterionType)
            {
                case "Data Network":
                    cbBoxContent.DataSource = _themeTable;
                    cbBoxContent.DisplayMember = "ThemeName";
                    cbBoxContent.ValueMember = "ThemeID";
                    break;

                case "Site":
                    cbBoxContent.DataSource = null;
                    cbBoxContent.DisplayMember = SiteDisplayColumn;
                    cbBoxContent.ValueMember = "SiteID";
                    cbBoxContent.DataSource = _siteTable;
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

            if (radSimple.Checked)
            {
                //simple filter
                var selectedRow = (DataRowView)cbBoxContent.SelectedItem;
                string selectedID = selectedRow[0].ToString();
                string criterionType = cbBoxCriterion.Text;
                string filter = "";

                switch (criterionType)
                {
                    case "Data Network":
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

                MainView.RowFilter = filter;
            }
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var series = RepositoryFactory.Instance.Get<IDataSeriesRepository>().GetByKey(_clickedSeriesID);
            if (series != null)
            {
                new SeriesProperties(series).Show(this);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete this series (ID: " + _clickedSeriesID + ")?",
                                "Confirm", MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
            {
                var manager = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
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

        private string _siteDisplayColumn = "SiteName";
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SiteDisplayColumn
        {
            get { return _siteDisplayColumn; }
            private set
            {
                _siteDisplayColumn = value;
                SetupDatabase();
            }
        }

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
            get { return _checkBoxesVisible; }
            set
            {
                _checkBoxesVisible = value;
                dgvSeries.Columns[Column_Checked].Visible = _checkBoxesVisible;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectedSeriesID
        {
            get
            {
                return _clickedSeriesID;
            }
            set
            {
                dgvSeries.ClearSelection();
                foreach (DataGridViewRow dr in dgvSeries.Rows)
                {
                    var rowSeriesID = Convert.ToInt32(dr.Cells[Column_SeriesID].Value);
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
                var view = MainView;
                return view != null ? view.RowFilter : String.Empty;
            }
            set
            {
                var view = MainView;
                if (view != null)
                {
                    view.RowFilter = value;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterTypes FilterType
        {
            get { return GetFilterType(FilterExpression); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return contextMenuStrip1; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public void RefreshSelection()
        {
            //refresh all the series, according to the new database.
            //before  refreshing, any check boxes are unchecked.
            SetChecked(false);
            SetupDatabase();
        }



        public event SeriesEventHandler SeriesCheck;
        public event EventHandler Refreshed;

        #endregion

        #region Methods

        private void SetChecked(bool isCheckedValue)
        {
            if (_checkedAllChanging) return; // to avoid multiple checking/un-checking

            _checkedAllChanging = true;
            try
            {
                // Get all rows to process
                var rowsToProcess = dgvSeries.Rows
                    .Cast<DataGridViewRow>()
                    .Where(row => Convert.ToBoolean(row.Cells[Column_Checked].Value) != isCheckedValue)
                    .ToList();

                // If rows to process is to many, ask user for confirmation
                if (isCheckedValue && rowsToProcess.Count > 20)
                {
                    var dialogResult = MessageBox.Show(
                        string.Format("Do you really want to check {0} series?", rowsToProcess.Count) +
                        Environment.NewLine
                        + "It may take a long time.", "Series View", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2
                        );
                    if (dialogResult != DialogResult.Yes) return;
                }

                // Process series...
                foreach (var row in rowsToProcess)
                {
                    row.Cells[Column_Checked].Value = isCheckedValue;
                    var seriesID = Convert.ToInt32(row.Cells[Column_SeriesID].Value);
                    _clickedSeriesID = seriesID;

                    dgvSeries.Refresh();
                    OnSeriesCheck(seriesID, isCheckedValue);
                    Application.DoEvents();
                }
            }
            finally
            {
                _checkedAllChanging = false;
            }
        }

        private DataView MainView
        {
            get { return dgvSeries.DataSource as DataView; }
        }

        public void SetupDatabase()
        {
            var conString = Settings.Instance.DataRepositoryConnectionString;

            //if the connection string is not set, exit
            if (String.IsNullOrEmpty(conString)) return;

            var manager = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
            var tbl = manager.GetDetailedSeriesTable();

            // Add Checked column
            var columnChecked = new DataColumn(Column_Checked, typeof(bool)) { DefaultValue = false, };
            tbl.Columns.Add(columnChecked);

            dgvSeries.DataSource = new DataView(tbl);
            //datagridview representation
            foreach (DataGridViewColumn col in dgvSeries.Columns)
            {
                if (col.Name != Column_Checked &&
                    col.Name != SiteDisplayColumn &&
                    col.Name != Column_VariableName &&
                    col.Name != Column_SeriesID)
                {
                    col.Visible = false;
                }
                else
                    col.Visible = true;
            }

            // Determine necessity to show VariableName with DataType in UI
            _needShowVariableNameWithDataType = false;
            foreach (DataRow row in tbl.Rows)
            {
                var variable = row[Column_VariableName].ToString();
                var site = row["SiteID"].ToString();
                if (tbl.Select(string.Format("VariableName = '{0}' and SiteID = '{1}'", variable, site)).Length >= 2)
                {
                    _needShowVariableNameWithDataType = true;
                    break;
                }
            }

            var column = dgvSeries.Columns[Column_Checked];
            Debug.Assert(column != null, "column != null");
            column.HeaderText = "Check";
            column.DisplayIndex = 0;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            column.ReadOnly = false;

            column = dgvSeries.Columns[Column_SeriesID];
            Debug.Assert(column != null, "column != null");
            column.DisplayIndex = 1;
            column.ReadOnly = true;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;

            column = dgvSeries.Columns[Column_VariableName];
            Debug.Assert(column != null, "column != null");
            column.DisplayIndex = 2;
            column.ReadOnly = true;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            column = dgvSeries.Columns[SiteDisplayColumn];
            Debug.Assert(column != null, "column != null");
            column.DisplayIndex = 3;
            column.ReadOnly = true;
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            //setup the filter option to "default all"
            SetFilterOption(FilterType);

            //to populate the 'Simple filter' criteria combo boxes
            AddSimpleFilterOptions();

            MainView.RowFilter = "";

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
                var listLocation = new Point();
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
                dgvSeries.Location = new Point(6, 90);
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
                dgvSeries.Location = new Point(6, 90);
                dgvSeries.Height = groupBox1.Bottom - cbBoxContent.Bottom - 10;
                radSimple.Checked = false;
                radComplex.Checked = true;
                radAll.Checked = false;
                panelComplexFilter.Visible = true;
                txtFilter.Text = FilterExpression;
            }
        }

        private void AddSimpleFilterOptions()
        {
            //Fill the cbBoxCriterion with 6 items
            cbBoxCriterion.Items.Clear();
            cbBoxCriterion.Items.Add("Please select a filter criterion");
            cbBoxCriterion.Items.Add("Data Network");
            cbBoxCriterion.Items.Add("Site");
            cbBoxCriterion.Items.Add("Variable");
            cbBoxCriterion.Items.Add("Method");
            cbBoxCriterion.Items.Add("Source");
            cbBoxCriterion.Items.Add("QCLevel");
            cbBoxCriterion.SelectedIndex = 0;

            _themeTable = RepositoryFactory.Instance.Get<IDataThemesRepository>().AsDataTable();
            _siteTable = RepositoryFactory.Instance.Get<ISitesRepository>().AsDataTable();
            var variables = RepositoryFactory.Instance.Get<IVariablesRepository>().GetAll();
            _variableTable = new DataTable();
            _variableTable.Columns.Add("VariableID", typeof(long));
            _variableTable.Columns.Add("VariableName", typeof(string));
            foreach (var variable in variables)
            {
                var row = _variableTable.NewRow();
                row["VariableID"] = variable.Id;
                row["VariableName"] = variable.Name + " (" + variable.VariableUnit.Abbreviation + ")";
                _variableTable.Rows.Add(row);
            }
            _sourceTable = RepositoryFactory.Instance.Get<ISourcesRepository>().AsDataTable();
            _methodTable = RepositoryFactory.Instance.Get<IMethodsRepository>().AsDataTable();
            _qcLevelTable = RepositoryFactory.Instance.Get<IQualityControlLevelsRepository>().AsDataTable();

            AddFilterOptionRow(_themeTable);
            AddFilterOptionRow(_siteTable);
            AddFilterOptionRow(_variableTable);
            AddFilterOptionRow(_sourceTable);
            AddFilterOptionRow(_methodTable);
            AddFilterOptionRow(_qcLevelTable);
        }

        //adds the 'please select filter option' item to the ComboBox
        private static void AddFilterOptionRow(DataTable table)
        {
            const string filterText = "Please select filter option";

            var row = table.NewRow();
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
            string[] parts = filterExpression.Split(new[] { '=' });
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
            var handler = SeriesCheck;
            if (handler != null)
            {
                handler(this, new SeriesEventArgs(seriesID, checkState));
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
                var isChecked = Convert.ToBoolean(dr.Cells[Column_Checked].Value);
                if (isChecked)
                {
                    seriesIDs.Add(Convert.ToInt32(dr.Cells[Column_SeriesID].Value));
                }
            }
            return seriesIDs.ToArray();
        }

        private int[] GetVisibleIDs()
        {
            var list = new List<int>();
            foreach (DataGridViewRow dr in dgvSeries.Rows)
            {
                if (dr.Visible)
                {
                    list.Add(Convert.ToInt32(dr.Cells[Column_SeriesID].Value));
                }
            }
            return list.ToArray();
        }

        #endregion

        #region Data Export

        /// <summary>
        /// Export the Selected Series to *.txt File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get the checked IDs
            var checkedIDs = new int[CheckedIDList.Length];
            if (checkedIDs.Length > 0)
            {
                Array.Copy(CheckedIDList, checkedIDs, checkedIDs.Length);
            }

            if (checkedIDs.Length == 0)
            {
                //If no series are checked, export the clicked series only.
                _clickedSeriesID = Convert.ToInt32(dgvSeries.SelectedRows[0].Cells[Column_SeriesID].Value);
                if (_clickedSeriesID > 0)
                {
                    checkedIDs = new[] { _clickedSeriesID };
                }
                else
                {
                    MessageBox.Show("Please select at least one series");
                    return;
                }
            }

            var repo = RepositoryFactory.Instance.Get<IDataValuesRepository>();
            DataTable table = null;
            foreach (var t in checkedIDs)
            {
                var exportTable = repo.GetTableForExport(t);
                if (table == null)
                {
                    table = exportTable;
                }
                else
                {
                    foreach (DataRow row in exportTable.Rows)
                    {
                        table.ImportRow(row);
                    }
                }
            }

            var exportPlugin = ParentPlugin == null? null : ParentPlugin.App.Extensions.OfType<IDataExportPlugin>().FirstOrDefault();
            if (exportPlugin != null)
            {
                exportPlugin.Export(table);
            }
        }

        #endregion

        private void btnEditFilter_Click(object sender, EventArgs e)
        {
            var frm = new frmComplexSelection(MainView.Table);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                txtFilter.Text = frm.FilterExpression;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            SetEnableToButtons(false);
            RefreshSelection();
            SetEnableToButtons(true);
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            SetEnableToButtons(false);
            var ds = new DisplaySettings
                         {
                             SiteDisplayColumn = SiteDisplayColumn,
                         };

            if (DisplayOptionsForm.ShowDialog(ds) == DialogResult.OK)
            {
                SiteDisplayColumn = ds.SiteDisplayColumn;
            }
            SetEnableToButtons(true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            SetEnableToButtons(false);
            if (MessageBox.Show("Delete all of the checked data sets?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
            {
              int[] CheckedRows = GetCheckedIDs(); // Gets all the ids of rows that are checked

                foreach (var id in CheckedRows ) // Deletes all those rows with thoses ids
                {
                    var manager = RepositoryFactory.Instance.Get<IDataSeriesRepository>();
                    manager.DeleteSeries(id);
                    RefreshSelection();
                }
            }
            SetEnableToButtons(true);
            
        }

    }
}

