using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.ImportExport;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.MetadataFetcher.Forms
{
	public partial class AddServicesForm : Form
	{
		#region Variables

		private enum BackgroundWorkerTasks
		{
			AddServicesFromHydroServer,
			AddServicesFromHydroPortal,
			AddServicesFromFile,
			CheckForExisingServices,
			UpdateDatabase,
			Unknown
		}

		private bool _formIsClosing = false;

		#endregion

		#region Constructor

		public AddServicesForm ()
		{
			InitializeComponent ();
		}

		#endregion

		#region Private Methods

		#region Form Loading / Unloading

		private void AddServicesForm_Load ( object sender, EventArgs e )
		{
			// Set column heading style
			dgvAddServices.EnableHeadersVisualStyles = false;

			Font boldFont = new Font ( dgvAddServices.Font, FontStyle.Bold );

			dgvAddServices.Columns[0].HeaderCell.Style.Font = boldFont;
			dgvAddServices.Columns[1].HeaderCell.Style.Font = boldFont;
		}

		void AddServicesForm_FormClosing ( object sender, FormClosingEventArgs e )
		{
			if (bgwMain.IsBusy)
			{
			    e.Cancel = true;
                _formIsClosing = true;
			    CancelWorker();
            }
		}

		#endregion

		#region Utilities

        private void SelectDataGridViewRow(DataGridView dataGridView, int rowIndex)
        {
            if (rowIndex < 0 || dataGridView==null || rowIndex >= dataGridView.RowCount)
                return;

            //dataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
            //dataGridView.Refresh();
            //dataGridView.CurrentCell = dataGridView.Rows[rowIndex].Cells[0];
            dataGridView.Rows[rowIndex].Selected = true;
        }

		/// <summary>
		/// Creates a string array that can be used to add a row to a data grid view, based on properties of a DataServiceInfo object
		/// </summary>
		/// <param name="serviceInfo">Details about the WaterOneFlow service</param>
		/// <returns>String array of properties from the DataServiceInfo object</returns>
		private string[] CreateServiceRow ( DataServiceInfo serviceInfo )
		{
			string[] row = { serviceInfo.ServiceTitle, 
							 serviceInfo.EndpointURL, 
							 serviceInfo.ServiceCode, 
							 serviceInfo.Citation, 
							 serviceInfo.Abstract, 
							 serviceInfo.DescriptionURL, 
							 serviceInfo.ContactName, 
							 serviceInfo.ContactEmail };

			return row;
		}

		/// <summary>
		/// Disables/Enables controls and sets mouse cursor in preparation for a BackgroundWorker to run
		/// </summary>
		private void SetupFormForWork ( bool showProgressBar )
		{
			// Disable controls until the asynchronous operation is done
			dgvAddServices.Enabled = false;
			btnCheckExisting.Enabled = false;
			btnUpdate.Enabled = false;
			menuStrip1.Enabled = false;

			// Enable the Cancel button while the asynchronous operation runs
			if ( showProgressBar == true )
			{
				gbxUpdate.Visible = false;
				gbxProgress.Enabled = true;
				btnCancel.Enabled = true;
				gbxProgress.Visible = true;
			}

			this.Cursor = Cursors.WaitCursor;
		}

		/// <summary>
		/// Disables/Enables controls and sets mouse cursor once a BackgroundWorker has finished
		/// </summary>
		private void RestoreFormFromWork ()
		{
			// Enable controls 
			dgvAddServices.Enabled = true;
			btnCheckExisting.Enabled = true;
			btnUpdate.Enabled = true;
			gbxUpdate.Visible = true;
			menuStrip1.Enabled = true;

			// Disable the Cancel button and progress bar
			gbxProgress.Enabled = false;
			btnCancel.Enabled = false;
			gbxProgress.Visible = false;

			// Reset progress bar
			prgMain.Value = 0;
			gbxProgress.Text = "Ready";

			this.Cursor = Cursors.Default;
		}

		/// <summary>
		/// Searches for WaterOneFlow services registered at an HydroServer, and adds info about those services to the data grid view
		/// </summary>
		/// <param name="portalUtils">Object that contains a pointer to the HydroServer chosen by the user</param>
		/// <param name="e">Parameters from the BackgroundWorker</param>
		/// <returns>Parameters (task type, output message, rows to add to view) to be processed by a BackgroundWorker event handler</returns>
		private object[] AddServicesFromHydroServer ( HydroServerClient hydroServerClient, DoWorkEventArgs e )
		{
			// Build parameters to pass to the background worker
			object[] parameters = new object[3];
			parameters[0] = BackgroundWorkerTasks.AddServicesFromHydroServer;
			parameters[1] = "Operation cancelled";

			// Get all items registered with the server
			bgwMain.ReportProgress ( 0, "Getting list of registered services..." );
			List<DataServiceInfo> services = (List<DataServiceInfo>)hydroServerClient.GetWaterOneFlowServices ();

			// Read the items into rows
			int totalSteps = services.Count;
			int currentStep = 0;

			List<string[]> rowsToAdd = new List<string[]> ();

			foreach ( DataServiceInfo serviceInfo in services )
			{
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return parameters;
				}

				currentStep++;
				bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Reading service info: " + currentStep + " of " + totalSteps + "..." );

				// Create an item to add to the data grid view
				rowsToAdd.Add ( CreateServiceRow ( serviceInfo ) );
			}

			// Prepare a message to the user
			string message = "";
			int servicesAdded = rowsToAdd.Count;
			if ( servicesAdded == 0 )
			{
				message = "No services found in HydroServer";
			}
			else if ( servicesAdded == 1 )
			{
				message = "1 service found in HydroServer";
			}
			else
			{
				message = servicesAdded.ToString () + " services found in HydroServer";
			}

			parameters[1] = message;
			parameters[2] = rowsToAdd;

			return parameters;
		}

		/// <summary>
		/// Searches for WaterOneFlow services registered at a HydroPortal, and adds info about those services to the data grid view
		/// </summary>
		/// <param name="portalUtils">Object that contains a pointer to the HydroPortal chosen by the user</param>
		/// <param name="e">Parameters from the BackgroundWorker</param>
		/// <returns>Parameters (task type, output message, rows to add to view) to be processed by a BackgroundWorker event handler</returns>
		private object[] AddServicesFromHydroPortal ( HydroPortalUtils portalUtils, DoWorkEventArgs e )
		{
			// Build parameters to pass to the background worker
			object[] parameters = new object[3];
			parameters[0] = BackgroundWorkerTasks.AddServicesFromHydroPortal;
			parameters[1] = "Operation cancelled";

			// Get all items registered with the portal
			bgwMain.ReportProgress ( 0, "Getting list of registered portal items..." );
			List<string> itemIds = portalUtils.GetRegisteredItemIds ();

			// Find items with an element providing the URL to a WaterOneFlow service
			int totalSteps = itemIds.Count;
			int currentStep = 0;

			List<string[]> rowsToAdd = new List<string[]> ();

			foreach ( string itemId in itemIds )
			{
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return parameters;
				}

				currentStep++;
				bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Searching registered items for WaterOneFlow services: " + currentStep + " of " + totalSteps + "..." );

				DataServiceInfo serviceInfo = portalUtils.ReadWaterOneFlowServiceInfo ( itemId );
				if ( serviceInfo != null && serviceInfo.EndpointURL != null && serviceInfo.EndpointURL != String.Empty )
				{
					// Create an item to add to the data grid view
					rowsToAdd.Add ( CreateServiceRow ( serviceInfo ) );
				}
			}

			// Prepare a message to the user
			string message = "";
			int servicesAdded = rowsToAdd.Count;
			if ( servicesAdded == 0 )
			{
				message = "No services found in portal";
			}
			else if ( servicesAdded == 1 )
			{
				message = "1 service found in portal";
			}
			else
			{
				message = servicesAdded.ToString () + " services found in portal";
			}

			parameters[1] = message;
			parameters[2] = rowsToAdd;

			return parameters;
		}

		/// <summary>
		/// Searches for WaterOneFlow service info in a DataTable, and adds info about those services to the data grid view
		/// </summary>
		/// <param name="pathToFile">Path to CSV file</param>
		/// <param name="e">Parameters from the BackgroundWorker</param>
		/// <returns>Parameters (task type, output message, rows to add to view) to be processed by a BackgroundWorker event handler</returns>
		private object[] AddServicesFromDataTable ( string pathToFile, DoWorkEventArgs e )
		{
			// Build parameters to pass to the background worker
			object[] parameters = new object[3];
			parameters[0] = BackgroundWorkerTasks.AddServicesFromFile;
			parameters[1] = "Operation cancelled";

			// Read the CSV file into a DataTable
			bgwMain.ReportProgress ( 0, "Opening file, please wait..." );

			DataTable dataTable = null;

			try
			{
				dataTable = CsvFileParser.ParseFileToDataTable ( pathToFile, true );
			}
			catch ( Exception ex )
			{
				throw new Exception ( "Could not read file. " + ex.Message );
			}

			// Get the columns from the table
			int colTitle = dataTable.Columns.IndexOf ( "Title" );
			int colUrl = dataTable.Columns.IndexOf ( "URL" );
			int colCode = dataTable.Columns.IndexOf ( "Code" );
			int colCitation = dataTable.Columns.IndexOf ( "Citation" );
			int colAbstract = dataTable.Columns.IndexOf ( "Abstract" );
			int colWebsite = dataTable.Columns.IndexOf ( "Website" );
			int colName = dataTable.Columns.IndexOf ( "Contact" );
			int colEmail = dataTable.Columns.IndexOf ( "Email" );

			if ( colUrl == -1 )
			{
				throw new Exception ( "URL column not found in CSV file" );
			}

			// Find items with an element providing the URL to a WaterOneFlow service
			int totalSteps = dataTable.Rows.Count;
			int currentStep = 0;

			List<string[]> rowsToAdd = new List<string[]> ();

			foreach ( DataRow dataRow in dataTable.Rows )
			{
				// Update progress
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return parameters;
				}

				currentStep++;
				bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Searching for WaterOneFlow service info: " + currentStep + " of " + totalSteps + "..." );

				// Build service info from the row
				string title = "";
				if ( colTitle != -1 )
				{
					title = dataRow[colTitle].ToString ();
				}
				string url = "";
				if ( colUrl != -1 )
				{
					url = dataRow[colUrl].ToString ();
					try
					{
						url = WebOperations.GetCanonicalUri ( url, true );
					}
					catch
					{
						url = "";
					}
				}
				string code = "";
				if ( colCode != -1 )
				{
					code = dataRow[colCode].ToString ();
				}
				string citation = "";
				if ( colCitation != -1 )
				{
					citation = dataRow[colCitation].ToString ();
				}
				string serviceAbstract = "";
				if ( colAbstract != -1 )
				{
					serviceAbstract = dataRow[colAbstract].ToString ();
				}
				string website = "";
				if ( colWebsite != -1 )
				{
					website = dataRow[colWebsite].ToString ();
				}
				string name = "";
				if ( colName != -1 )
				{
					name = dataRow[colName].ToString ();
				}
				string email = "";
				if ( colEmail != -1 )
				{
					email = dataRow[colEmail].ToString ();
				}

				// Create an item to add to the data grid view
				if ( url != String.Empty )
				{
					string[] row = { title, url, code, citation, serviceAbstract, website, name, email };

					rowsToAdd.Add ( row );
				}
			}

			// Prepare a message to the user
			string message = "";
			int servicesAdded = rowsToAdd.Count;
			if ( servicesAdded == 0 )
			{
				message = "No services found in file";
			}
			else if ( servicesAdded == 1 )
			{
				message = "1 service found in file";
			}
			else
			{
				message = servicesAdded.ToString () + " services found in file";
			}

			parameters[1] = message;
			parameters[2] = rowsToAdd;

			return parameters;
		}

		/// <summary>
		/// Finds rows for services in the view for which a record already exists in the metadata cache database with the same URL
		/// </summary>
		/// <param name="rows">List of DataServiceInfo objects whose URLs we want to check (the service URL is assumed to have come from a row in an object such as a data grid view)</param>
		/// <param name="e">Parameters from the BackgroundWorker</param>
		/// <returns>Parameters (task type, output message, rows for which a record already exists in the metadata cache database with the same URL) to be processed by a BackgroundWorker event handler</returns>
        private object[] CheckForServicesInCache(List<DataServiceInfo> rows, DoWorkEventArgs e)
		{
			// Build parameters to pass to the background worker
			object[] parameters = new object[3];
			parameters[0] = BackgroundWorkerTasks.CheckForExisingServices;
			parameters[1] = "Operation cancelled";

			List<int> rowsToSelect = new List<int> ();

			// Get a list of existing URLs from the metadata cache databsae
			if ( e != null )
			{
				bgwMain.ReportProgress ( 0, "Getting list of existing WaterOneFlow services from database..." );
			}

			List<string> existingUrls = DatabaseOperations.GetCacheServiceUrls ( true );

			// Check all items in the view
			int totalSteps = rows.Count;
			int currentStep = 0;
			

			IEqualityComparer<string> comparer = new CaseInsensitiveEqualityComparer ();
           
            for (int i = 0, len=rows.Count; i < len; i++)
            {
                DataServiceInfo row = rows[i];
			
				if ( e != null )
				{
					if ( bgwMain.CancellationPending )
					{
						e.Cancel = true;
						return parameters;
					}

					currentStep++;
					bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Checking service " + currentStep + " of " + totalSteps + "..." );
				}

				string serviceUrl = row.EndpointURL;
				

				if ( existingUrls.Contains ( serviceUrl, comparer ) == true )
				{
					rowsToSelect.Add(i);
				}
			}

			// Prepare a message to the user
            // TODO: This message should be created outside of this service, and instead should be done in the display logic
			string message = "";
            int existingCount = rowsToSelect.Count;

            if ( existingCount == 0 )
			{
				message = "No services with the same URL were found in the metadata cache database";
			}
            else if (existingCount == 1)
            {
                message = "1 service has a URL that already exists in the metadata cache database. This service has been selected.";
            }
            else
            {
                message = existingCount.ToString() + " services have URLs that already exist in the metadata cache database. These services have been selected.";
            }

			parameters[1] = message;
			parameters[2] = rowsToSelect;
			return parameters;
		}

		/// <summary>
		/// For each service described in the data grid view, adds records to the metadata cache database to describe the service
		/// </summary>
		/// <param name="servicesToAdd">List of services to add to the metadata cache database</param>
		/// <param name="e">Parameters from the BackgroundWorker</param>
		/// <returns>Parameters (task type, output message, rows that were successfully added) to be processed by a BackgroundWorker event handler</returns>
		private object[] AddServicesToDatabase ( List<DataServiceInfo> servicesToAdd, DoWorkEventArgs e )
		{
			// Build parameters to pass to the background worker
			object[] parameters = new object[3];
			parameters[0] = BackgroundWorkerTasks.UpdateDatabase;
			parameters[1] = "Operation cancelled";

			List<int> rowsToSelect = new List<int> ();

			bgwMain.ReportProgress ( 0, "Getting list of existing WaterOneFlow services from database..." );

			// Get a list of existing URLs from the metadata cache databsae
			List<string> existingUrls = DatabaseOperations.GetCacheServiceUrls ( true );

			// Add service records to the database
			int totalSteps = servicesToAdd.Count;
			int currentStep = 0;
			int countAlreadyExists = 0;
			int countInvalidService = 0;

			IEqualityComparer<string> comparer = new CaseInsensitiveEqualityComparer ();

			MetadataCacheManagerSQL cacheManager = DatabaseOperations.GetCacheManager();

			for ( int i = 0; i < servicesToAdd.Count; i++ )
			{
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return parameters;
				}

				currentStep++;
				bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Checking service " + currentStep + " of " + totalSteps + "..." );

				DataServiceInfo serviceInfo = servicesToAdd[i];

				// Check if the service already exists in the database
				if ( existingUrls.Contains ( serviceInfo.EndpointURL, comparer ) == true )
				{
					countAlreadyExists += 1;
					continue;
				}

				// Check that the URL is for a live service
				if ( this.mnuCheckForValidService.Checked == true )
				{
					// Attempt to create a WaterOneFlowServiceClient from the URL.  If the URL is not for a WaterOneFlow service, an error is thrown in the constructor.
					try
					{
						WaterOneFlowClient waterOneFlowClient = new WaterOneFlowClient ( serviceInfo.EndpointURL );
						DataServiceInfo clientServiceInfo = waterOneFlowClient.ServiceInfo;

						serviceInfo.ServiceName = clientServiceInfo.ServiceName;
						serviceInfo.Protocol = clientServiceInfo.Protocol;
						serviceInfo.ServiceType = clientServiceInfo.ServiceType;
						serviceInfo.Version = clientServiceInfo.Version;
					}
					catch
					{
						countInvalidService += 1;
						continue;
					}

				}

				// Save the service
				cacheManager.SaveDataService ( serviceInfo );

				existingUrls.Add ( serviceInfo.EndpointURL );

				rowsToSelect.Add ( i );

			}

			// Prepare a message to the user
			string message = "";

			int serviceCount = rowsToSelect.Count;
			if ( serviceCount == 0 )
			{
				message += "No new services added to metadata cache database.\n\n ";
			}
			else if ( serviceCount == 1 )
			{
				message += "1 new service added to metadata cache database.\n\n";
			}
			else
			{
				message += serviceCount.ToString () + " new services added to metadata cache database.\n\n";
			}

			if ( countAlreadyExists == 1 )
			{
				message += "1 service was not added because it already exists in the database.\n\n";
			}
			else if ( countAlreadyExists > 1 )
			{
				message += countAlreadyExists.ToString () + " services were not added because they already exist in the database.\n\n";
			}

			if ( countInvalidService == 1 )
			{
				message += "1 service was not added because it does not point to a valid WaterOneFlow service.\n\n";
			}
			else if ( countInvalidService > 1 )
			{
				message += countInvalidService.ToString () + " services were not added because they do not point to a valid WaterOneFlow service.\n\n";
			}

			if ( serviceCount == 1 )
			{
				message += "Remember to download metadata for this service in the Metadata Fetcher window.";
			}
			else if ( serviceCount > 1 )
			{
				message += "Remember to download metadata for these services in the Metadata Fetcher window.";
			}

			parameters[1] = message;
			parameters[2] = rowsToSelect;
			return parameters;
		}

		#endregion

		#region UI Events

		/// <summary>
		/// Cancels or closes the form if the user presses the ESC key
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="keyData"></param>
		/// <returns>true if key press was handled, false otherwise</returns>
		protected override bool ProcessCmdKey ( ref System.Windows.Forms.Message msg, System.Windows.Forms.Keys keyData )
		{
			try
			{
				if ( msg.WParam.ToInt32 () == (int)Keys.Escape )
				{
					if ( bgwMain.IsBusy )
					{
						CancelWorker ();
					}
					else
					{
						this.DialogResult = DialogResult.Cancel;
						this.Close ();
					}

					return true;
				}
				else
				{
					return base.ProcessCmdKey ( ref msg, keyData );
				}
			}
			catch ( Exception Ex )
			{
				MessageBox.Show ( "Key Overrided Events Error:" + Ex.Message );
			}

			return base.ProcessCmdKey ( ref msg, keyData );
		}

		private void fromHydroPortalToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			AddServiceFromHydroPortalForm frmAddFromPortal = new AddServiceFromHydroPortalForm ();

			if ( frmAddFromPortal.ShowDialog ( this ) == DialogResult.OK )
			{
				if ( bgwMain.IsBusy )
				{
					MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
					return;
				}

				HydroPortalUtils portalUtils = frmAddFromPortal.HydroPortalConnection;

				// Build parameters to pass to the background worker
				object[] parameters = new object[2];
				parameters[0] = BackgroundWorkerTasks.AddServicesFromHydroPortal;
				parameters[1] = portalUtils;

				// Set form controls for doing work
				dgvAddServices.ClearSelection ();
				SetupFormForWork ( true );

				// Start the asynchronous operation
				bgwMain.RunWorkerAsync ( parameters );
			}
		}

		private void fromHydroServerToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			AddServiceFromHydroServerForm frmAddFromHisServer = new AddServiceFromHydroServerForm ();

			if ( frmAddFromHisServer.ShowDialog ( this ) == DialogResult.OK )
			{
				if ( bgwMain.IsBusy )
				{
					MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
					return;
				}

                tcAddService.SelectedTab = tpAddMultiSvcs; //switch to the multi service tab
                
                HydroServerClient hydroServerClient = frmAddFromHisServer.HisServerConnection;

				// Build parameters to pass to the background worker
				object[] parameters = new object[2];
				parameters[0] = BackgroundWorkerTasks.AddServicesFromHydroServer;
				parameters[1] = hydroServerClient;

				// Set form controls for doing work
				dgvAddServices.ClearSelection ();
				SetupFormForWork ( true );

				// Start the asynchronous operation
				bgwMain.RunWorkerAsync ( parameters );
			}
		}

		private void fromFileToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			if ( openFileDialog1.ShowDialog () == DialogResult.OK )
			{
				if ( bgwMain.IsBusy )
				{
					MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
					return;
				}

                tcAddService.SelectedTab = tpAddMultiSvcs; //switch to the multi service tab

                string path = openFileDialog1.FileName;

                // Build parameters to pass to the background worker
				object[] parameters = new object[2];
				parameters[0] = BackgroundWorkerTasks.AddServicesFromFile;
				parameters[1] = path;

				// Set form controls for doing work
				dgvAddServices.ClearSelection ();
				SetupFormForWork ( true );

				// Start the asynchronous operation
				bgwMain.RunWorkerAsync ( parameters );
			}
		}

		private void btnCancel_Click ( object sender, EventArgs e )
		{
			CancelWorker ();
		}

        /// <summary>
        /// Updates the Metadata Cache when the Update button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void btnUpdate_Click ( object sender, EventArgs e )
		{
			if ( bgwMain.IsBusy )
			{
				MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.",
								  this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}


            // Build a list of services to add
            List<DataServiceInfo> services = new List<DataServiceInfo>();

            if (tcAddService.SelectedTab.Equals(tpAddSingleSvc))
            {
                services = GetSingleServiceRow();
            }
            else //Add Multiple
            {

                if (!ValidateDgvAddServices())
                    return;

                services = GetMultipleServiceRows();
            }


			// Build parameters to pass to the background worker
			object[] parameters = new object[2];
			parameters[0] = BackgroundWorkerTasks.UpdateDatabase;
			parameters[1] = services;

			// Set form controls for doing work
			dgvAddServices.ClearSelection ();
			SetupFormForWork ( true );

			// Start the asynchronous operation
			bgwMain.RunWorkerAsync ( parameters );
		}

        private bool ValidateDgvAddServices()
        {
            if (dgvAddServices.Rows.Count == 1 && dgvAddServices.Rows[0].IsNewRow == true)
            {
                MessageBox.Show("Please add one or more services to the list.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }

            foreach (DataGridViewRow serviceRow in dgvAddServices.Rows)
            {
                if (serviceRow.IsNewRow == false)
                {
                 
                    // Service Title
                    object cellValue = serviceRow.Cells[0].Value;
                    if ((cellValue == null) || (cellValue.ToString().Trim() == String.Empty))
                    {
                        MessageBox.Show("Please provide a service title for row " + serviceRow.Index.ToString(),
                                          this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                   
                    // Service URL
                    cellValue = serviceRow.Cells[1].Value;
                    if ((cellValue == null) || (cellValue.ToString().Trim() == String.Empty))
                    {
                        MessageBox.Show("Please provide a service URL for row " + serviceRow.Index.ToString(),
                                          this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                    // Trim the query off of the URL
                    string serviceUrl = cellValue.ToString().Trim();
                    try
                    {
                        serviceUrl = WebOperations.GetCanonicalUri(serviceUrl, true);
                    }
                    catch
                    {
                        MessageBox.Show("Please provide a service URL for row " + serviceRow.Index.ToString(),
                                          this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                }
            }

            return true;
        }

        
		private void btnCheckExisting_Click ( object sender, EventArgs e )
		{
		
            if ( bgwMain.IsBusy )
			{
				MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

            List<DataServiceInfo> rows = null;
           
            if (tcAddService.SelectedTab.Equals(tpAddSingleSvc))
            {
                rows = GetSingleServiceRow();
            }
            else //tpAddMultiSvcs is selected
            {
                if (!ValidateDgvAddServices())
                    return;

                rows = GetMultipleServiceRows();     
            }

            if (rows == null)
                return;

            // Build parameters to pass to the background worker
            object[] parameters = new object[2];
            parameters[0] = BackgroundWorkerTasks.CheckForExisingServices;
            parameters[1] = rows;

            // Set form controls for doing work
            dgvAddServices.ClearSelection();
            SetupFormForWork(false);

            parameters = CheckForServicesInCache(rows, null);

            // Select rows that were already in the cache
            string message = parameters[1] as string;

            if (tcAddService.SelectedTab.Equals(tpAddMultiSvcs)) //if on the Add Multiple Services tab
            {
                List<int> rowsToSelect = parameters[2] as List<int>;
                foreach (int row in rowsToSelect)
                {
                    dgvAddServices.Rows[row].Selected = true;
                }
            }
            

            MessageBox.Show(message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Enable controls that were disabled until the asynchronous operation was finished
            RestoreFormFromWork();

            this.ActiveControl = btnCheckExisting;
		}

        /// <summary>
        /// Adds Multiple Services from the DataGrid input of this form
        /// </summary>
        private List<DataServiceInfo> GetMultipleServiceRows()
        {
            // Build a list of URLs to check
            List<DataServiceInfo> rows = new List<DataServiceInfo>();

            foreach (DataGridViewRow serviceRow in dgvAddServices.Rows)
            {
                if (serviceRow.IsNewRow == false)
                {
                    object cellValue = serviceRow.Cells[1].Value;
                    if (cellValue != null)
                    {
                        string serviceUrl = cellValue.ToString().Trim();

                        // Trim the query off of the URL
                        int index = serviceUrl.IndexOf("?");
                        if (index > -1)
                        {
                            serviceUrl = serviceUrl.Substring(0, index);
                        }

                        DataServiceInfo dataServiceInfo = new DataServiceInfo();
                        dataServiceInfo.EndpointURL = serviceUrl;
                        dataServiceInfo.ServiceTitle = serviceRow.Cells[0].Value.ToString().Trim();

                        if (serviceRow.Cells[2].Value != null)
                            dataServiceInfo.ServiceCode = serviceRow.Cells[2].Value.ToString().Trim();

                        if (serviceRow.Cells[3].Value != null)
                            dataServiceInfo.Citation = serviceRow.Cells[3].Value.ToString().Trim();

                        if (serviceRow.Cells[4].Value != null)
                            dataServiceInfo.Abstract = serviceRow.Cells[4].Value.ToString().Trim();

                        if (serviceRow.Cells[5].Value != null)
                            dataServiceInfo.DescriptionURL = serviceRow.Cells[5].Value.ToString().Trim();

                        if (serviceRow.Cells[6].Value != null)
                            dataServiceInfo.ContactName = serviceRow.Cells[6].Value.ToString().Trim();

                        if (serviceRow.Cells[7].Value != null)
                            dataServiceInfo.ContactEmail = serviceRow.Cells[7].Value.ToString().Trim();
                        
                        rows.Add(dataServiceInfo);
                    }
                }
            }

            return rows;
        }

        private void txtTitle_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txtTitle.Text))
            {
                errorProvider1.SetError(txtTitle, "Please enter a title");
            }
            else
            {
                errorProvider1.SetError(txtTitle, "");
            }

            CheckFields();
        }

        private void txtURL_Validating(object sender, CancelEventArgs e)
        {
           
            if (String.IsNullOrEmpty(txtURL.Text))
            {
                errorProvider1.SetError(txtURL, "Please enter a valid URL");
                return;
            }
            
            if (!txtURL.Text.StartsWith("http://") && !txtURL.Text.StartsWith("https://"))
            {
                txtURL.Text = "http://" + txtURL.Text; //add http:// to the beginning
            }

            //check that the URL is Valid -- this is quite slow. Perhaps a REGEX would be better.
            if (!WebOperations.IsUrlFormatValid(txtURL.Text))
            {
                errorProvider1.SetError(txtURL, "Please enter a valid URL");
            }
            else
            {
                errorProvider1.SetError(txtURL, "");
            }

            CheckFields();
        }

        private void tcAddService_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tcAddService.SelectedTab.Equals(tpAddSingleSvc)) //Single Service tab
            {
                txtURL.CausesValidation = true;
                txtTitle.CausesValidation = true;

                CheckFields();
            }
            else //on the Multiple Service tab
            {
                txtURL.CausesValidation = false;
                txtTitle.CausesValidation = false;

                btnCheckExisting.Enabled = true;
                btnUpdate.Enabled = true;
            }
        }

        private void CheckFields()
        {
            if (errorProvider1.GetError(txtTitle).Equals("")
                && errorProvider1.GetError(txtURL).Equals("")
                && !String.IsNullOrEmpty(txtTitle.Text)
                && !String.IsNullOrEmpty(txtURL.Text))
            {
                btnCheckExisting.Enabled = true;
                btnUpdate.Enabled = true;
            }
            else
            {
                btnCheckExisting.Enabled = false;
                btnUpdate.Enabled = false;
            }
        }

     

        /// <summary>
        /// Adds single service from the form input
        /// </summary>
        private List<DataServiceInfo> GetSingleServiceRow()
        {

            List<DataServiceInfo> rows = new List<DataServiceInfo>();

            string serviceUrl = txtURL.Text;

            // Trim the query off of the URL
            int index = serviceUrl.IndexOf("?");
            if (index > -1)
            {
                serviceUrl = serviceUrl.Substring(0, index);
            }

            DataServiceInfo dataServiceInfo = new DataServiceInfo();
            dataServiceInfo.EndpointURL = serviceUrl;
            dataServiceInfo.ServiceTitle = txtTitle.Text;
            dataServiceInfo.ServiceCode = txtCode.Text;
            dataServiceInfo.Citation = txtCitation.Text;
            dataServiceInfo.Abstract = txtAbstract.Text;
            dataServiceInfo.DescriptionURL = txtWebsite.Text;
            dataServiceInfo.ContactName = txtContact.Text;
            dataServiceInfo.ContactEmail = txtEmail.Text;


            rows.Add(dataServiceInfo);

            return rows;
        }

		#endregion

		#region Background Worker

		private void CancelWorker ()
		{
			// Cancel the asynchronous operation
			bgwMain.CancelAsync ();

			// Disable the Cancel button
			btnCancel.Enabled = false;

			gbxProgress.Text = "Cancelling...";
		}

		private void bgwMain_DoWork ( object sender, DoWorkEventArgs e )
		{
			object[] parameters = e.Argument as object[];
			BackgroundWorkerTasks task = (BackgroundWorkerTasks)parameters[0];
			if ( task == BackgroundWorkerTasks.AddServicesFromHydroPortal )
			{
				HydroPortalUtils portalUtils = parameters[1] as HydroPortalUtils;
				e.Result = AddServicesFromHydroPortal ( portalUtils, e );
			}
			else if ( task == BackgroundWorkerTasks.AddServicesFromFile )
			{
				string path = parameters[1] as string;
				e.Result = AddServicesFromDataTable ( path, e );
			}
			else if ( task == BackgroundWorkerTasks.CheckForExisingServices )
			{
                List<DataServiceInfo> rows = parameters[1] as List<DataServiceInfo>;
				e.Result = CheckForServicesInCache(rows, e);
			}
			else if ( task == BackgroundWorkerTasks.UpdateDatabase )
			{
				List<DataServiceInfo> services = parameters[1] as List<DataServiceInfo>;
				e.Result = AddServicesToDatabase ( services, e );
			}
			else if ( task == BackgroundWorkerTasks.AddServicesFromHydroServer )
			{
				HydroServerClient hydroServerClient = parameters[1] as HydroServerClient;
				e.Result = AddServicesFromHydroServer ( hydroServerClient, e );
			}
			else
			{
				// Build parameters to pass to the background worker
				object[] outParameters = new object[2];
				outParameters[0] = BackgroundWorkerTasks.Unknown;
				outParameters[1] = "Unknown task provided to background worker";

				e.Result = outParameters;
			}
		}

		private void bgwMain_ProgressChanged ( object sender, ProgressChangedEventArgs e )
		{
			prgMain.Value = e.ProgressPercentage;
			gbxProgress.Text = e.UserState.ToString ();
		}

		private void bgwMain_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e )
		{
			this.Cursor = Cursors.Default;

			BackgroundWorkerTasks task = BackgroundWorkerTasks.Unknown;

			// Report the result
			if ( e.Error != null )
			{
				MessageBox.Show ( e.Error.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else if ( e.Cancelled )
			{
				if ( this.Visible == true && _formIsClosing == false )
				{
					MessageBox.Show ( "Operation cancelled", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
				}
			}
			else
			{
				string message = "";

				object[] parameters = e.Result as object[];

				if ( (parameters != null) && (parameters.Length > 1) )
				{
					message = parameters[1] as string;

					task = (BackgroundWorkerTasks)parameters[0];
					if ( task == BackgroundWorkerTasks.AddServicesFromHydroPortal ||
						 task == BackgroundWorkerTasks.AddServicesFromFile ||
						 task == BackgroundWorkerTasks.AddServicesFromHydroServer )
					{
						dgvAddServices.Enabled = true;

						List<string[]> rowsToAdd = parameters[2] as List<string[]>;
						foreach ( string[] row in rowsToAdd )
						{
                            int rowIndex = dgvAddServices.Rows.Add(row);
                            SelectDataGridViewRow(dgvAddServices, rowIndex);
							//dgvAddServices.Rows[dgvAddServices.Rows.Add ( row )].Cells["Selected"].Value = true;
						}

						this.ActiveControl = btnUpdate;
					}
					else if ( task == BackgroundWorkerTasks.CheckForExisingServices )
					{
						List<int> rowsToSelect = parameters[2] as List<int>;
						foreach ( int row in rowsToSelect )
						{
							dgvAddServices.Rows[row].Cells["Selected"].Value = true;
						}

						this.ActiveControl = btnCheckExisting;
					}
					else if ( task == BackgroundWorkerTasks.UpdateDatabase )
					{
						this.ActiveControl = btnUpdate;
					}

				}

				MessageBox.Show ( message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
			}

			// Enable controls that were disabled until the asynchronous operation was finished
			RestoreFormFromWork ();

			if ( _formIsClosing )
			{
				_formIsClosing = false;
                DialogResult = DialogResult.Cancel;
			}
			else if ( task == BackgroundWorkerTasks.UpdateDatabase )
			{
				DialogResult = DialogResult.OK;
			}
		}

		#endregion

		#endregion

        #region Public Members

        public void ClearInputs()
        {
            if (bgwMain.IsBusy == false)
            {
                dgvAddServices.Rows.Clear();
            }
        }

        #endregion

	}
}
