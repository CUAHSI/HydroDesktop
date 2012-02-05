using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using HydroDesktop.Database;
using HydroDesktop.Help;
using HydroDesktop.Interfaces;

namespace HydroDesktop.ExportToCSV
{
    /// <summary>
    /// Export Data Form with BackgroundWorker and allow users to select themes to export.
    /// </summary>
    public partial class ExportDialog : Form
    {
        #region Fields

        private readonly DbOperations _dboperation;
        private readonly DataTable _dataToExport;
        private readonly IEnumerable<string> _selectedThemes;
        private bool _formIsClosing;
        private readonly string _localHelpUri = Properties.Settings.Default.localHelpUri;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the ExportData form with themes to export
        /// </summary>
        public ExportDialog(DbOperations dbOperation, IEnumerable<string> selectedThemes = null)
        {
            if (dbOperation == null) throw new ArgumentNullException("dbOperation");
            Contract.EndContractBlock();

            _dboperation = dbOperation;
            _selectedThemes = selectedThemes;

            InitializeComponent();
        }

        /// <summary>
        /// Initialize the ExportData form with data table to export
        /// </summary>
        public ExportDialog(DbOperations dbOperation, DataTable dataToExport)
        {
            if (dbOperation == null) throw new ArgumentNullException("dbOperation");
            if (dataToExport == null) throw new ArgumentNullException("dataToExport");
            if (dataToExport.Columns.Count == 0)
            {
                throw new ArgumentException("Data table for export must have at least one column.");
            }
            if (dataToExport.Rows.Count == 0)
            {
                throw new ArgumentException("Data table for export must have at least one row.");
            }
            Contract.EndContractBlock();

            _dboperation = dbOperation;
            _dataToExport = dataToExport;

            InitializeComponent();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the export form.
        /// </summary>
        private void ExportDialog_load(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            if (ExpotThemes)
            {
                //populate list box with list of themes
                var repository = RepositoryFactory.Instance.Get<IDataThemesRepository>(_dboperation);
                var dtThemes = repository.GetThemesForAllSeries();

                clbThemes.Items.Clear();
                foreach (DataRow row in dtThemes.Rows)
                {
                    var themeName = row["ThemeName"].ToString();
                    var check = _selectedThemes == null || _selectedThemes.Contains(themeName);
                    var themeID = row["ThemeID"] == DBNull.Value ? (int?) null : Convert.ToInt32(row["ThemeID"]);
                    clbThemes.Items.Add(new ThemeDescription(themeID, themeName), check);
                }
            }
            else
            {
                gbxThemes.Visible = false;
                var themesHeight = gbxThemes.Height;
                gbxThemes.Height = 0;
                Height -= themesHeight;
                gbxFields.Location = gbxThemes.Location;
                gbxFields.Height = delimiterSelector1.Location.Y - 10 - gbxFields.Location.Y;
                tcMain.TabPages.Remove(tpAdvancedOptions);
            }

            // Populate checked list box with list of fields to export
            LoadFieldList();

            Cursor = Cursors.Default;
        }

        /// <summary>
        /// Initialize the ChecklistBox to show all the fields necessary.
        /// </summary>
        private void LoadFieldList()
        {
            //Set fields in CheckListBox
            DataTable dtList;

            if (ExpotThemes)
            {
                var repo = RepositoryFactory.Instance.Get<IDataValuesRepository>(_dboperation);
                dtList = repo.GetTableForExport(-1);
            }else
            {
                dtList = _dataToExport;
            }

            //Headers shown in CheckListBox
            for (int i = 0; i < dtList.Columns.Count; i++)
            {
                var header = dtList.Columns[i].ToString();
                var ind = clbExportItems.Items.Add(header);
                clbExportItems.SetItemChecked(ind, true);
            }

            // Fill DateTime columns
            cmbDateTimeColumns.Items.Clear();
            foreach (DataColumn column in dtList.Columns)
            {
                if (column.DataType == typeof (DateTime))
                {
                    cmbDateTimeColumns.Items.Add(column.ColumnName);
                }
            }
            if (cmbDateTimeColumns.Items.Count > 0)
            {
                cmbDateTimeColumns.SelectedIndex = 0;
            }
            chbUseDateRange_CheckedChanged(this, EventArgs.Empty); // To update "use date range" controls
        }

        #endregion

        #region Private Methods

        private bool ExpotThemes
        {
            get { return _dataToExport == null; }
        }

        /// <summary>
        /// BackgroundWorker method used to create a datatable including data queried from Databasein in all the fields selected.
        /// </summary>
        /// <param name="parameters"> BackgroundWorker parameters passed from Export Button Click Event</param>
        /// <param name="exportdlgWorker"> BackgroundWorker (may be null), in order to show progress</param>
        /// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
        /// <returns>Return the BackgroundWorker result.</returns>
        private string Exportdlg(BwParameters parameters, BackgroundWorker exportdlgWorker, DoWorkEventArgs e)
        {
            if (ExpotThemes)
            {
                return ExportDataSeriesTable(parameters, exportdlgWorker, e);
            }
            return ExportAnyDataTable(parameters, exportdlgWorker, e);
        }

        private string ExportDataSeriesTable(BwParameters parameters, BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            //get parameters
            var fileName = parameters.OutputFileName;
            var dtSeries = parameters.Series;
            var checkNodata = parameters.CheckNoData;
            var delimiter = parameters.Delimiter;
            var checkedItems = parameters.Columns;
            var datesRange = parameters.DatesRange;

            var repo = RepositoryFactory.Instance.Get<IDataValuesRepository>(_dboperation);

            //export data row by row
            for (int r = 0; r < dtSeries.Rows.Count; r++)
            {
                var row = dtSeries.Rows[r];

                //Check for cancel
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return "Data Export Cancelled.";
                }

                var noDataValue = !checkNodata? Convert.ToDouble(row["NoDataValue"]) : (double?) null;

                // Date range filter
                string dateColumn = null;
                DateTime? firstDate = null;
                DateTime? lastDate = null;
                if (datesRange != null)
                {
                    dateColumn = datesRange.ColumnName;
                    firstDate = datesRange.StartDate;
                    lastDate = datesRange.EndDate;
                }
                var tbl = repo.GetTableForExport(Convert.ToInt64(row["SeriesID"]), noDataValue, dateColumn, firstDate,
                                                 lastDate);

                //Construct columns that were selected
                for (int i = 0; i < tbl.Columns.Count; i++)
                {
                    var column = tbl.Columns[i];
                    if (!checkedItems.Contains(column.ColumnName))
                    {
                        tbl.Columns.Remove(column);
                        i--;
                    }
                }

                //Check for cancel
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return "Data Export Cancelled.";
                }

                var includeHeaders = r == 0;
                ImportExport.DelimitedTextWriter.DataTableToDelimitedFile(tbl, fileName, delimiter,
                                                                          includeHeaders, true,
                                                                          backgroundWorker, e,
                                                                          ImportExport.BackgroundWorkerReportingOptions.UserStateAndProgress);

                //progress report
                var percent = (int)(((float)r / dtSeries.Rows.Count) * 100);
                var userState = "Writing series " + r + " of " + dtSeries.Rows.Count + "...";
                backgroundWorker.ReportProgress(percent, userState);
            }

