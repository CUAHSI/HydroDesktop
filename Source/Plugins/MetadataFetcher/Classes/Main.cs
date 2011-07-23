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
using DotSpatial.Controls.Header;

#endregion

namespace HydroDesktop.MetadataFetcher
{
	class Main : Extension, IMapPlugin
	{
		#region Variables

        public const string TableTabKey = "kTable";
        
        // Reference to the main application and its UI items
		private IMapPluginArgs _mapArgs;

		private MainForm _mainForm = null;
		private AddServicesForm _addServicesForm = null;

		#endregion

		#region IExtension Members

		/// <summary>
		/// Fires when the plugin should become inactive
		/// </summary>
		protected override void OnDeactivate ()
		{
            _mapArgs.AppManager.HeaderControl.RemoveItems();
            
            //_mapArgs.AppManager.HeaderControl.RemoveItem("kDownloadMetadata");
            //_mapArgs.AppManager.HeaderControl.RemoveItem("kAddServices");
            //_mapArgs.Ribbon.Tabs[1].Panels.Remove ( _rPanelMetadataFetcher );

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

            //// Create a new Panel and add it to the "Table" Tab.
            //_rPanelMetadataFetcher = new RibbonPanel ( "Metadata", RibbonPanelFlowDirection.Bottom );
            //_rPanelMetadataFetcher.ButtonMoreVisible = false;
            //_rPanelMetadataFetcher.Image = Properties.Resources.Metadata_Fetcher_32;
            //_mapArgs.Ribbon.Tabs[1].Panels.Add ( _rPanelMetadataFetcher );

			// Add button to manage metadata.
            var btnDownloadMetadata = new SimpleActionItem("Manage", mnuDownloadMetadata_Click);
            //btnDownloadMetadata.Key = "kDownloadMetadata";
            btnDownloadMetadata.RootKey = TableTabKey;
			btnDownloadMetadata.LargeImage = Properties.Resources.Metadata_Fetcher_32;
			btnDownloadMetadata.SmallImage = Properties.Resources.Metadata_Fetcher_16;
			//ToolTipTitle is not supported in HeaderControl
            //btnDownloadMetadata.ToolTipTitle = "Manage";
			btnDownloadMetadata.ToolTipText = "Manage the contents of the local metadata catalog.";
            btnDownloadMetadata.GroupCaption = "Metadata";
            args.AppManager.HeaderControl.Add(btnDownloadMetadata);

			// Add button to add more services to the list of services for metadata harvesting.
			var btnAddServices = new SimpleActionItem ( "Add", mnuAddServices_Click );
            //btnAddServices.Key = "kAddServices";
            btnAddServices.RootKey = TableTabKey;
			btnAddServices.LargeImage = Properties.Resources.Metadata_Fetcher_Add_32;
			btnAddServices.SmallImage = Properties.Resources.Metadata_Fetcher_Add_16;
            //ToolTipTitle is not supported in HeaderControl
            //btnAddServices.ToolTipTitle = "Add";
			btnAddServices.ToolTipText = "Add services to the list of services that can be harvested in metadata catalog.";
            btnAddServices.GroupCaption = "Metadata";
			args.AppManager.HeaderControl.Add ( btnAddServices );
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

		#endregion

	}
}
