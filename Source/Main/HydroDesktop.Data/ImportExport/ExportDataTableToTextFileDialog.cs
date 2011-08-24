using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

using HydroDesktop.Database;

namespace HydroDesktop.ImportExport
{
    /// <summary>
    /// Right Click Export Data Form in TableView with BackgroundWorker.
    /// </summary>
    public partial class ExportDataTableToTextFileDialog : Form
    {
        #region Variables

        private bool _formIsClosing = false;
        private DataTable _originalDataTable;
        SaveFileDialog _saveFileDlg = new SaveFileDialog();

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the RightClickExport form, and pass the users' datatable to the form
        /// </summary>
        /// <param name="dataToExport"> DataTable used to pass data from the users' datatable.</param>
        public ExportDataTableToTextFileDialog(DataTable originalDataTable)
        {
            if (originalDataTable.Columns.Count == 0)
            {
                throw new ArgumentException("Data table for export must have at least one column.");
            }
            else if (originalDataTable.Rows.Count == 0)
            {
                throw new ArgumentException("Data table for export must have at least one row.");
            }

            _originalDataTable = originalDataTable;

            InitializeComponent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize the ChecklistBox to show all the fields that included in the users' datatable.
        /// </summary>
        private void ListBox_load(object sender, EventArgs e)
        {
            //Headers shown in CheckListBox
            string[] headers = new string[_originalDataTable.Columns.Count];
            for (int i = 0; i < _originalDataTable.Columns.Count; i++)
            {
                headers[i] = _originalDataTable.Columns[i].ToString();
            }

            //Initialize items in CheckedlistBox
            clbFieldsToExport.Items.AddRange(headers);
            for (int h = 0; h < clbFieldsToExport.Items.Count; h++)
            {
                clbFieldsToExport.SetItemChecked(h, true);
            }
        }

        /// <summary>
        /// BackgroundWorker method used to create a datatable including data queried from Databasein in all the fields selected.
        /// </summary>
        /// <param name="filename"> BackgroundWorker argument passed from Export Button Click Event</param>
        /// <param name="exportdlg_worker"> BackgroundWorker (may be null), in order to show progress</param>
        /// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
        /// <returns>Return the BackgroundWorker result.</returns>
        private string BGWExportdlg(object[] parameters, BackgroundWorker exportdlg_worker, DoWorkEventArgs e)
        {
            string filename = (string)parameters[0];
            Hashtable checkedItems = (Hashtable)parameters[1];
            string delimiter = (string)parameters[2];
            DataTable originalDataTable = (DataTable)parameters[3];

            DataTable exportDataTable = new DataTable();

            //Report status
            exportdlg_worker.ReportProgress(0, "Preparing output...");

            if (checkedItems.Count == originalDataTable.Rows.Count)
            {
                exportDataTable = originalDataTable;
            }

            else
            {
                //Build a new datatable to accept selected columns in the original datatable and used to export.
                exportDataTable = originalDataTable.Copy(); // copy

                //Check for cancel
                if (exportdlg_worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return "Data Export Cancelled.";
                }

                //Report status
                exportdlg_worker.ReportProgress(0, "Checking columns...");

                //Remove unwanted columns
                foreach (DataColumn column in originalDataTable.Columns)
                {
                    
                    //Check for cancel
                    if (exportdlg_worker.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return "Data Export Cancelled.";
                    }

                    string columnName = column.ColumnName;
                    if (checkedItems.Contains(columnName) == false)
                    {
                        exportDataTable.Columns.Remove(columnName);
                    }

                    exportDataTable.AcceptChanges();
                }                 
            }

            // Check for cancel
            if (exportdlg_worker.CancellationPending == true)
            {
                e.Cancel = true;
                return "Data Export Cancelled.";
            }

            //get count for ProgressReport
            int totalSeriesCount = exportDataTable.Rows.Count;

            bool includeHeaders = true;
            bool append = false;
            HydroDesktop.ImportExport.DelimitedTextWriter.DataTableToDelimitedFile(exportDataTable, filename, delimiter, 
                includeHeaders, append, exportdlg_worker, e, 
                HydroDesktop.ImportExport.BackgroundWorkerReportingOptions.UserStateAndProgress);

            if (exportdlg_worker.CancellationPending == true)
            {
                e.Cancel = true;
                return "Data Export Cancelled.";
            }
            else return "Export complete.  Rows exported: " + exportDataTable.Rows.Count.ToString();
        }

        /// <summary>
        /// When "Cancel" button is clicked during the exporting process, BackgroundWorker stops.
        /// </summary>
        private void Cancel_worker()
        {
            bgwMain.CancelAsync();
            gbxProgresses.Text = "Cancelling...";
            btnPgsCancel.Enabled = false;
        }

        #endregion

        #region BackgroundWorker

        /// <summary>
        /// BackgroundWorker Do event, used to call for the BackgroundWorker method.
        /// </summary>
        private void bgwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] parameters = e.Argument as object[];
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = BGWExportdlg(parameters, worker, e);
        }

