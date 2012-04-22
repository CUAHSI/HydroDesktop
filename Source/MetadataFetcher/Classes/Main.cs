#region Namespaces
using System;
using HydroDesktop.Common;
using HydroDesktop.MetadataFetcher.Forms;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.MetadataFetcher.Properties;
using Msg = HydroDesktop.MetadataFetcher.MessageStrings;

#endregion

namespace HydroDesktop.MetadataFetcher
{
	class Main : Extension
	{
		#region Variables

		private MainForm _mainForm;
		private AddServicesForm _addServicesForm;

		#endregion

		#region IExtension Members

		/// <summary>
		/// Fires when the plugin should become inactive
		/// </summary>
		public override void Deactivate ()
		{
            App.HeaderControl.RemoveAll();

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
		    var rootKey = SharedConstants.SearchRootkey;
		    var dsGroup = SharedConstants.SearchDataSourcesGroupName;
		    var header = App.HeaderControl;

            header.Add(new SimpleActionItem(rootKey, Msg.Manage, mnuDownloadMetadata_Click) { LargeImage = Resources.Metadata_Fetcher_32, SmallImage = Resources.Metadata_Fetcher_16, ToolTipText = Msg.Manage_ToolTip, GroupCaption = dsGroup });
            header.Add(new SimpleActionItem(rootKey, Msg.Add, mnuAddServices_Click) { LargeImage = Resources.Metadata_Fetcher_Add_32, SmallImage = Resources.Metadata_Fetcher_Add_16, ToolTipText = Msg.Add_ToolTip, GroupCaption = dsGroup });

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
