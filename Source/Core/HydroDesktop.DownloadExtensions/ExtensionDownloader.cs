using System.ComponentModel.Composition;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Extensions;
using DotSpatial.Plugins.ExtensionManager;
using System;
using System.Diagnostics;
using System.Threading;

namespace HydroDesktop.DownloadExtensions
{
    /// <summary>
    /// The purpose of this extension is to download the Ribbon, MenuBar and
    /// WebMap extensions when the user starts
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

        private string message = null;

        #region ISatisfyImportsExtension Members

        public void Activate()
        {
            //bool isHeaderControlNeeded = !App.CompositionContainer.GetExportedValues<IHeaderControl>().Any();
            //bool isStatusControlNeeded = !App.CompositionContainer.GetExportedValues<IStatusControl>().Any();

            ////installs the extensions from the online repository on-demand
            ////note: some of these packages will be shipped with the installer but they are installed from online
            ////when running from Visual Studio solution (direct reference in Visual Studio is not allowed by the license of Ribbon)

            ////install the ribbon (must be downloaded first)
            //if (isHeaderControlNeeded && isStatusControlNeeded)
            //{
            //    App.UpdateProgress("Downloading a Ribbon extension...");
            //    packages.Install("DotSpatial.Plugins.Ribbon");
            //}

            //foreach (string package in Properties.Settings.Default.ExternalExtensions)
            //{
            //    if (App.GetExtension(package) == null)
            //    {
            //        App.UpdateProgress("Downloading " + package + " extension...");
            //        packages.Install(package);
            //    }
            //}

            ////special case: download the WebMap plugin with BruTile
            //if (App.GetExtension("DotSpatial.Plugins.WebMap") == null)
            //{
            //    App.UpdateProgress("Downloading DotSpatial.Plugins.WebMap extension...");
            //    packages.Install("BruTile");
            //    packages.Install("DotSpatial.Plugins.WebMap");
            //}
            ////special case: download EPADelineation with Newtonsoft.Json
            //if (App.GetExtension("EPADelineation") == null)
            //{
            //    App.UpdateProgress("Downloading EPADelineation extension...");
            //    packages.Install("Newtonsoft.Json");
            //    packages.Install("EPADelineation");
            //}
           /* Thread updateThread = new Thread(() => InstallSampleProjects());
            updateThread.Start();

            //Update splash screen's progress bar while thread is active.
            while (updateThread.IsAlive)
            {
                if(message != null)
                    App.UpdateProgress(message);
            }
            updateThread.Join(100);
            */
            //App.RefreshExtensions();
        }

        private void InstallSampleProjects()
        {
            foreach (string sampleProject in Properties.Settings.Default.ExternalSampleProjects)
            {
                if (!SampleProjectFinder.IsSampleProjectInstalled(sampleProject))
                {
                    try
                    {
                        message = "Downloading " + sampleProject + " sample project...";
                        packages.Install(sampleProject);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.StackTrace); ;
                    }
                }
            }
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
