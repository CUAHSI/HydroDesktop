using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.MetadataFetcher.Forms
{
	public partial class MainForm : Form
	{
		#region Variables

		private enum BackgroundWorkerTasks { FetchMetadata, RemoveServices, Unknown }

		private AddServicesForm _addServicesForm;
		private bool _formIsClosing;

		#endregion

		#region Constructor

		public MainForm ()
		{
			InitializeComponent ();

			FormClosing += MainForm_FormClosing;

			// Background worker for data download
			bgwMain = new BackgroundWorker ();
			bgwMain.DoWork += bgwMain_DoWork;
			bgwMain.ProgressChanged += bgwMain_ProgressChanged;
			bgwMain.RunWorkerCompleted += bgwMain_RunWorkerCompleted;
			bgwMain.WorkerReportsProgress = true;
			bgwMain.WorkerSupportsCancellation = true;
		}

		#endregion

		#region Private Methods

		#region Form Loading / Unloading

		private void MainForm_Load ( object sender, EventArgs e )
		{
			Cursor = Cursors.WaitCursor;
			RefreshServiceList ();
			Cursor = Cursors.Default;

			mnuRemoveSelectedServices.Enabled = false;
		}

		void MainForm_FormClosing ( object sender, FormClosingEventArgs e )
		{
			e.Cancel = true;

			if ( bgwMain.IsBusy )
			{
				CancelWorker ();

				_formIsClosing = true;
			}
			else
			{
				Hide ();
			}
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Opens a form to allow a user to enter details for a new service to add to the metadata cache list
		/// </summary>
		private void AddServices ()
		{
			// Initialize the form
			if ( _addServicesForm == null )
			{
				_addServicesForm = new AddServicesForm ();
			}
			_addServicesForm.ClearInputs ();

			// Show the form
		    _addServicesForm.ShowDialog(this);
			SelectNewServices ();
		}

		private void RemoveSelectedServicesPrep ()
		{
			if ( dgvServices.Rows.Count == 0 )
			{
				MessageBox.Show ( "There are no services to remove", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Build a list of services that the user marked for removal
			var checkedServices = new List<string> ();

			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				var selected = (bool)serviceRow.Cells["Selected"].Value;
				if ( selected )
				{
					var serviceUrl = serviceRow.Cells["Service URL"].Value.ToString ();
					checkedServices.Add ( serviceUrl );
				}
			}

			if ( checkedServices.Count == 0 )
			{
				MessageBox.Show ( "Please select services to remove and try again", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			if ( MessageBox.Show ( "Are you sure you want to DELETE the selected services from your local cache database?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{
				// Build parameters to pass to the background worker
				var parameters = new object[2];
				parameters[0] = BackgroundWorkerTasks.RemoveServices;
				parameters[1] = checkedServices;

				SetupFormForWork ();

				// Start the asynchronous operation
				bgwMain.RunWorkerAsync ( parameters );
			}
		}

		/// <summary>
		/// Services selected in the DataGridView are removed from the view and also the Metadata Cache database
		/// </summary>
		private string RemoveSelectedServices ( List<string> serviceUrls, DoWorkEventArgs e )
		{
			var totalServices = serviceUrls.Count;
			var currentService = 0;

            var cacheManager = DatabaseOperations.GetCacheManager();

			foreach ( var serviceUrl in serviceUrls )
			{
				// Update progress
				currentService++;
				bgwMain.ReportProgress ( 100 * currentService / totalServices, "Removing service " + currentService + " of " + totalServices + "..." );

				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Delete the service info for this item
				var serviceInfo = DatabaseOperations.GetDataServiceFromCache ( serviceUrl );

				try
				{
					cacheManager.DeleteRecordsForService ( serviceInfo, true );
				}
				catch ( Exception ex )
				{
					throw new Exception ( "Unable to remove service from local metadata database. \n" + serviceUrl + "\n" + ex.Message );
				}
			}


		    return totalServices == 1 ? "Service successfully removed" : "Services successfully removed";
		}

		/// <summary>
		/// Refreshes the list of services in the DataGridView, and selects new ones
		/// </summary>
		private void SelectNewServices ()
		{
			// Keep track of existing services
			var existingUris = new List<string> ();
			var checkedUris = new List<string> ();

			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				var serviceUrl = serviceRow.Cells["Service URL"].Value.ToString ();
				existingUris.Add ( serviceUrl );

				if ( Convert.ToBoolean(serviceRow.Cells["Selected"].Value) )
				{
					checkedUris.Add ( serviceUrl );
				}
			}

			var previousServiceCount = dgvServices.Rows.Count;

			// Refresh the service list
			RefreshServiceList ();

			if ( dgvServices.Rows.Count > previousServiceCount )
			{
				// New services added, so select any services that weren't previously in the list
				foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
				{
				    var serviceUri = serviceRow.Cells["Service URL"].Value.ToString();
				    serviceRow.Cells["Selected"].Value = !existingUris.Contains(serviceUri);
				}
			}
			else
			{
				// No new services added, so select any existing services that the user had previously selected
				foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
				{
					var serviceUri = serviceRow.Cells["Service URL"].Value.ToString ();
				    serviceRow.Cells["Selected"].Value = checkedUris.Contains(serviceUri);
				}
			}
		}

		/// <summary>
		/// Updates the label that shows the number of selected services and colors selected rows.
		/// </summary>
		private void updateSelection ()
		{
			var count = 0;
			mnuRemoveSelectedServices.Enabled = false;
		    foreach (DataGridViewRow row in dgvServices.Rows)
		    {
		        if ((bool) row.Cells["Selected"].Value)
		        {
		            count++;
		            mnuRemoveSelectedServices.Enabled = true;
		            foreach (DataGridViewCell cell in row.Cells)
		                cell.Style.BackColor = Color.LightYellow;
		        }
		        else
		        {
		            foreach (DataGridViewCell cell in row.Cells)
		                cell.Style.BackColor = Color.Empty;
		        }
		    }

		    lblSelectionCount.Text = count.ToString (CultureInfo.InvariantCulture);
		}

	    private string FetchMetadata (List<string> serviceUrls, DoWorkEventArgs e )
		{
			var totalServices = serviceUrls.Count;
			var currentService = 0;
			var seriesCount = 0; // Keeps track of how many series were successfully processed

			var cacheManager = DatabaseOperations.GetCacheManager();

			var errors = new StringBuilder ();  // Keep track of errors and report them at the end
	        foreach ( var serviceUrl in serviceUrls )
			{
				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Update progress
				currentService++;
				bgwMain.ReportProgress ( 100 * (currentService-1) / totalServices, "Reading database info for service " + currentService + " of " + totalServices + "..." );

				// Get the service info for this item
				var serviceInfo = DatabaseOperations.GetDataServiceFromCache ( serviceUrl );

				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Update progress
				bgwMain.ReportProgress ( 100 * (currentService - 1) / totalServices, "Deleting old records for service " + currentService + " of " + totalServices + "..." );

				// Delete existing records for this service
				cacheManager.DeleteRecordsForService ( serviceInfo, false );

				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Update progress
				bgwMain.ReportProgress ( 100 * (currentService - 1) / totalServices, "Connecting to service " + currentService + " of " + totalServices + "..." );

				// Create a WaterOneFlow Service for this URL
				WaterOneFlowClient waterOneFlowClient;
				try
				{
					waterOneFlowClient = new WaterOneFlowClient ( serviceUrl );
				}
				catch ( Exception ex )
				{
					// Flag the error and continue to the next service
					errors.AppendLine ( "Could not connect to service with URL: " + serviceUrl + ".\n" + ex.Message + "\n\n" );
					continue;
				}

				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Update progress
				bgwMain.ReportProgress ( 100 * (currentService - 1) / totalServices, "Updating database description for service " + currentService + " of " + totalServices + "..." );

				// Update service info in the metadata database
				var waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;

				serviceInfo.IsHarvested = false;
				serviceInfo.ServiceName = waterOneFlowServiceInfo.ServiceName;
				serviceInfo.Version = waterOneFlowServiceInfo.Version;
				serviceInfo.ServiceType = waterOneFlowServiceInfo.ServiceType;
				serviceInfo.Protocol = waterOneFlowServiceInfo.Protocol;
                serviceInfo.VariableCount = (cacheManager.GetVariablesByService((int)serviceInfo.Id)).Count;
				cacheManager.UpdateDataRow ( serviceInfo ); 

				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Update progress
				bgwMain.ReportProgress ( 100 * (currentService - 1) / totalServices, "Downloading site list for service " + currentService + " of " + totalServices + "..." );

				// Get all sites for this service
				IList<Site> siteList;

				try
				{
					siteList = waterOneFlowClient.GetSites ();
				}
				catch ( WebException ex )
				{

                    var sr = new StreamReader(ex.Response.GetResponseStream());
                    sr.ReadToEnd();
                    
                    // Flag the error and continue to the next service
					errors.AppendLine ( "Could not get site list from service with URL: " + serviceUrl + ".\n" + ex.Message + "\n\n" );
					continue;
				}

				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				var totalSteps = siteList.Count;
				var currentStep = 0;

				// Error tracking
				var siteErrorCount = 0; // Keeps track of how many errors we had while downloading site info for the current service
				var saveErrorCount = 0; // Keeps track of how many errors we had while saving site info for the current service
				var firstSiteError = ""; // Records the message from the first error we had while getting site info for the current service
				var firstSaveError = ""; // Records the message from the first error we had while saving site info for the current service

				// Default extent for the service.  These values are designed to be overwritten as we query sites in the service
				double east = -180;
				double west = 360;
				double north = -90;
				double south = 90;
                int valueCount = 0;
				foreach (var site in siteList)
				{
					// Check for cancel
					if (bgwMain.CancellationPending )
					{
						e.Cancel = true;
						return "Operation cancelled";
					}

					// Update progress
					currentStep++;
					bgwMain.ReportProgress ( 100 * currentStep / totalSteps,
						"Processing site " + currentStep + " of " + totalSteps + 
						" from service " + currentService + " of " + totalServices + "..." );

					// Get series for this site
					IList<SeriesMetadata> currentSeriesList;

                    try
                    {
                        currentSeriesList = waterOneFlowClient.GetSiteInfo(site.Code);
                    }
                    catch (WebException ex)
                    {
                        // Flag the error and continue to the next site
                        siteErrorCount++;

                        if (siteErrorCount == 1)
                        {
                            firstSiteError = ex.Message;
                        }

                        if (ex.Response != null)
                        {
                            var rdr = new StreamReader(ex.Response.GetResponseStream());
                            rdr.ReadToEnd();
                        }

                        continue;
                    }
                    catch (Exception ex)
                    {
                        // Flag the error and continue to the next site
                        siteErrorCount++;

                        if (siteErrorCount == 1)
                        {
                            firstSiteError = ex.Message;
                        }

                        continue;
                    }

					// Update service extent 
					if ( site.Latitude > north )
					{
						north = site.Latitude;
					}
					if ( site.Latitude < south )
					{
						south = site.Latitude;
					}
					if ( site.Longitude > east )
					{
						east = site.Longitude;
					}
					if ( site.Longitude < west )
					{
						west = site.Longitude;
					}

					// Save series info to metadata cache database
					foreach (var series in currentSeriesList )
					{
                        valueCount += series.ValueCount;
						// Check for cancel
						if ( bgwMain.CancellationPending )
						{
							e.Cancel = true;
							return "Operation cancelled";
						}

						try
						{
						    cacheManager.SaveSeries(series, serviceInfo);
						}
						catch ( Exception ex )
						{
							// Flag the error and continue to the next series
							saveErrorCount++;

							if ( saveErrorCount == 1 )
							{
								firstSaveError = ex.Message;
							}

							continue;
						}

						// Keep track of how many series were successfully processed
						seriesCount++;
					}
				}

				// Log errors
				if ( siteErrorCount == 1 )
				{
					errors.AppendLine ( "Could not get site info for 1 site in service with URL: " + serviceUrl + 
						"\n" + firstSiteError + "\n\n" );
				}
				else if ( siteErrorCount > 1 )
				{
					errors.AppendLine ( "Could not get site info for " + siteErrorCount + " sites in service with URL: " + serviceUrl + 
						"\n" + firstSiteError + "\n\n" );
				}

				if ( saveErrorCount == 1 )
				{
					errors.AppendLine ( "Could not save site info for 1 site in service with URL: " + serviceUrl + 
						"\n" + firstSaveError + "\n\n" );
				}
				else if ( saveErrorCount > 1 )
				{
					errors.AppendLine ( "Could not save site info for " + siteErrorCount + " sites in service with URL: " + serviceUrl + 
						"\n" + firstSaveError + "\n\n" );
				}

				// Check for cancel
				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Update progress
				bgwMain.ReportProgress ( 100 * currentService / totalServices, 
					"Updating harvested statistics for service " + currentService + " of " + totalServices + "..." );

				// Update service info
				serviceInfo.IsHarvested = true;
				serviceInfo.HarveDateTime = DateTime.Now;
				serviceInfo.EastLongitude = east;
				serviceInfo.WestLongitude = west;
				serviceInfo.NorthLatitude = north;
				serviceInfo.SouthLatitude = south;
                serviceInfo.SiteCount = siteList.Count;
                serviceInfo.ValueCount = valueCount;
                serviceInfo.VariableCount = (cacheManager.GetVariablesByService((int)serviceInfo.Id)).Count;
			    cacheManager.UpdateDataRow(serviceInfo); // Updates properties like harvest datetime and service extent
			}

			// Check for cancel
			if ( bgwMain.CancellationPending )
			{
				e.Cancel = true;
				return "Operation cancelled";
			}

			// Update progress
			bgwMain.ReportProgress ( 100, "Operation complete" );

			// Report result
			var message = "Download complete. " + seriesCount + " series saved to metadata cache database.";

			if ( errors.Length > 0 )
			{
				message += "\n\nSome errors occurred during the operation:\n\n" + errors;
			}

			return message;
		}

		/// <summary>
		/// Disables/Enables controls and sets mouse cursor in preparation for a BackgroundWorker to run
		/// </summary>
		private void SetupFormForWork ()
		{
			// Disable controls until the asynchronous operation is done

			mnuServiceManagement.Enabled = false;
			chkAll.Enabled = false;
			//btnRemove.Enabled = false;
			//btnAddService.Enabled = false;
			btnUpdate.Enabled = false;
			dgvServices.Enabled = false;
			lblSelectionCount.Visible = false;
			lblServicesSelected.Visible = false;

			// Enable the Cancel button while the asynchronous operation runs
			gbxProgress.Visible = true;
			btnCancelDownload.Enabled = true;

			Cursor = Cursors.WaitCursor;
		}

		/// <summary>
		/// Disables/Enables controls and sets mouse cursor once a BackgroundWorker has finished
		/// </summary>
		private void RestoreFormFromWork ()
		{
			mnuServiceManagement.Enabled = true;
			chkAll.Enabled = true;
			//btnRemove.Enabled = true;
			//btnAddService.Enabled = true;
			btnUpdate.Enabled = true;
			dgvServices.Enabled = true;
			lblServicesSelected.Visible = true;
			lblSelectionCount.Visible = true;

			// Disable the Cancel button 
			btnCancelDownload.Enabled = false;
			gbxProgress.Visible = false;

			// Reset progress bar
			prgMain.Value = 0;
			gbxProgress.Text = "Ready";
		}

		#endregion

		#region UI Events

		private void btnUpdate_Click ( object sender, EventArgs e )
		{
			if ( dgvServices.Rows.Count == 0 )
			{
				MessageBox.Show ( "Please add a WaterOneFlow web service to the list", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			if ( bgwMain.IsBusy )
			{
				MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Build a list of services that the user marked for download
			var checkedServices = new List<string> ();

			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				var selected = (bool)serviceRow.Cells["Selected"].Value;

				if ( selected )
				{
					var serviceUrl = serviceRow.Cells["Service URL"].Value.ToString ();
					checkedServices.Add ( serviceUrl );
				}
			}

			//Check that at least one service row was selected
			if ( checkedServices.Count == 0 )
			{
				MessageBox.Show ( "Please select the services for which you want to retrieve metadata", Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Build parameters to pass to the background worker
			var parameters = new object[2];
			parameters[0] = BackgroundWorkerTasks.FetchMetadata;
			parameters[1] = checkedServices;

			SetupFormForWork ();

			// Start the asynchronous operation
			bgwMain.RunWorkerAsync ( parameters );
		}

		private void chkAll_CheckedChanged ( object sender, EventArgs e )
		{
			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				serviceRow.Cells["Selected"].Value = chkAll.Checked;
			}
		}

		private void dgvServices_CurrentCellDirtyStateChanged ( object sender, EventArgs e )
		{
			if ( dgvServices.IsCurrentCellDirty )
			{
				dgvServices.CommitEdit ( DataGridViewDataErrorContexts.Commit );
			}
		}

		private void dgvServices_CellValueChanged ( object sender, DataGridViewCellEventArgs e )
		{
			updateSelection ();
		}

		private void btnCancelDownload_Click ( object sender, EventArgs e )
		{
			CancelWorker ();
		}

		private void mnuAddServices_Click ( object sender, EventArgs e )
		{
			AddServices ();
		}

		private void mnuRemoveSelectedServices_Click ( object sender, EventArgs e )
		{
			RemoveSelectedServicesPrep ();
		}

		private void mnuRefreshServiceList_Click ( object sender, EventArgs e )
		{
			Cursor = Cursors.WaitCursor;
			RefreshServiceList ();
			Cursor = Cursors.Default;
		}

		#endregion

		#region Background Worker

		private void CancelWorker ()
		{
			// Cancel the asynchronous operation
			bgwMain.CancelAsync ();

			// Disable the Cancel button
			btnCancelDownload.Enabled = false;

			gbxProgress.Text = "Cancelling...";
		}

		private void bgwMain_DoWork ( object sender, DoWorkEventArgs e )
		{
			var parameters = e.Argument as object[];
			var task = (BackgroundWorkerTasks)parameters[0];
			if ( task == BackgroundWorkerTasks.FetchMetadata )
			{
				var serviceUrls = parameters[1] as List<string>;
				e.Result = FetchMetadata ( serviceUrls, e );
			}
			else if ( task == BackgroundWorkerTasks.RemoveServices )
			{
				var serviceUrls = parameters[1] as List<string>;
				e.Result = RemoveSelectedServices ( serviceUrls, e );
			}
			else
			{
				e.Result = "Unknown task provided to background worker";
			}
		}

		private void bgwMain_ProgressChanged ( object sender, ProgressChangedEventArgs e )
		{
			prgMain.Value = e.ProgressPercentage;
			gbxProgress.Text = e.UserState.ToString ();
		}

		private void bgwMain_RunWorkerCompleted ( object sender, RunWorkerCompletedEventArgs e )
		{
			Cursor = Cursors.Default;

			// Report the result
			if ( e.Error != null )
			{
				MessageBox.Show ( e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			else if ( e.Cancelled )
			{
				if ( Visible && _formIsClosing == false )
				{
					MessageBox.Show ( "Operation cancelled", Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
				}
			}
			else
			{
				MessageBox.Show ( e.Result.ToString (), Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
			}

			// Enable controls that were disabled until the asynchronous operation was finished
			RestoreFormFromWork ();

			SelectNewServices ();

			ActiveControl = btnUpdate;

			if ( _formIsClosing )
			{
				_formIsClosing = false;
				Hide ();
			}
		}

		#endregion

		#endregion

		#region Public Members

		/// <summary>
		/// Populates checked list box with web service URLs from the local cache.
		/// </summary>
		public void RefreshServiceList ()
		{
			try
			{
				var cacheManager = DatabaseOperations.GetCacheManager();
				var serviceList = cacheManager.GetAllServices ();

				// Put the data in a DataTable so that column sorting works
				var dataTable = new DataTable ();

				//First create the columns of the DataTable
				var col = new DataColumn ( "Selected", typeof ( bool ) );
				dataTable.Columns.Add ( col );

				col = new DataColumn ( "Title", typeof ( string ) ) {ReadOnly = true};
			    dataTable.Columns.Add ( col );

				col = new DataColumn ( "Harvested", typeof ( string ) ) {ReadOnly = true};
			    dataTable.Columns.Add ( col );

				col = new DataColumn ( "Service URL", typeof ( string ) ) {ReadOnly = true};
			    dataTable.Columns.Add ( col );

				//Then add all the rows
				foreach ( var serviceInfo in serviceList )
				{
					dataTable.Rows.Add ( new object[]{false, serviceInfo.ServiceTitle, 
                        serviceInfo.HarveDateTime, serviceInfo.EndpointURL} );
				}

				//Set the DataGrid to use the DataTable just created
				dgvServices.DataSource = dataTable;
				updateSelection ();
			}
			catch ( Exception ex )
			{
				MessageBox.Show ( ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			chkAll.Checked = false;
		}

		#endregion
	}
}
