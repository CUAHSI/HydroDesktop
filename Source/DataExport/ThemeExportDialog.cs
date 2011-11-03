using System;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

using HydroDesktop.Database;
using HydroDesktop.Help;

namespace HydroDesktop.ExportToCSV
{
	/// <summary>
	/// Export Data Form with BackgroundWorker and allow users to select themes to export.
	/// </summary>
	public partial class ThemeExportDialog : Form
	{
		#region Variables

		private readonly DbOperations _dboperation;
		private bool _formIsClosing = false;
		private DataTable _dtList = new DataTable ();
		SaveFileDialog _saveFileDlg = new SaveFileDialog ();
		private readonly string _localHelpUri = Properties.Settings.Default.localHelpUri;

		#endregion

		#region Constructor

		/// <summary>
		/// Initialize the ExportData form, and create a connection for building datatable.
		/// </summary>
		public ThemeExportDialog (DbOperations dbOperation)
		{
		    if (dbOperation == null) throw new ArgumentNullException("dbOperation");

		    InitializeComponent ();
            _dboperation = dbOperation;
		}

		#endregion

		#region Initialization

		/// <summary>
		/// Initialize the export form.
		/// </summary>
		private void ExportDialog_load ( object sender, EventArgs e )
		{
			Cursor = Cursors.WaitCursor;

			//populate list box with list of themes
			var dtThemes = _dboperation.LoadTable ( "themes", "SELECT ThemeID, ThemeName from DataThemeDescriptions" );
            clbThemes.Items.Clear();
            foreach(DataRow row in dtThemes.Rows)
            {
                clbThemes.Items.Add(new ThemeDescription(Convert.ToInt32(row["ThemeID"]), 
                                                         row["ThemeName"].ToString()), true);
            }
		    

			// Populate checked list box with list of fields to export
			LoadFieldList ();

			Cursor = Cursors.Default;
		}

