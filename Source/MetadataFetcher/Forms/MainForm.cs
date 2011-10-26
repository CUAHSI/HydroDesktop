using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices;
using HydroDesktop.WebServices.WaterOneFlow;
using HydroDesktop.MetadataFetcher;
using System.Threading;
using System.IO;

namespace HydroDesktop.MetadataFetcher.Forms
{
	public class MainForm : Form
	{
		#region Variables

		private enum BackgroundWorkerTasks { FetchMetadata, RemoveServices, Unknown }

		private AddServicesForm _addServicesForm = null;
		private bool _formIsClosing = false;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private ProgressBar prgMain;
		private BackgroundWorker bgwMain;
		private Button btnCancelDownload;
		private DataGridView dgvServices;
		private Label lblServicesSelected;
		private Label lblSelectionCount;
		private MenuStrip menuStrip1;
		private ToolStripMenuItem mnuServiceManagement;
		private ToolStripMenuItem mnuAddServices;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem mnuRemoveSelectedServices;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem mnuRefreshServiceList;

		#endregion

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent ()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle ();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle ();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle ();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager ( typeof ( MainForm ) );
			this.gbxServices = new System.Windows.Forms.GroupBox ();
			this.dgvServices = new System.Windows.Forms.DataGridView ();
			this.chkAll = new System.Windows.Forms.CheckBox ();
			this.btnUpdate = new System.Windows.Forms.Button ();
			this.gbxProgress = new System.Windows.Forms.GroupBox ();
			this.btnCancelDownload = new System.Windows.Forms.Button ();
			this.prgMain = new System.Windows.Forms.ProgressBar ();
			this.bgwMain = new System.ComponentModel.BackgroundWorker ();
			this.lblServicesSelected = new System.Windows.Forms.Label ();
			this.lblSelectionCount = new System.Windows.Forms.Label ();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip ();
			this.mnuServiceManagement = new System.Windows.Forms.ToolStripMenuItem ();
			this.mnuAddServices = new System.Windows.Forms.ToolStripMenuItem ();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator ();
			this.mnuRemoveSelectedServices = new System.Windows.Forms.ToolStripMenuItem ();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator ();
			this.mnuRefreshServiceList = new System.Windows.Forms.ToolStripMenuItem ();
			this.gbxServices.SuspendLayout ();
			((System.ComponentModel.ISupportInitialize)(this.dgvServices)).BeginInit ();
			this.gbxProgress.SuspendLayout ();
			this.menuStrip1.SuspendLayout ();
			this.SuspendLayout ();
			// 
			// gbxServices
			// 
			this.gbxServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxServices.Controls.Add ( this.dgvServices );
			this.gbxServices.Controls.Add ( this.chkAll );
			this.gbxServices.Controls.Add ( this.btnUpdate );
			this.gbxServices.Location = new System.Drawing.Point ( 12, 27 );
			this.gbxServices.Name = "gbxServices";
			this.gbxServices.Size = new System.Drawing.Size ( 718, 302 );
			this.gbxServices.TabIndex = 1;
			this.gbxServices.TabStop = false;
			this.gbxServices.Text = "Select services and then download metadata to update the metadata cache";
			// 
			// dgvServices
			// 
			this.dgvServices.AllowUserToAddRows = false;
			this.dgvServices.AllowUserToDeleteRows = false;
			this.dgvServices.AllowUserToOrderColumns = true;
			this.dgvServices.AllowUserToResizeRows = false;
			this.dgvServices.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvServices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvServices.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvServices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle2.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvServices.DefaultCellStyle = dataGridViewCellStyle2;
			this.dgvServices.Location = new System.Drawing.Point ( 6, 19 );
			this.dgvServices.MultiSelect = false;
			this.dgvServices.Name = "dgvServices";
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle3.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvServices.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.dgvServices.RowHeadersVisible = false;
			this.dgvServices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvServices.Size = new System.Drawing.Size ( 706, 250 );
			this.dgvServices.TabIndex = 5;
			this.dgvServices.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler ( this.dgvServices_CellValueChanged );
			this.dgvServices.CurrentCellDirtyStateChanged += new System.EventHandler ( this.dgvServices_CurrentCellDirtyStateChanged );
			// 
			// chkAll
			// 
			this.chkAll.AutoSize = true;
			this.chkAll.Location = new System.Drawing.Point ( 9, 275 );
			this.chkAll.Name = "chkAll";
			this.chkAll.Size = new System.Drawing.Size ( 70, 17 );
			this.chkAll.TabIndex = 1;
			this.chkAll.Text = "Select &All";
			this.chkAll.UseVisualStyleBackColor = true;
			this.chkAll.CheckedChanged += new System.EventHandler ( this.chkAll_CheckedChanged );
			// 
			// btnUpdate
			// 
			this.btnUpdate.Location = new System.Drawing.Point ( 599, 273 );
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.Size = new System.Drawing.Size ( 113, 23 );
			this.btnUpdate.TabIndex = 4;
			this.btnUpdate.Text = "&Download Metadata";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler ( this.btnUpdate_Click );
			// 
			// gbxProgress
			// 
			this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbxProgress.Controls.Add ( this.btnCancelDownload );
			this.gbxProgress.Controls.Add ( this.prgMain );
			this.gbxProgress.Location = new System.Drawing.Point ( 12, 335 );
			this.gbxProgress.Name = "gbxProgress";
			this.gbxProgress.Size = new System.Drawing.Size ( 718, 46 );
			this.gbxProgress.TabIndex = 5;
			this.gbxProgress.TabStop = false;
			this.gbxProgress.Text = "Ready";
			this.gbxProgress.Visible = false;
			// 
			// btnCancelDownload
			// 
			this.btnCancelDownload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancelDownload.Enabled = false;
			this.btnCancelDownload.Location = new System.Drawing.Point ( 627, 16 );
			this.btnCancelDownload.Name = "btnCancelDownload";
			this.btnCancelDownload.Size = new System.Drawing.Size ( 85, 22 );
			this.btnCancelDownload.TabIndex = 28;
			this.btnCancelDownload.Text = "Cancel";
			this.btnCancelDownload.UseVisualStyleBackColor = true;
			this.btnCancelDownload.Click += new System.EventHandler ( this.btnCancelDownload_Click );
			// 
			// prgMain
			// 
			this.prgMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.prgMain.Location = new System.Drawing.Point ( 9, 16 );
			this.prgMain.Name = "prgMain";
			this.prgMain.Size = new System.Drawing.Size ( 612, 22 );
			this.prgMain.TabIndex = 0;
			// 
			// bgwMain
			// 
			this.bgwMain.WorkerReportsProgress = true;
			this.bgwMain.WorkerSupportsCancellation = true;
			// 
			// lblServicesSelected
			// 
			this.lblServicesSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblServicesSelected.AutoSize = true;
			this.lblServicesSelected.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			this.lblServicesSelected.Location = new System.Drawing.Point ( 26, 354 );
			this.lblServicesSelected.Name = "lblServicesSelected";
			this.lblServicesSelected.Size = new System.Drawing.Size ( 125, 15 );
			this.lblServicesSelected.TabIndex = 6;
			this.lblServicesSelected.Text = "Services Selected:";
			// 
			// lblSelectionCount
			// 
			this.lblSelectionCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.lblSelectionCount.AutoSize = true;
			this.lblSelectionCount.Font = new System.Drawing.Font ( "Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
			this.lblSelectionCount.Location = new System.Drawing.Point ( 149, 354 );
			this.lblSelectionCount.Name = "lblSelectionCount";
			this.lblSelectionCount.Size = new System.Drawing.Size ( 15, 15 );
			this.lblSelectionCount.TabIndex = 7;
			this.lblSelectionCount.Text = "0";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange ( new System.Windows.Forms.ToolStripItem[] {
            this.mnuServiceManagement} );
			this.menuStrip1.Location = new System.Drawing.Point ( 0, 0 );
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size ( 742, 24 );
			this.menuStrip1.TabIndex = 8;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// mnuServiceManagement
			// 
			this.mnuServiceManagement.DropDownItems.AddRange ( new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddServices,
            this.toolStripSeparator1,
            this.mnuRemoveSelectedServices,
            this.toolStripSeparator2,
            this.mnuRefreshServiceList} );
			this.mnuServiceManagement.Name = "mnuServiceManagement";
			this.mnuServiceManagement.Size = new System.Drawing.Size ( 119, 20 );
			this.mnuServiceManagement.Text = "Service Management";
			// 
			// mnuAddServices
			// 
			this.mnuAddServices.Name = "mnuAddServices";
			this.mnuAddServices.Size = new System.Drawing.Size ( 230, 22 );
			this.mnuAddServices.Text = "&Add WaterOneFlow Service(s)";
			this.mnuAddServices.Click += new System.EventHandler ( this.mnuAddServices_Click );
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size ( 227, 6 );
			// 
			// mnuRemoveSelectedServices
			// 
			this.mnuRemoveSelectedServices.Name = "mnuRemoveSelectedServices";
			this.mnuRemoveSelectedServices.Size = new System.Drawing.Size ( 230, 22 );
			this.mnuRemoveSelectedServices.Text = "&Remove Selected Services";
			this.mnuRemoveSelectedServices.Click += new System.EventHandler ( this.mnuRemoveSelectedServices_Click );
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size ( 227, 6 );
			// 
			// mnuRefreshServiceList
			// 
			this.mnuRefreshServiceList.Name = "mnuRefreshServiceList";
			this.mnuRefreshServiceList.Size = new System.Drawing.Size ( 230, 22 );
			this.mnuRefreshServiceList.Text = "Refresh Service List";
			this.mnuRefreshServiceList.Click += new System.EventHandler ( this.mnuRefreshServiceList_Click );
			// 
			// MainForm
			// 
			this.AcceptButton = this.btnUpdate;
			this.AutoScaleDimensions = new System.Drawing.SizeF ( 6F, 13F );
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size ( 742, 391 );
			this.Controls.Add ( this.lblSelectionCount );
			this.Controls.Add ( this.lblServicesSelected );
			this.Controls.Add ( this.gbxProgress );
			this.Controls.Add ( this.gbxServices );
			this.Controls.Add ( this.menuStrip1 );
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject ( "$this.Icon" )));
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimumSize = new System.Drawing.Size ( 480, 247 );
			this.Name = "MainForm";
			this.Text = "Metadata Fetcher";
			this.Load += new System.EventHandler ( this.MainForm_Load );
			this.gbxServices.ResumeLayout ( false );
			this.gbxServices.PerformLayout ();
			((System.ComponentModel.ISupportInitialize)(this.dgvServices)).EndInit ();
			this.gbxProgress.ResumeLayout ( false );
			this.menuStrip1.ResumeLayout ( false );
			this.menuStrip1.PerformLayout ();
			this.ResumeLayout ( false );
			this.PerformLayout ();

		}

		private System.Windows.Forms.GroupBox gbxServices;
		private System.Windows.Forms.Button btnUpdate;
		private CheckBox chkAll;
		private GroupBox gbxProgress;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose ( bool disposing )
		{
			if ( disposing && (components != null) )
			{
				components.Dispose ();
			}
			base.Dispose ( disposing );
		}

		#endregion

		#region Constructor

		public MainForm ()
		{
			InitializeComponent ();

			this.FormClosing += new FormClosingEventHandler ( MainForm_FormClosing );

			// Background worker for data download
			bgwMain = new BackgroundWorker ();
			bgwMain.DoWork += new DoWorkEventHandler ( bgwMain_DoWork );
			bgwMain.ProgressChanged += new ProgressChangedEventHandler ( bgwMain_ProgressChanged );
			bgwMain.RunWorkerCompleted += new RunWorkerCompletedEventHandler ( bgwMain_RunWorkerCompleted );
			bgwMain.WorkerReportsProgress = true;
			bgwMain.WorkerSupportsCancellation = true;
		}

		#endregion

		#region Private Methods

		#region Form Loading / Unloading

		private void MainForm_Load ( object sender, EventArgs e )
		{
			this.Cursor = Cursors.WaitCursor;
			RefreshServiceList ();
			this.Cursor = Cursors.Default;

			mnuRemoveSelectedServices.Enabled = false;
		}

		void MainForm_FormClosing ( object sender, FormClosingEventArgs e )
		{
			e.Cancel = true;

			if ( bgwMain.IsBusy == true )
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
			DialogResult result = _addServicesForm.ShowDialog ( this );
			SelectNewServices ();
		}

		private void RemoveSelectedServicesPrep ()
		{
			if ( dgvServices.Rows.Count == 0 )
			{
				MessageBox.Show ( "There are no services to remove", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Build a list of services that the user marked for removal
			List<string> checkedServices = new List<string> ();

			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				bool selected = (bool)serviceRow.Cells["Selected"].Value;
				if ( selected )
				{
					string serviceUrl = serviceRow.Cells["Service URL"].Value.ToString ();
					checkedServices.Add ( serviceUrl );
				}
			}

			if ( checkedServices.Count == 0 )
			{
				MessageBox.Show ( "Please select services to remove and try again", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			if ( MessageBox.Show ( "Are you sure you want to DELETE the selected services from your local cache database?", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
			{


				// Build parameters to pass to the background worker
				object[] parameters = new object[2];
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
			int totalServices = serviceUrls.Count;
			int currentService = 0;

            MetadataCacheManagerSQL cacheManager = DatabaseOperations.GetCacheManager();

			foreach ( string serviceUrl in serviceUrls )
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
				DataServiceInfo serviceInfo = DatabaseOperations.GetDataServiceFromCache ( serviceUrl );

				try
				{
					cacheManager.DeleteRecordsForService ( serviceInfo, true );
				}
				catch ( Exception ex )
				{
					throw new Exception ( "Unable to remove service from local metadata database. \n" + serviceUrl + "\n" + ex.Message );
				}
			}


			string message = "";
			if ( totalServices == 1 )
			{
				message = "Service successfully removed";
			}
			else
			{
				message = "Services successfully removed";
			}

			return message;
		}

		/// <summary>
		/// Refreshes the list of services in the DataGridView, and selects new ones
		/// </summary>
		internal void SelectNewServices ()
		{
			// Keep track of existing services
			List<string> existingUris = new List<string> ();
			List<string> checkedUris = new List<string> ();

			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				string serviceUrl = serviceRow.Cells["Service URL"].Value.ToString ();
				existingUris.Add ( serviceUrl );

				if ( Convert.ToBoolean(serviceRow.Cells["Selected"].Value) == true )
				{
					checkedUris.Add ( serviceUrl );
				}
			}

			int previousServiceCount = dgvServices.Rows.Count;

			// Refresh the service list
			RefreshServiceList ();

			if ( dgvServices.Rows.Count > previousServiceCount )
			{
				// New services added, so select any services that weren't previously in the list
				foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
				{
					string serviceUri = serviceRow.Cells["Service URL"].Value.ToString ();

					serviceRow.Cells["Selected"].Value = !existingUris.Contains ( serviceUri );
				}
			}
			else
			{
				// No new services added, so select any existing services that the user had previously selected
				foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
				{
					string serviceUri = serviceRow.Cells["Service URL"].Value.ToString ();

					serviceRow.Cells["Selected"].Value = checkedUris.Contains ( serviceUri );
				}
			}
			

		}

		/// <summary>
		/// Updates the label that shows the number of selected services and colors selected rows.
		/// </summary>
		private void updateSelection ()
		{
			int count = 0;
			mnuRemoveSelectedServices.Enabled = false;


			foreach ( DataGridViewRow row in dgvServices.Rows )
			{
				if ( (bool)row.Cells["Selected"].Value == true )
				{
					count++;

					mnuRemoveSelectedServices.Enabled = true;

					Color orig = row.Cells["Selected"].Style.BackColor;

					foreach ( DataGridViewCell cell in row.Cells )
						cell.Style.BackColor = Color.LightYellow;
				}
				else
				{
					foreach ( DataGridViewCell cell in row.Cells )
						cell.Style.BackColor = Color.Empty;
				}

			}

			lblSelectionCount.Text = count.ToString ();
		}


		private string FetchMetadataOld ( List<string> serviceUrls, DoWorkEventArgs e )
		{
			int totalServices = serviceUrls.Count;
			int currentService = 0;
			int seriesCount = 0;

			MetadataCacheManagerSQL cacheManager = DatabaseOperations.GetCacheManager();

			StringBuilder errors = new StringBuilder ();

			DateTime testStart = DateTime.Now;

			foreach ( string serviceUrl in serviceUrls )
			{
				// Update progress
				currentService++;
				bgwMain.ReportProgress ( 0, "Querying service " + currentService + " of " + totalServices + " for site list..." );

				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Get the service info for this item
				DataServiceInfo serviceInfo = DatabaseOperations.GetDataServiceFromCache ( serviceUrl );

				// Create a WaterOneFlow Service for this URL
				WaterOneFlowClient waterOneFlowClient;
				try
				{
					waterOneFlowClient = new WaterOneFlowClient ( serviceUrl );
				}
				catch ( Exception ex )
				{
					//throw new Exception ( "Could not connect to service with URL: " + serviceUrl + ".\n" + ex.Message );
					errors.AppendLine ( "Could not connect to service with URL: " + serviceUrl + ".\n" + ex.Message + "\n\n" );
					continue;
				}

				DataServiceInfo waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;

				// Get all series for all sites at this service
				IList<Site> siteList;

				try
				{
					siteList = waterOneFlowClient.GetSites ();
				}
				catch ( Exception ex )
				{
					//throw new Exception ( "Could not get site list from service with URL: " + serviceUrl + ".\n" + ex.Message );
					errors.AppendLine ( "Could not get site list from service with URL: " + serviceUrl + ".\n" + ex.Message + "\n\n" );
					continue;
				}


				IList<SeriesMetadata> seriesList = new List<SeriesMetadata> ();

				int totalSteps = siteList.Count;
				int currentStep = 0;

				bool getSiteInfoFailed = false;
				int siteErrorCount = 0;

				foreach ( Site site in siteList )
				{
					// Update progress
					currentStep++;
					bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Querying service " + currentService + " of " + totalServices + ", site " + currentStep + " of " + totalSteps + " for series..." );

					if ( bgwMain.CancellationPending )
					{
						e.Cancel = true;
						return "Operation cancelled";
					}

					// Get series for this site
					IList<SeriesMetadata> currentSeriesList;

					try
					{
						currentSeriesList = waterOneFlowClient.GetSiteInfo ( site.Code );
					}
					catch
					{
						//throw new Exception ( "Could not get site info for site " + site.Code + " from service with URL: " + serviceUrl + ".\n" + ex.Message );
						if ( getSiteInfoFailed == false )
						{
							getSiteInfoFailed = true;
						}
						siteErrorCount++;
						continue;
					}

					foreach ( SeriesMetadata series in currentSeriesList )
					{
						seriesList.Add ( series );
					}
				}

				if ( siteErrorCount == 1 )
				{
					errors.AppendLine ( "Could not get site info for 1 site in service with URL: " + serviceUrl + "\n\n" );
				}
				else if ( siteErrorCount > 1 )
				{
					errors.AppendLine ( "Could not get site info for " + siteErrorCount.ToString () + " sites in service with URL: " + serviceUrl + "\n\n" );
				}

				totalSteps = seriesList.Count;
				currentStep = 0;

				// Update progress
				bgwMain.ReportProgress ( 0, "Deleting old records for service " + currentService + " of " + totalServices + "..." );

				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Delete existing records for this service
				cacheManager.DeleteRecordsForService ( serviceInfo, false );

				// Update progress
				bgwMain.ReportProgress ( 0, "Determining spatial extent of service " + currentService + " of " + totalServices + "..." );

				if ( bgwMain.CancellationPending )
				{
					e.Cancel = true;
					return "Operation cancelled";
				}

				// Get the extent of the service
				double east = -180;
				double west = 360;
				double north = -90;
				double south = 90;
				foreach ( SeriesMetadata series in seriesList )
				{
					HydroDesktop.Interfaces.ObjectModel.Site site = series.Site;
					if ( site != null )
					{
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
					}
				}

				// Update service info
				serviceInfo.IsHarvested = true;
				serviceInfo.HarveDateTime = DateTime.Now;
				serviceInfo.ServiceName = waterOneFlowServiceInfo.ServiceName;
				serviceInfo.Version = waterOneFlowServiceInfo.Version;
				serviceInfo.ServiceType = waterOneFlowServiceInfo.ServiceType;
				serviceInfo.Protocol = waterOneFlowServiceInfo.Protocol;
				serviceInfo.EastLongitude = east;
				serviceInfo.WestLongitude = west;
				serviceInfo.NorthLatitude = north;
				serviceInfo.SouthLatitude = south;

				cacheManager.UpdateDataRow ( serviceInfo ); // Updates properties like harvest datetime and service extent

				foreach ( SeriesMetadata series in seriesList )
				{
					// Update progress
					currentStep++;
					bgwMain.ReportProgress ( 100 * currentStep / totalSteps, "Saving series " + currentStep + " of " + totalSteps + " from service " + currentService + " of " + totalServices + "..." );

					if ( bgwMain.CancellationPending )
					{
						e.Cancel = true;
						return "Operation cancelled";
					}

					// Save series to metadata cache database
					try
					{
						cacheManager.SaveSeries ( series, serviceInfo );
					}
					catch ( Exception ex )
					{
						//throw new Exception ( "Could not save metadata for service with URL: " + serviceInfo.EndpointURL + ".\n" + ex.Message );
						errors.AppendLine ( "Could not save metadata for service with URL: " + serviceInfo.EndpointURL + ".\n" + ex.Message + "\n\n" );
						continue;
					}

					seriesCount++;
				}

			}

			// Update progress
			bgwMain.ReportProgress ( 100, "Operation complete" );

			if ( bgwMain.CancellationPending )
			{
				e.Cancel = true;
				return "Operation cancelled";
			}

			// Report result
			string message = "Download complete. " + seriesCount.ToString () + " series saved to metadata cache database.";

			if ( errors.Length > 0 )
			{
				message += "\n\nSome errors occurred during the operation:\n\n" + errors.ToString (); ;
			}

			TimeSpan span = DateTime.Now.Subtract ( testStart );
			string seconds = span.TotalSeconds.ToString ();

			return message;
		}

		private string FetchMetadata ( List<string> serviceUrls, DoWorkEventArgs e )
		{
			int totalServices = serviceUrls.Count;
			int currentService = 0;
			int seriesCount = 0; // Keeps track of how many series were successfully processed

			MetadataCacheManagerSQL cacheManager = DatabaseOperations.GetCacheManager();

			StringBuilder errors = new StringBuilder ();  // Keep track of errors and report them at the end

			foreach ( string serviceUrl in serviceUrls )
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
				DataServiceInfo serviceInfo = DatabaseOperations.GetDataServiceFromCache ( serviceUrl );

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
				DataServiceInfo waterOneFlowServiceInfo = waterOneFlowClient.ServiceInfo;

				serviceInfo.IsHarvested = false;
				serviceInfo.ServiceName = waterOneFlowServiceInfo.ServiceName;
				serviceInfo.Version = waterOneFlowServiceInfo.Version;
				serviceInfo.ServiceType = waterOneFlowServiceInfo.ServiceType;
				serviceInfo.Protocol = waterOneFlowServiceInfo.Protocol;

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
				catch ( Exception ex )
				{
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

				int totalSteps = siteList.Count;
				int currentStep = 0;

				// Error tracking
				int siteErrorCount = 0; // Keeps track of how many errors we had while downloading site info for the current service
				int saveErrorCount = 0; // Keeps track of how many errors we had while saving site info for the current service
				string firstSiteError = ""; // Records the message from the first error we had while getting site info for the current service
				string firstSaveError = ""; // Records the message from the first error we had while saving site info for the current service

				// Default extent for the service.  These values are designed to be overwritten as we query sites in the service
				double east = -180;
				double west = 360;
				double north = -90;
				double south = 90;

				foreach ( Site site in siteList )
				{
					// Check for cancel
					if ( bgwMain.CancellationPending )
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
						currentSeriesList = waterOneFlowClient.GetSiteInfo ( site.Code );
					}
					catch ( Exception ex )
					{
						// Flag the error and continue to the next site
						siteErrorCount++;

						if ( siteErrorCount == 1 )
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
					foreach ( SeriesMetadata series in currentSeriesList )
					{
						// Check for cancel
						if ( bgwMain.CancellationPending )
						{
							e.Cancel = true;
							return "Operation cancelled";
						}

						try
						{
							cacheManager.SaveSeries ( series, serviceInfo );
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
					errors.AppendLine ( "Could not get site info for " + siteErrorCount.ToString () + " sites in service with URL: " + serviceUrl + 
						"\n" + firstSiteError + "\n\n" );
				}

				if ( saveErrorCount == 1 )
				{
					errors.AppendLine ( "Could not save site info for 1 site in service with URL: " + serviceUrl + 
						"\n" + firstSaveError + "\n\n" );
				}
				else if ( saveErrorCount > 1 )
				{
					errors.AppendLine ( "Could not save site info for " + siteErrorCount.ToString () + " sites in service with URL: " + serviceUrl + 
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

				cacheManager.UpdateDataRow ( serviceInfo ); // Updates properties like harvest datetime and service extent
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
			string message = "Download complete. " + seriesCount.ToString () + " series saved to metadata cache database.";

			if ( errors.Length > 0 )
			{
				message += "\n\nSome errors occurred during the operation:\n\n" + errors.ToString (); ;
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

			this.Cursor = Cursors.WaitCursor;
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

		private void btnRemove_Click ( object sender, EventArgs e )
		{
			RemoveSelectedServicesPrep ();
		}

		private void btnAddService_Click ( object sender, EventArgs e )
		{
			AddServices ();
		}

		private void btnUpdate_Click ( object sender, EventArgs e )
		{
			if ( dgvServices.Rows.Count == 0 )
			{
				MessageBox.Show ( "Please add a WaterOneFlow web service to the list", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			if ( bgwMain.IsBusy )
			{
				MessageBox.Show ( "The background worker is currently busy.  Please try again in a few moments.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Build a list of services that the user marked for download
			List<string> checkedServices = new List<string> ();

			foreach ( DataGridViewRow serviceRow in dgvServices.Rows )
			{
				bool selected = (bool)serviceRow.Cells["Selected"].Value;

				if ( selected )
				{
					string serviceUrl = serviceRow.Cells["Service URL"].Value.ToString ();
					checkedServices.Add ( serviceUrl );
				}
			}

			//Check that at least one service row was selected
			if ( checkedServices.Count == 0 )
			{
				MessageBox.Show ( "Please select the services for which you want to retrieve metadata", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Asterisk );
				return;
			}

			// Build parameters to pass to the background worker
			object[] parameters = new object[2];
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
			this.Cursor = Cursors.WaitCursor;
			RefreshServiceList ();
			this.Cursor = Cursors.Default;
		}

		private class ServiceInfoRow
		{
			private bool _selected;
			private string _serviceTitle;
			private DateTime _harveDateTime;
			private string _endpointUrl;

			public ServiceInfoRow ( bool selected, string serviceTitle, DateTime harveDateTime, string endpointUrl )
			{
				_selected = selected;
				_serviceTitle = serviceTitle;
				_harveDateTime = harveDateTime;
				_endpointUrl = endpointUrl;
			}

			public bool Selected { get { return _selected; } set { _selected = value; } }
			public string ServiceTitle { get { return _serviceTitle; } }
			public DateTime HarveDateTime { get { return _harveDateTime; } }
			public string EndpointUrl { get { return _endpointUrl; } }
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
			object[] parameters = e.Argument as object[];
			BackgroundWorkerTasks task = (BackgroundWorkerTasks)parameters[0];
			if ( task == BackgroundWorkerTasks.FetchMetadata )
			{
				List<string> serviceUrls = parameters[1] as List<string>;
				e.Result = FetchMetadata ( serviceUrls, e );
			}
			else if ( task == BackgroundWorkerTasks.RemoveServices )
			{
				List<string> serviceUrls = parameters[1] as List<string>;
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
			this.Cursor = Cursors.Default;

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
				MessageBox.Show ( e.Result.ToString (), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
			}

			// Enable controls that were disabled until the asynchronous operation was finished
			RestoreFormFromWork ();

			SelectNewServices ();

			this.ActiveControl = btnUpdate;

			if ( _formIsClosing == true )
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
				MetadataCacheManagerSQL cacheManager = DatabaseOperations.GetCacheManager();
				List<DataServiceInfo> serviceList = cacheManager.GetAllServices () as List<DataServiceInfo>;

				// Put the data in a DataTable so that column sorting works
				DataTable dataTable = new DataTable ();

				//First create the columns of the DataTable
				DataColumn col = new DataColumn ( "Selected", typeof ( bool ) );
				dataTable.Columns.Add ( col );

				col = new DataColumn ( "Title", typeof ( string ) );
				col.ReadOnly = true;
				dataTable.Columns.Add ( col );

				col = new DataColumn ( "Harvested", typeof ( string ) );
				col.ReadOnly = true;
				dataTable.Columns.Add ( col );

				col = new DataColumn ( "Service URL", typeof ( string ) );
				col.ReadOnly = true;
				dataTable.Columns.Add ( col );

				//Then add all the rows
				foreach ( DataServiceInfo serviceInfo in serviceList )
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
				MessageBox.Show ( ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error );
			}

			chkAll.Checked = false;
		}

		#endregion


	}

}