        /// <summary>
        /// BackgroundWorker Progress event, used to report the progress when doing BackgroundWorker.
        /// </summary>
        private void bgwMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.pgsBar.Value = e.ProgressPercentage;
            this.gbxProgresses.Text = e.UserState.ToString();
        }

        /// <summary>
        /// Enable all the buttons again when BackgroundWorker complete working.
        /// </summary>
        private void bgwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;

            // Restore controls to their regular state
            btnPgsCancel.Enabled = true;
            gbxExportdata.Enabled = true;
            gbxExportdata.Visible = true;
            gbxExport.Enabled = true;
            btnSelectAll.Enabled = true;
            btnSelectNone.Enabled = true;
            gbxDelimiter.Enabled = true;
            gbxFields.Enabled = true;
            pgsBar.Value = 0;
            gbxProgresses.Text = "Processing...";
            gbxProgresses.Enabled = false;
            gbxProgresses.Visible = false;

            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }
            else if (e.Cancelled == true || e.Result.ToString() == "Data Export Cancelled.")
            {
                // Close the form if the user clicked the X to close it.
                if (_formIsClosing == true)
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
            else
            {
                MessageBox.Show(e.Result.ToString(),"Export To Text File");
				this.DialogResult = DialogResult.OK;
            }
        }

        #endregion

        #region Button Click Events

        /// <summary>
        ///Specify the location and file name to export.
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            _saveFileDlg.Title = "Select file";
			_saveFileDlg.OverwritePrompt = false;

            if (rdoComma.Checked == true)
            {
                _saveFileDlg.Filter = "CSV (Comma delimited) (*.csv)|*.csv|Text (*.txt)|*.txt";
            }
            else
            {
                _saveFileDlg.Filter = "Text (*.txt)|*.txt";
            }

            if (_saveFileDlg.ShowDialog() == DialogResult.OK)
            {
                tbOutputFilename.Text = _saveFileDlg.FileName;
            }
        }

        /// <summary>
        /// Check outputFileNames, delimiter and add checked items to the Hashtable. Export data using BackgroundWorker. Build series table and pass parameters from here to BackgroundWorker.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
			// Make sure the background worker isn't already doing some work
            if (bgwMain.IsBusy == true)
            {
                MessageBox.Show("The background worker is busy now. Please try again later.", "Export To Text File");
                return;
            }

            //Check the desired fields for export.  There should be at least one item selected.
			Hashtable checkedItems = new Hashtable ();

			if ( clbFieldsToExport.CheckedItems.Count == 0 )
            {
                MessageBox.Show("Please choose at least one field to export", "Export To Text File");
                return;
            }

            foreach (string item in clbFieldsToExport.CheckedItems)
            {
                if (checkedItems.Contains(item) == false)
                {
                    checkedItems.Add(item, item);
                }
            }

            //Check which delimiter is checked
            string delimiter = "";
            if (rdoComma.Checked) delimiter = ",";
            if (rdoTab.Checked) delimiter = "\t";
            if (rdoSpace.Checked) delimiter = "\0";
            if (rdoPipe.Checked) delimiter = "|";
            if (rdoSemicolon.Checked) delimiter = ";";

            if (rdoOthers.Checked == true)
            {
                if (tbOther.Text.Length != 0)
                {
                    delimiter = tbOther.Text.ToString();
                }
                else
                {
                    MessageBox.Show("Please input delimiter", "Export To Text File");
                    return;
                }
            }

            //Check the output file path
            string outputFilename = tbOutputFilename.Text.Trim();
            if (outputFilename == String.Empty)
            {
                MessageBox.Show("Please specify output filename", "Export To Text File");
                return;
            }
            else if (Directory.Exists(Path.GetDirectoryName(outputFilename)) == false)
            {
                MessageBox.Show("The directory for the output filename does not exist", "Export To Text File");
                return;
            }

			// Check for overwrite
			if ( File.Exists ( outputFilename ) == true )
			{
				string message = "File " + outputFilename + " already exists.\nDo you want to replace it?";
				if ( MessageBox.Show ( message, "Export To Text File", MessageBoxButtons.YesNo ) == DialogResult.No )
				{
					return;
				}
			}


            //Disable all the buttons after "Export" button is clicked.
            gbxExportdata.Enabled = false;
            gbxExportdata.Visible = false;
            gbxExport.Enabled = false;
            btnSelectAll.Enabled = false;
            btnSelectNone.Enabled = false;
            gbxDelimiter.Enabled = false;
            gbxFields.Enabled = false;
            gbxProgresses.Enabled = true;
            gbxProgresses.Visible = true;
            btnPgsCancel.Enabled = true;

			// Show hourglass
			this.Cursor = Cursors.WaitCursor;

            //pass parameters to BackgroundWorker
            object[] parameters = new object[4];
            parameters[0] = outputFilename;
            parameters[1] = checkedItems;
            parameters[2] = delimiter;
            parameters[3] = _originalDataTable;

            bgwMain.RunWorkerAsync(parameters);
        }

        /// <summary>
        ///Set the "Others" radiobutton be selected automatically when the textbox is changed.
        /// </summary>   
        private void other_TextChanged(object sender, EventArgs e)
        {
            if (tbOther.Text.Length != 0)
            {
                rdoOthers.Checked = true;
            }
        }

        #region CheckListBox Selection Events

        /// <summary>
        /// Checks all items in the list of fields to export.
        /// </summary>
        private void SelectAll_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < clbFieldsToExport.Items.Count; c++)
            {
                clbFieldsToExport.SetItemChecked(c, true);
            }
        }

        /// <summary>
        /// Unchecks all items in the list of fields to export.
        /// </summary>
        private void SelectNone_Click(object sender, EventArgs e)
        {
            for (int c = 0; c < clbFieldsToExport.Items.Count; c++)
            {
                clbFieldsToExport.SetItemChecked(c, false);
            }
        }

        #endregion

        #region Cancel Events

        /// <summary>
        /// Call "Cancel_worker" when button click happens.
        /// </summary>
        private void pgsCancel_Click(object sender, EventArgs e)
        {
            Cancel_worker();
        }

        /// <summary>
        /// When Export Form is closed, BackgroundWorker has to stop.
        /// </summary>
        private void ExportDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgwMain.IsBusy)
            {
                Cancel_worker();
                _formIsClosing = true;
                e.Cancel = true;
                return;
            }
        }

        /// <summary>
        /// Close the form if Cancel button is clicked before or after an export event.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        #endregion

        #endregion
    }
}
