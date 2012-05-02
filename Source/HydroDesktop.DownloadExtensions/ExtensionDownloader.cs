using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Extensions;
using DotSpatial.Controls;
using DotSpatial.Plugins.ExtensionManager;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls.Header;

namespace HydroDesktop.DownloadExtensions
{
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
            bool isDockManagerNeeded = App.CompositionContainer.GetExportedValues<IDockManager>().Count() == 0;
            bool isHeaderControlNeeded = App.CompositionContainer.GetExportedValues<IHeaderControl>().Count() == 0;
            bool isStatusControlNeeded = App.CompositionContainer.GetExportedValues<IStatusControl>().Count() == 0;

            if (isHeaderControlNeeded && isStatusControlNeeded)
            {
                App.UpdateProgress("Downloading a Ribbon extension...");
                packages.Install("DotSpatial.Plugins.Ribbon");
            }

            if (App.GetExtension("DotSpatial.Plugins.MenuBar") == null)
            {
                App.UpdateProgress("Downloading a MenuBar extension...");
                packages.Install("DotSpatial.Plugins.MenuBar");
            }

            //install other packages
            if (App.GetExtension("DotSpatial.Plugins.AttributeDataExplorer") == null)
            {
                App.UpdateProgress("Downloading an AttributeDataExplorer extension...");
                packages.Install("DotSpatial.Plugins.AttributeDataExplorer");
            }

            if (App.GetExtension("DotSpatial.Plugins.WebMap") == null)
            {
                App.UpdateProgress("Downloading a WebMap extension...");
                packages.Install("DotSpatial.Plugins.WebMap");
            }

            App.UpdateProgress("Downloading a ProjectTemplateManager extension...");
            packages.Install("DotSpatial.ProjectTemplate");

            App.UpdateProgress("Downloading a World project template...");
            packages.Install("DotSpatial.ProjectTemplate.World");

            App.UpdateProgress("Downloading a North America project template...");
            packages.Install("DotSpatial.ProjectTemplate.NorthAmerica");

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