            //Check for cancel
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return "Data Export Cancelled.";
            }

            return "Export completed. Series exported: " + dtSeries.Rows.Count.ToString();
        }

        private string ExportAnyDataTable(BwParameters parameters, BackgroundWorker backgroundWorker, DoWorkEventArgs e)
        {
            var filename = parameters.OutputFileName;
            var checkedItems = parameters.Columns;
            var delimiter = parameters.Delimiter;
            var originalDataTable = parameters.Series;

            //Report status
            backgroundWorker.ReportProgress(0, "Preparing output...");

            DataTable exportDataTable;
            if (checkedItems.Count == originalDataTable.Rows.Count)
            {
                exportDataTable = originalDataTable;
            }
            else
            {
                //Build a new datatable to accept selected columns in the original datatable and used to export.
                exportDataTable = originalDataTable.Copy(); // copy

                //Check for cancel
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return "Data Export Cancelled.";
                }

                //Report status
                backgroundWorker.ReportProgress(0, "Checking columns...");

                //Remove unwanted columns
                foreach (DataColumn column in originalDataTable.Columns)
                {
                    //Check for cancel
                    if (backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return "Data Export Cancelled.";
                    }

                    var columnName = column.ColumnName;
                    if (!checkedItems.Contains(columnName))
                    {
                        exportDataTable.Columns.Remove(columnName);
                    }

                    exportDataTable.AcceptChanges();
                }
            }

            // Check for cancel
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return "Data Export Cancelled.";
            }
            
            ImportExport.DelimitedTextWriter.DataTableToDelimitedFile(exportDataTable, filename, delimiter,
                true, false, backgroundWorker, e,
                ImportExport.BackgroundWorkerReportingOptions.UserStateAndProgress);

            if (backgroundWorker.CancellationPending == true)
            {
                e.Cancel = true;
                return "Data Export Cancelled.";
            }
            else return "Export complete.  Rows exported: " + exportDataTable.Rows.Count.ToString();
        }

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// BackgroundWorker Do event, used to call for the BackgroundWorker method.
        /// </summary>
        private void bgwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            var parameters = e.Argument as BwParameters;
            var worker = sender as BackgroundWorker;
            e.Result = Exportdlg(parameters, worker, e);
        }

        /// <summary>
        /// BackgroundWorker Progress event, used to report the progress when doing BackgroundWorker.
        /// </summary>
        private void bgwMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgsBar.Value = e.ProgressPercentage;
            gbxProgress.Text = e.UserState.ToString();
        }

        /// <summary>
        /// Enable all the buttons again when BackgroundWorker complete working.
        /// </summary>
        private void bgwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Cursor = Cursors.Default;

            // Restore controls to their regular state
            UpdateControlsState(false);

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            else if (e.Cancelled  || e.Result.ToString() == "Data Export Cancelled.")
            {
                btncancel.Enabled = true;
                // Close the form if the user clicked the X to close it.
                if (_formIsClosing)
                {
                    DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                MessageBox.Show(e.Result.ToString());
            }
        }

        #endregion

        #region Button Click Event

        /// <summary>
        ///Specify the location and file name to export, meanwhile check the delimiter.
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (var saveFileDlg = new SaveFileDialog())
            {
                saveFileDlg.Title = "Select file";
                saveFileDlg.OverwritePrompt = false;
                saveFileDlg.Filter = delimiterSelector1.CurrentDelimiter == ","
                                         ? "CSV (Comma delimited) (*.csv)|*.csv|Text (*.txt)|*.txt"
                                         : "Text (*.txt)|*.txt";

                if (saveFileDlg.ShowDialog() == DialogResult.OK)
                {
                    tbOutPutFileName.Text = saveFileDlg.FileName;
                }
            }
        }

        /// <summary>
        /// Export data using BackgroundWorker. Build series table and pass parameters from here to BackgroundWorker.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            // Make sure we aren't still working on a previous task
            if (bgwMain.IsBusy)
            {
                MessageBox.Show("The background worker is busy now. Please try later.", "Export To Text File",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Check the themes  for export.  There should be at least one item selected.
            if (ExpotThemes && clbThemes.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please choose at least one theme to export", "Export To Text File",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Check the desired fields for export.  There should be at least one item selected.
            if (clbExportItems.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please choose at least one field to export", "Export To Text File",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Check whether a delimiter is checked
            var delimiter = delimiterSelector1.CurrentDelimiter;
            if (String.IsNullOrEmpty(delimiter))
            {
                MessageBox.Show("Please input delimiter.", "Export To Text File");
                return;
            }

            //Check the output file path
            string outputFilename = tbOutPutFileName.Text.Trim();

            if (outputFilename == String.Empty)
            {
                MessageBox.Show("Please specify output filename", "Export To Text File");
                return;
            }
            if (Directory.Exists(Path.GetDirectoryName(outputFilename)) == false)
            {
                MessageBox.Show("The directory for the output filename does not exist", "Export To Text File");
                return;
            }

            // Construct DataTable of all the series in the selected theme
            DataTable dtSeries;
            if (ExpotThemes)
            {
                var themeIds =
                    (from ThemeDescription themeDescr in clbThemes.CheckedItems select themeDescr.ThemeId).ToList();
                var repository = RepositoryFactory.Instance.Get<IDataSeriesRepository>(_dboperation);
                dtSeries = repository.GetSeriesIDsWithNoDataValueTable(themeIds);
            }
            else
            {
                dtSeries = _dataToExport;
            }
           
            var checkedItems = new List<string>();
            foreach (var item in clbExportItems.CheckedItems.Cast<string>().Where(item => !checkedItems.Contains(item)))
            {
                checkedItems.Add(item);
            }

            DatesRange datesRange = null;
            if (chbUseDateRange.Checked && cmbDateTimeColumns.SelectedIndex >= 0)
            {
                datesRange = new DatesRange
                                 {
                                     ColumnName = cmbDateTimeColumns.SelectedItem.ToString(),
                                     StartDate = dtpStartDateRange.Value,
                                     EndDate = dtpEndDateRange.Value,
                                 };
            }


            //Disable all the buttons after "Export" button is clicked.
            UpdateControlsState(true);

            // Show hourglass
            Cursor = Cursors.WaitCursor;

            //pass parameters to BackgroundWorker
            var parameters = new BwParameters
                                 {
                                     CheckNoData = chkNodata.Checked,
                                     Columns = checkedItems,
                                     DatesRange = datesRange,
                                     Delimiter = delimiter,
                                     OutputFileName = outputFilename,
                                     Series = dtSeries,
                                 };

            // Check for overwrite
            if (File.Exists(outputFilename))
            {
                var message = "File " + outputFilename + " already exists.\nWould you like to replace it?";
                var replace = MessageBox.Show(message, "Export To Text File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (replace == DialogResult.No)
                {
                    Cursor = Cursors.Default;

                    // Restore controls to their regular state
                    UpdateControlsState(false);
                    return;
                }

                File.Delete(outputFilename);
                bgwMain.RunWorkerAsync(parameters);
            }

            else
                bgwMain.RunWorkerAsync(parameters);
        }

        private void UpdateControlsState(bool isExporting)
        {
            gbxDatesRange.Enabled = !isExporting;
            btnExport.Enabled = !isExporting;
            gbxThemes.Enabled = !isExporting;
            gbxExport.Enabled = !isExporting;
            btnSelectAllFields.Enabled = !isExporting;
            btnSelectNoneFields.Enabled = !isExporting;
            delimiterSelector1.Enabled = !isExporting;
            gbxFields.Enabled = !isExporting;
            gbxProgress.Enabled = isExporting;
            gbxProgress.Visible = isExporting;
            if (!isExporting)
            {
                pgsBar.Value = 0;
                gbxProgress.Text = "Processing...";
            }
        }

        /// <summary>
        /// Opens a help topic for the item in context when the Help button is clicked.
        /// </summary>
        private void ThemeExportDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            LocalHelp.OpenHelpFile(_localHelpUri);
            e.Cancel = true; // Prevents mouse cursor from changing to question mark.
        }

        /// <summary>
        /// Opens a help topic for the item in context when the user presses F1.
        /// </summary>
        private void ThemeExportDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            LocalHelp.OpenHelpFile(_localHelpUri);
            hlpevent.Handled = true; // Signal that we've handled the help request.
        }

        #region CheckListBox Selection Events

        /// <summary>
        /// If users want all the items, "Select All" button add all of them to the hashtable _checkedItems as Key.
        /// </summary>
        private void SelectAll_Click(object sender, EventArgs e)
        {
            SetCheckedItems(clbExportItems, true);
        }

        /// <summary>
        /// Users can use "Select None" button to remove all the items from the hashtable, and then select what they want again.
        /// </summary>
        private void SelectNone_Click(object sender, EventArgs e)
        {
            SetCheckedItems(clbExportItems, false);
        }

        private void btnSelectAllThemes_Click(object sender, EventArgs e)
        {
            SetCheckedItems(clbThemes, true);
        }

        private void btnSelectNoneThemes_Click(object sender, EventArgs e)
        {
            SetCheckedItems(clbThemes, false);
        }

        private void SetCheckedItems(CheckedListBox clb, bool isChecked)
        {
            for (int c = 0; c < clb.Items.Count; c++)
            {
                clb.SetItemChecked(c, isChecked);
            }
        }

        private void chbUseDateRange_CheckedChanged(object sender, EventArgs e)
        {
            var useDateRange = chbUseDateRange.Checked;
            cmbDateTimeColumns.Enabled = useDateRange;
            dtpStartDateRange.Enabled = useDateRange;
            lblAndRange.Enabled = useDateRange;
            dtpEndDateRange.Enabled = useDateRange;
        }

        #endregion

        #region Cancel Events

        /// <summary>
        /// When "Cancel" button is clicked during the exporting process, BackgroundWorker stops.
        /// </summary>
        private void Cancel_worker()
        {
            bgwMain.CancelAsync();
            gbxProgress.Text = "Cancelling...";
            btncancel.Enabled = false;
        }

        /// <summary>
        /// When Export Form is closed, BackgroundWorker has to stop.
        /// </summary>
        private void ExportDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bgwMain.IsBusy) return;
            Cancel_worker();
            _formIsClosing = true;
            e.Cancel = true;
        }

        /// <summary>
        /// Close the form if Cancel button is clicked before or after an export event.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (bgwMain.IsBusy)
            {
                Cancel_worker();
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        #endregion

        #endregion

        #region Helpers

        private class ThemeDescription
        {
            public int? ThemeId { get; private set; }
            public string ThemeName { get; private set; }

            public ThemeDescription(int? themeId, string themeName)
            {
                ThemeId = themeId;
                ThemeName = themeName;
            }

            public override string ToString()
            {
                return ThemeName;
            }
        }

        private class DatesRange
        {
            public string ColumnName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private class BwParameters
        {
            public string OutputFileName { get; set; }
            public DataTable Series { get; set; }
            public bool CheckNoData { get; set; }
            public string Delimiter { get; set; }
            public List<string> Columns { get; set; }
            public DatesRange DatesRange { get; set; }
        }

        #endregion
    }
}
