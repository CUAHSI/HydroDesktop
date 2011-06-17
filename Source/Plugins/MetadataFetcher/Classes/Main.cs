#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HydroDesktop.MetadataFetcher.Forms;
using System.Windows.Forms;

using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using System.Drawing;
using HydroDesktop.Database;

#endregion

namespace HydroDesktop.MetadataFetcher
{
	[Plugin ( "Metadata Fetcher", Author = "MapWindow", UniqueName = "mw_DataFetcher_1", Version = "1" )]
	class Main : Extension, IMapPlugin
	{
		#region Variables

		// Reference to the main application and its UI items
		private IMapPluginArgs _mapArgs;

		private RibbonPanel _rPanelMetadataFetcher;

		private RibbonButton _rBtnDownloadMetadata;

		private RibbonButton _rBtnAddServices;

		// Toolbar button added by the plugin
		//private ToolStripButton btnMetadataFetcher;

		// Menu for the Metadata Fetcher
		//private ToolStripMenuItem mnuMetadataFetcher;

		private MainForm _mainForm = null;
		private AddServicesForm _addServicesForm = null;

		#endregion

		#region IExtension Members

		/// <summary>
		/// Fires when the plugin should become inactive
		/// </summary>
		protected override void OnDeactivate ()
		{
			_mapArgs.Ribbon.Tabs[1].Panels.Remove ( _rPanelMetadataFetcher );

			//necessary in plugin deactivation
			base.OnDeactivate ();
		}

		#endregion

		#region IMapPlugin Members

		/// <summary>
		/// Initialize the HydroDesktop plugin
		/// </summary>
		/// <param name="args">The plugin arguments to access the main application</param>
		public void Initialize ( IMapPluginArgs args )
		{
			_mapArgs = args;

			// Create a new Panel and add it to the "Data" Tab.
			_rPanelMetadataFetcher = new RibbonPanel ( "Metadata", RibbonPanelFlowDirection.Bottom );
			_rPanelMetadataFetcher.ButtonMoreVisible = false;
			_rPanelMetadataFetcher.Image = Properties.Resources.Metadata_Fetcher_32;
			_mapArgs.Ribbon.Tabs[1].Panels.Add ( _rPanelMetadataFetcher );

			// Add button to manage metadata.
			_rBtnDownloadMetadata = new RibbonButton ( "Manage" );
			_rBtnDownloadMetadata.Image = Properties.Resources.Metadata_Fetcher_32;
			_rBtnDownloadMetadata.SmallImage = Properties.Resources.Metadata_Fetcher_16;
			_rBtnDownloadMetadata.ToolTipTitle = "Manage";
			_rBtnDownloadMetadata.ToolTip = "Manage the contents of the local metadata catalog.";
			_rBtnDownloadMetadata.Click += new EventHandler ( mnuDownloadMetadata_Click );

			_rPanelMetadataFetcher.Items.Add ( _rBtnDownloadMetadata );


			// Add button to add more services to the list of services for metadata harvesting.
			_rBtnAddServices = new RibbonButton ( "Add" );
			_rBtnAddServices.Image = Properties.Resources.Metadata_Fetcher_Add_32;
			_rBtnAddServices.SmallImage = Properties.Resources.Metadata_Fetcher_Add_16;
			_rBtnAddServices.ToolTipTitle = "Add";
			_rBtnAddServices.ToolTip = "Add services to the list of services that can be harvested in metadata catalog.";
			_rBtnAddServices.Click += new EventHandler ( mnuAddServices_Click );

			_rPanelMetadataFetcher.Items.Add ( _rBtnAddServices );

			//btnMetadataFetcher = new ToolStripButton ();

			// Add UI features
			//btnMetadataFetcher.DisplayStyle = ToolStripItemDisplayStyle.Image;
			//btnMetadataFetcher.Image = Properties.Resources.Database.ToBitmap ();
			//btnMetadataFetcher.Name = "btnMetadataFetcher";
			//btnMetadataFetcher.ToolTipText = "Metadata Fetcher";
			//btnMetadataFetcher.Size = new System.Drawing.Size ( 23, 69 );
			//btnMetadataFetcher.Click += new EventHandler ( btnMetadataFetcher_Click );

			//if ( _mapArgs.ToolStripContainer != null )
			//{
			//	_mapArgs.MainToolStrip.Items.Add ( btnMetadataFetcher );
			//}

			//Add ToolStrip Menu for this plugin
			//mnuMetadataFetcher = new ToolStripMenuItem("Metadata Fetcher");

			//Add entries to the plugin menu
			//ToolStripMenuItem mnuDownloadMetadata = new ToolStripMenuItem("Download Metadata");
			//mnuMetadataFetcher.DropDown.Items.Add(mnuDownloadMetadata);

			//ToolStripMenuItem mnuAddServices = new ToolStripMenuItem("Add Services");
			//mnuMetadataFetcher.DropDown.Items.Add(mnuAddServices);

			//ToolStripMenuItem mnuRemoveServices = new ToolStripMenuItem("Remove Services");
			//mnuMetadataFetcher.DropDown.Items.Add(mnuRemoveServices);

			//if (_mapArgs.MainMenu != null)
			//{
			//    _mapArgs.MainMenu.Items.Add(mnuMetadataFetcher);
			//    mnuDownloadMetadata.Click += new EventHandler(mnuDownloadMetadata_Click);
			//    mnuAddServices.Click +=new EventHandler(mnuAddServices_Click);
			//}

		}

		#endregion

		#region Event Handlers

		void mnuDownloadMetadata_Click ( object sender, EventArgs e )
		{
			// Initialize the main form
			if ( _mainForm == null )
			{
				_mainForm = new MainForm ();
			}

			// Show the form
			if ( _mainForm.Visible == false )
			{
                _mainForm.RefreshServiceList();
                _mainForm.Show ();
			}

			_mainForm.Focus ();
		}

		void mnuAddServices_Click ( object sender, EventArgs e )
		{
			if ( _addServicesForm == null )
				_addServicesForm = new AddServicesForm ();

			DialogResult result = _addServicesForm.ShowDialog ();

			if ( _mainForm != null )
			{
				_mainForm.SelectNewServices ();
			}
		}


		//TODO: Maybe Change this Icon to something about downloading metadata
		//void btnMetadataFetcher_Click(object sender, EventArgs e)
		//{
		//    // Initialize the main form
		//    if ( _mainForm == null )
		//    {
		//        _mainForm = new MainForm ();
		//    }

		//    // Show the form
		//    if ( _mainForm.Visible == false )
		//    {
		//        _mainForm.Show ();
		//    }

		//    //_mainForm.RefreshServiceList(); //This is already called on the mainForm_Load event method
		//    _mainForm.Focus();
		//}

		#endregion

	}
}
