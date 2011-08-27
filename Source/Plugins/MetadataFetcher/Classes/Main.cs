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
	class Main : Extension
	{
		#region Variables

        public const string TableTabKey = "kHydroTable";

		private MainForm _mainForm = null;
		private AddServicesForm _addServicesForm = null;

		#endregion

		#region IExtension Members

		/// <summary>
		/// Fires when the plugin should become inactive
		/// </summary>
		public override void Deactivate ()
		{
            App.HeaderControl.RemoveItems();

			//necessary in plugin deactivation
			base.Deactivate ();
		}

		#endregion

		#region IMapPlugin Members

		/// <summary>
		/// Activate the HydroDesktop plugin
		/// </summary>
		public override void Activate ()
		{
			// Add button to manage metadata.
            var btnDownloadMetadata = new SimpleActionItem("Manage", mnuDownloadMetadata_Click);

            btnDownloadMetadata.RootKey = TableTabKey;
			btnDownloadMetadata.LargeImage = Properties.Resources.Metadata_Fetcher_32;
			btnDownloadMetadata.SmallImage = Properties.Resources.Metadata_Fetcher_16;
			//ToolTipTitle is not supported in HeaderControl
            //btnDownloadMetadata.ToolTipTitle = "Manage";
			btnDownloadMetadata.ToolTipText = "Manage the contents of the local metadata catalog.";
            btnDownloadMetadata.GroupCaption = "Metadata";
            App.HeaderControl.Add(btnDownloadMetadata);

			// Add button to add more services to the list of services for metadata harvesting.
			var btnAddServices = new SimpleActionItem ( "Add", mnuAddServices_Click );
            btnAddServices.RootKey = TableTabKey;
			btnAddServices.LargeImage = Properties.Resources.Metadata_Fetcher_Add_32;
			btnAddServices.SmallImage = Properties.Resources.Metadata_Fetcher_Add_16;
            //ToolTipTitle is not supported in HeaderControl
            //btnAddServices.ToolTipTitle = "Add";
			btnAddServices.ToolTipText = "Add services to the list of services that can be harvested in metadata catalog.";
            btnAddServices.GroupCaption = "Metadata";
			App.HeaderControl.Add ( btnAddServices );

            base.Activate();
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
