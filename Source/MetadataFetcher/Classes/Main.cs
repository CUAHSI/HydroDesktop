#region Namespaces
using System;
using HydroDesktop.Common;
using HydroDesktop.Interfaces.PluginContracts;
using HydroDesktop.MetadataFetcher.Forms;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.MetadataFetcher.Properties;
using Msg = HydroDesktop.MetadataFetcher.MessageStrings;

#endregion

namespace HydroDesktop.MetadataFetcher
{
    class Main : Extension, IMetadataFetcherPlugin
	{
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
		    DownloadMetadata();
		}

		void mnuAddServices_Click ( object sender, EventArgs e )
		{
		    AddServices();
		}

        private void DownloadMetadata()
        {
            using (var form = new MainForm())
            {
                form.RefreshServiceList();
                form.ShowDialog();
            }
        }

        #endregion

        #region Implementation of IMetadataFetcherPlugin

        public void AddServices()
        {
            using (var form = new AddServicesForm())
            {
                var dialogResult = form.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    DownloadMetadata();
                }   
            }
        }

        #endregion
	}
}
