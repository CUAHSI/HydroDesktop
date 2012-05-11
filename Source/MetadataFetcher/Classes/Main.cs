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
		
		public override void Deactivate ()
		{
            App.ExtensionsActivated -= AppOnExtensionsActivated;
            App.HeaderControl.RemoveAll();

			//necessary in plugin deactivation
			base.Deactivate ();
		}
		
		public override void Activate ()
		{
            base.Activate();

            App.ExtensionsActivated += AppOnExtensionsActivated;
		}

        #endregion

		#region Event Handlers

        private void AppOnExtensionsActivated(object sender, EventArgs e)
        {
            if (App.GetExtension("Search3") != null)
            {
                var rootKey = SharedConstants.SearchRootkey;
                var dsGroup = SharedConstants.SearchDataSourcesGroupName;
                var header = App.HeaderControl;

                header.Add(new SimpleActionItem(rootKey, Msg.Manage, mnuDownloadMetadata_Click){LargeImage = Resources.Metadata_Fetcher_32,SmallImage = Resources.Metadata_Fetcher_16,ToolTipText = Msg.Manage_ToolTip, GroupCaption = dsGroup});
                header.Add(new SimpleActionItem(rootKey, Msg.Add, mnuAddServices_Click){LargeImage = Resources.Metadata_Fetcher_Add_32,SmallImage = Resources.Metadata_Fetcher_Add_16,ToolTipText = Msg.Add_ToolTip, GroupCaption = dsGroup});
            }
        }

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
