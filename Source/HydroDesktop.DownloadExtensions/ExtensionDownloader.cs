using System.ComponentModel.Composition;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Extensions;
using DotSpatial.Plugins.ExtensionManager;

namespace HydroDesktop.DownloadExtensions
{
    /// <summary>
    /// The purpose of this extension is to download the Ribbon, MenuBar,
    /// WebMap and AttributeDataExplorer extensions when the user starts
    /// HydroDesktop for the first time.
    /// </summary>
    public class ExtensionDownloader : ISatisfyImportsExtension
    {
        private readonly Packages packages = new Packages();

        /// <summary>
        /// Gets the AppManager that is responsible for activating and deactivating plugins as well as coordinating
        /// all of the other properties.
        /// </summary>
        [Import]
        private AppManager App { get; set; }

        #region ISatisfyImportsExtension Members

        public void Activate()
        {
            bool isHeaderControlNeeded = App.CompositionContainer.GetExportedValues<IHeaderControl>().Count() == 0;
            bool isStatusControlNeeded = App.CompositionContainer.GetExportedValues<IStatusControl>().Count() == 0;

            //installs the extensions from the online repository on-demand
            //TODO: the list of extensions to install should be in a xml config file instead of hard-coding it
            
            //install ribbon (from DotSpatial MyGet repository)
            if (isHeaderControlNeeded && isStatusControlNeeded)
            {
                App.UpdateProgress("Downloading a Ribbon extension...");
                packages.Install("DotSpatial.Plugins.Ribbon");
            }
            //install menu bar
            if (App.GetExtension("DotSpatial.Plugins.MenuBar") == null)
            {
                App.UpdateProgress("Downloading a MenuBar extension...");
                packages.Install("DotSpatial.Plugins.MenuBar");
            }

            //install attribute data explorer
            if (App.GetExtension("DotSpatial.Plugins.AttributeDataExplorer") == null)
            {
                App.UpdateProgress("Downloading an AttributeDataExplorer extension...");
                packages.Install("DotSpatial.Plugins.AttributeDataExplorer");
            }
            //install web map
            if (App.GetExtension("DotSpatial.Plugins.WebMap") == null)
            {
                App.UpdateProgress("Downloading a WebMap extension...");
                packages.Install("DotSpatial.Plugins.WebMap");
            }

            //install web map
            if (App.GetExtension("GeostatisticalTool") == null)
            {
                App.UpdateProgress("Downloading geostatistical tool...");
                packages.Install("GeostatisticalTool");
            }

            //install North America sample project (this will also install the SampleProjectManager)
            if (App.GetExtension("DotSpatial.SampleProjects.NorthAmerica") == null)
            {
                App.UpdateProgress("Downloading North America project template...");
                packages.Install("DotSpatial.SampleProjects.NorthAmerica");
            }
            //install the World sample project
            if (App.GetExtension("DotSpatial.SampleProjects.World") == null)
            {
                App.UpdateProgress("Downloading World project template...");
                packages.Install("DotSpatial.SampleProjects.World");
            }

            App.RefreshExtensions();
        }

        public int Priority
        {
            get
            {
                return 1;
            }
        }

        #endregion
    }
}