	    /// <summary>
		/// Initialize the ChecklistBox to show all the fields necessary.
		/// </summary>
		private void LoadFieldList ()
		{
			//Set fields in CheckListBox
			string list;
            list = "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.LocalDateTime, dv.DataValue, dv.CensorCode, U1.UnitsName As VarUnits, v.DataType, s.SiteID, s.SiteCode, v.VariableID, v.VariableCode, " +
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
					"AND ds.SeriesID = 1";

			_dtList = _dboperation.LoadTable ( "dtList", list );

			//Headers shown in CheckListBox
			string[] headers = new string[_dtList.Columns.Count];
			for ( int i = 0; i < _dtList.Columns.Count; i++ )
			{
				headers[i] = _dtList.Columns[i].ToString ();
			}

			//Initialize items in CheckedlistBox
			clbExportItems.Items.AddRange ( headers );
			for ( int h = 0; h < clbExportItems.Items.Count; h++ )
			{
				clbExportItems.SetItemChecked ( h, true );
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// BackgroundWorker method used to create a datatable including data queried from Databasein in all the fields selected.
		/// </summary>
		/// <param name="parameters"> BackgroundWorker parameters passed from Export Button Click Event</param>
		/// <param name="exportdlg_worker"> BackgroundWorker (may be null), in order to show progress</param>
		/// <param name="e">Arguments from a BackgroundWorker (may be null), in order to support canceling</param>
		/// <returns>Return the BackgroundWorker result.</returns>
		private string Exportdlg ( object[] parameters, BackgroundWorker exportdlg_worker, DoWorkEventArgs e )
		{
			bool includeHeaders = true;
			bool append = true;

			//get parameters
			string fileName = (string)parameters[0];
			DataTable dtSeries = (DataTable)parameters[1];
			bool checkNodata = (bool)parameters[2];
			string delimiter = (string)parameters[3];
			Hashtable checkedItems = (Hashtable)parameters[4];
			DbOperations dbOperations = (DbOperations)parameters[5];

			//get count for ProgressReport
			int totalSeriesCount = dtSeries.Rows.Count;
			int count = 0;

			//export data row by row
			foreach ( DataRow row in dtSeries.Rows )
			{
				//Check for cancel
				if ( exportdlg_worker.CancellationPending == true )
				{
					e.Cancel = true;
					return "Data Export Cancelled.";
				}

				object objNoData = row["NoDataValue"];
				double noDataValue = Convert.ToDouble ( objNoData );

				int seriesID = Convert.ToInt32 ( row["SeriesID"] );

				string sql;
				if ( checkNodata == true )
				{
                    sql = "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.LocalDateTime, dv.DataValue, U1.UnitsName As VarUnits, v.DataType, s.SiteID, s.SiteCode, v.VariableID, v.VariableCode, " +
                          "S.Organization, S.SourceDescription, S.SourceLink, v.ValueType, v.TimeSupport, U2.UnitsName As TimeUnits, v.IsRegular, v.NoDataValue, " +
                          "dv.UTCOffset, dv.DateTimeUTC, s.Latitude, s.Longitude, dv.ValueAccuracy, dv.CensorCode, m.MethodDescription, q.QualityControlLevelCode, v.SampleMedium, v.GeneralCategory " +
					"FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U1, Units U2, Methods m, QualityControlLevels q, Sources S " +
					"WHERE v.VariableID = ds.VariableID " +
					"AND s.SiteID = ds.SiteID " +
					"AND m.MethodID = ds.MethodID " +
					"AND q.QualityControlLevelID = ds.QualityControlLevelID " +
					"AND S.SourceID = ds.SourceID " +
					"AND dv.SeriesID = ds.SeriesID " +
					"AND U1.UnitsID = v.VariableUnitsID " +
                    "AND U2.UnitsID = v.TimeUnitsID " +
					"AND ds.SeriesID = " + row["SeriesID"].ToString ();
				}
				else
				{
                    sql = "SELECT ds.SeriesID, s.SiteName, v.VariableName, dv.LocalDateTime, dv.DataValue, U1.UnitsName As VarUnits, v.DataType, s.SiteID, s.SiteCode, v.VariableID, v.VariableCode, " +
                          "S.Organization, S.SourceDescription, S.SourceLink, v.ValueType, v.TimeSupport, U2.UnitsName As TimeUnits, v.IsRegular, v.NoDataValue, " +
                          "dv.UTCOffset, dv.DateTimeUTC, s.Latitude, s.Longitude, dv.ValueAccuracy, dv.CensorCode, m.MethodDescription, q.QualityControlLevelCode, v.SampleMedium, v.GeneralCategory " +
                    "FROM DataSeries ds, Sites s, Variables v, DataValues dv, Units U1, Units U2, Methods m, QualityControlLevels q, Sources S " +
					"WHERE dv.DataValue != " + noDataValue + " " +
					"AND v.VariableID = ds.VariableID " +
					"AND U1.UnitsID = v.VariableUnitsID " +
                    "AND U2.UnitsID = v.TimeUnitsID " +
					"AND q.QualityControlLevelID = ds.QualityControlLevelID " +
					"AND S.SourceID = ds.SourceID " +
					"AND s.SiteID = ds.SiteID " +
					"AND m.MethodID = ds.MethodID " +
					"AND dv.SeriesID = ds.SeriesID " +
					"AND ds.SeriesID = " + row["SeriesID"].ToString ();
				}

				DataTable tbl = dbOperations.LoadTable ( "values", sql );

				//Construct columns that were selected
				for ( int i = 0; i < tbl.Columns.Count; i++ )
				{
					DataColumn column = tbl.Columns[i];
					if ( checkedItems.ContainsKey ( column.ColumnName ) == false )
					{
						tbl.Columns.Remove ( column );
						i--;
					}
				}

				//Check for cancel
				if ( exportdlg_worker.CancellationPending == true )
				{
					e.Cancel = true;
					return "Data Export Cancelled.";
				}
                
                HydroDesktop.ImportExport.DelimitedTextWriter.DataTableToDelimitedFile ( tbl, fileName, delimiter, includeHeaders, append, exportdlg_worker, e, HydroDesktop.ImportExport.BackgroundWorkerReportingOptions.UserStateAndProgress );

		        if ( includeHeaders == true )
		        {
			        includeHeaders = false;
		        }

				//progress report
				count++;
				int percent = (int)(((float)count / (float)totalSeriesCount) * 100);
				string userState = "Writing series " + count + " of " + totalSeriesCount + "...";
				exportdlg_worker.ReportProgress ( percent, userState );
			}

			//Check for cancel
			if ( exportdlg_worker.CancellationPending == true )
			{
				e.Cancel = true;
				return "Data Export Cancelled.";
			}

			else return "Export completed. Series exported: " + dtSeries.Rows.Count.ToString ();
		}

		#endregion

		#region BackgroundWorker
		/// <summary>
		/// BackgroundWorker Do event, used to call for the BackgroundWorker method.
		/// </summary>
		private void bgwMain_DoWork ( object sender, DoWorkEventArgs e )
		{
			object[] parameters = e.Argument as object[];
			BackgroundWorker worker = sender as BackgroundWorker;
			e.Result = Exportdlg ( parameters, worker, e );
		}

		/// <summary>
		/// BackgroundWorker Progress event, used to report the progress when doing BackgroundWorker.
		/// </summary>
		private void bgwMain_ProgressChanged ( object sender, ProgressChangedEventArgs e )
		{
			this.pgsBar.Value = e.ProgressPercentage;
			this.gbxProgress.Text = e.UserState.ToString ();
		}

		/// <summary>
		/// Enable all the buttons again when BackgroundWorker complete working.
		/// </summary>
		private void bgwMain_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e )
		{
			this.Cursor = Cursors.Default;

			// Restore controls to their regular state
			gbxThemes.Enabled = true;
			gbxExportData.Enabled = true;
			gbxExportData.Visible = true;
			gbxExport.Enabled = true;
			btnSelectAllFields.Enabled = true;
			btnSelectNoneFields.Enabled = true;
			gbxDelimiters.Enabled = true;
			gbxFields.Enabled = true;
			pgsBar.Value = 0;
			gbxProgress.Text = "Processing...";
			gbxProgress.Enabled = false;
			gbxProgress.Visible = false;
			btnPgsCancel.Enabled = true;

			if ( e.Error != null )
			{
				MessageBox.Show ( e.Error.Message );
			}

			else if ( e.Cancelled == true || e.Result.ToString () == "Data Export Cancelled." )
			{
				// Close the form if the user clicked the X to close it.
				if ( _formIsClosing == true )
				{
					this.DialogResult = DialogResult.Cancel;
				}
			}
			else
			{
				MessageBox.Show ( e.Result.ToString () );
			}
		}

		#endregion

		#region Button Click Event

		/// <summary>
		///Specify the location and file name to export, meanwhile check the delimiter.
		/// </summary>
		private void btnBrowse_Click ( object sender, EventArgs e )
		{
			_saveFileDlg.Title = "Select file";
			_saveFileDlg.OverwritePrompt = false;

			if ( rdoComma.Checked == true )
			{
				_saveFileDlg.Filter = "CSV (Comma delimited) (*.csv)|*.csv|Text (*.txt)|*.txt";
			}
			else
			{
				_saveFileDlg.Filter = "Text (*.txt)|*.txt";
			}

			if ( _saveFileDlg.ShowDialog () == DialogResult.OK )
			{
				tbOutPutFileName.Text = _saveFileDlg.FileName;
			}
		}

		/// <summary>
		/// Export data using BackgroundWorker. Build series table and pass parameters from here to BackgroundWorker.
		/// </summary>
		private void btnExport_Click ( object sender, EventArgs e )
		{
			// Make sure we aren't still working on a previous task
			if ( bgwMain.IsBusy == true )
			{
                MessageBox.Show("The background worker is busy now. Please try later.", "Export To Text File", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

            //Check the themes  for export.  There should be at least one item selected.
            if (clbThemes.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please choose at least one theme to export", "Export To Text File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

			//Check the desired fields for export.  There should be at least one item selected.
			if ( clbExportItems.CheckedItems.Count == 0 )
			{
                MessageBox.Show("Please choose at least one field to export", "Export To Text File", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			//Check whether a delimiter is checked
			string delimiter = "";
			if ( rdoComma.Checked ) delimiter = ",";
			if ( rdoTab.Checked ) delimiter = "\t";
			if ( rdoSpace.Checked ) delimiter = "\0";
			if ( rdoPipe.Checked ) delimiter = "|";
			if ( rdoSemicolon.Checked ) delimiter = ";";

			if ( rdoOthers.Checked == true )
			{
				if ( tbOther.Text.Length != 0 )
				{
					delimiter = tbOther.Text.ToString ();
				}
				else
				{
					MessageBox.Show ( "Please input delimiter.", "Export To Text File" );
					return;
				}
			}

			//Check the output file path
			string outputFilename = tbOutPutFileName.Text.Trim ();

			if ( outputFilename == String.Empty )
			{
				MessageBox.Show ( "Please specify output filename", "Export To Text File" );
				return;
			}
			else if ( Directory.Exists ( Path.GetDirectoryName ( outputFilename ) ) == false )
			{
				MessageBox.Show ( "The directory for the output filename does not exist", "Export To Text File" );
				return;
			}

			// Construct DataTable of all the series in the selected theme
		    var themeIds = new StringBuilder();
            foreach(var item in clbThemes.CheckedItems)
            {
                var themeDescr = (ThemeDescription)item;
                themeIds.AppendFormat("{0},", themeDescr.ThemeId);
            }
		    themeIds.Remove(themeIds.Length - 1, 1);
		    
            DataTable dtSeries = _dboperation.LoadTable("series",
			                                              "SELECT v.NoDataValue, ds.SeriesID " +
			                                              "FROM DataSeries ds INNER JOIN variables v ON ds.VariableID = v.VariableID " +
			                                              "INNER JOIN DataThemes t ON ds.SeriesID = t.SeriesID " +
			                                              "WHERE t.ThemeID in (" + themeIds + ")");

		    var checkNoData = chkNodata.Checked;
			//Add all checked items into the HashTable
			Hashtable checkedItems = new Hashtable ();
			foreach ( string item in clbExportItems.CheckedItems )
			{
				if ( checkedItems.Contains ( item ) == false )
				{
					checkedItems.Add ( item, item );
				}
			}

			//Disable all the buttons after "Export" button is clicked.
			gbxExportData.Enabled = false;
			gbxExportData.Visible = false;
			gbxThemes.Enabled = false;
			gbxExport.Enabled = false;
			btnSelectAllFields.Enabled = false;
			btnSelectNoneFields.Enabled = false;
			gbxDelimiters.Enabled = false;
			gbxFields.Enabled = false;
			gbxProgress.Enabled = true;
			gbxProgress.Visible = true;
			btnPgsCancel.Enabled = true;

			// Show hourglass
			this.Cursor = Cursors.WaitCursor;

			//pass parameters to BackgroundWorker
			object[] parameters = new object[6];
			parameters[0] = outputFilename;
			parameters[1] = dtSeries;
			parameters[2] = checkNoData;
			parameters[3] = delimiter;
			parameters[4] = checkedItems;
            parameters[5] = _dboperation;

            // Check for overwrite
            if (File.Exists(outputFilename) == true)
            {
                string message = "File " + outputFilename + " already exists.\nWould you like to replace it?";

                DialogResult replace = MessageBox.Show(message, "Export To Text File", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if ( replace == DialogResult.No)
                {
                    this.Cursor = Cursors.Default;

                    // Restore controls to their regular state
                    gbxThemes.Enabled = true;
                    gbxExportData.Enabled = true;
                    gbxExportData.Visible = true;
                    gbxExport.Enabled = true;
                    btnSelectAllFields.Enabled = true;
                    btnSelectNoneFields.Enabled = true;
                    gbxDelimiters.Enabled = true;
                    gbxFields.Enabled = true;
                    pgsBar.Value = 0;
                    gbxProgress.Text = "Processing...";
                    gbxProgress.Enabled = false;
                    gbxProgress.Visible = false;
                    btnPgsCancel.Enabled = true;

                    return;
                }

                else
                {
                    File.Delete(outputFilename);
                    bgwMain.RunWorkerAsync(parameters);
                }
            }

            else 
                bgwMain.RunWorkerAsync ( parameters );
		}

		/// <summary>
		///Set the "Others" radiobutton be selected automatically when the textbox is changed.
		/// </summary>        
		private void other_TextChanged ( object sender, EventArgs e )
		{
			if ( tbOther.Text.Length != 0 )
			{
				rdoOthers.Checked = true;
			}
		}

		/// <summary>
		/// Opens a help topic for the item in context when the Help button is clicked.
		/// </summary>
		private void ThemeExportDialog_HelpButtonClicked ( object sender, CancelEventArgs e )
		{
			LocalHelp.OpenHelpFile ( _localHelpUri );
			e.Cancel = true; // Prevents mouse cursor from changing to question mark.
		}

		/// <summary>
		/// Opens a help topic for the item in context when the user presses F1.
		/// </summary>
		private void ThemeExportDialog_HelpRequested ( object sender, HelpEventArgs hlpevent )
		{
			LocalHelp.OpenHelpFile ( _localHelpUri );
			hlpevent.Handled = true; // Signal that we've handled the help request.
		}

		#region CheckListBox Selection Events

		/// <summary>
		/// If users want all the items, "Select All" button add all of them to the hashtable _checkedItems as Key.
		/// </summary>
		private void SelectAll_Click ( object sender, EventArgs e )
		{
            SetCheckedItems(clbExportItems, true);
		}

		/// <summary>
		/// Users can use "Select None" button to remove all the items from the hashtable, and then select what they want again.
		/// </summary>
		private void SelectNone_Click ( object sender, EventArgs e )
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


	    #endregion

		#region Cancel Events

		/// <summary>
		/// When "Cancel" button is clicked during the exporting process, BackgroundWorker stops.
		/// </summary>
		private void Cancel_worker ()
		{
			bgwMain.CancelAsync ();
			gbxProgress.Text = "Cancelling...";
			btnPgsCancel.Enabled = false;
		}

		/// <summary>
		/// Call "Cancel_worker" when button click happens.
		/// </summary>
		private void pgsCancel_Click ( object sender, EventArgs e )
		{
			Cancel_worker ();
		}

		/// <summary>
		/// When Export Form is closed, BackgroundWorker has to stop.
		/// </summary>
		private void ExportDialog_FormClosing ( object sender, FormClosingEventArgs e )
		{
			if ( bgwMain.IsBusy )
			{
				Cancel_worker ();
				_formIsClosing = true;
				e.Cancel = true;
				return;
			}
		}

		/// <summary>
		/// Close the form if Cancel button is clicked before or after an export event.
		/// </summary>
		private void btnCancel_Click ( object sender, EventArgs e )
		{
			this.DialogResult = DialogResult.Cancel;
		}

		#endregion
        
		#endregion

        #region Helpers

        private class ThemeDescription
        {
            public int ThemeId { get; private set; }
            public string ThemeName { get; private set; }

            public ThemeDescription(int themeId, string themeName)
            {
                ThemeId = themeId;
                ThemeName = themeName;
            }

            public override string ToString()
            {
                return ThemeName;
            }
        }

        #endregion

    }
}
