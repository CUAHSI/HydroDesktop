#region Namespaces

using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Interfaces.PluginContracts;
using HydroDesktop.Plugins.MetadataFetcher.Forms;

#endregion

namespace HydroDesktop.Plugins.MetadataFetcher
{
    class Main : Extension, IMetadataFetcherPlugin
	{
		#region IExtension Members
		
		public override void Deactivate ()
		{
            App.HeaderControl.RemoveAll();

			//necessary in plugin deactivation
			base.Deactivate ();
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

        public void DownloadMetadata()
        {
            using (var form = new MainForm())
            {
                form.RefreshServiceList();
                form.ShowDialog();
            }
        }

        #endregion
	}
}
