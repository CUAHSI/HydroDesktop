using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Docking;

namespace HydroDesktop.MainApplication
{
    /// <summary>
    /// A Form to test the map controls.
    /// </summary>
    internal partial class MainForm : Form
    {
        // the main form is exported so that the IHeaderControl plug-in can add the menu or the
        // ribbon toolbar to it.
        [Export("Shell", typeof(ContainerControl))]
        private static ContainerControl Shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            //It looks better if we Maximize the main window, 
            //but the Form.Owner property doesn't seem to do anything when running on Mono, so for now only maximize if on Windows.
            this.WindowState = FormWindowState.Maximized;
            appManager = new AppManager();
            appManager.Map = new Map();
            LoadCustomBranding(Properties.Settings.Default);

            appManager.SatisfyImportsExtensionsActivated +=
                delegate
                {
                    appManager.DockManager.Add(new DockablePanel("kMap", "Map", (Map)appManager.Map, DockStyle.Fill) { SmallImage = Properties.Resources.map_16x16 });
                };

            Shell = this;
            appManager.LoadExtensions();
            // Hack: HydroDockManager need to know time when all plugins are activated to save initial layout of dock-panels
            // (this need to working of HydroDockManager.ResetLayout() method).
            // appManager.ExtensionsActivated is not suitable, because plugins also can change dock-panels there.
            // To avoid direct referencing from MainForm to HydroDockManager (or vice versa), 
            // we just send needed info through Add() method.
            appManager.DockManager.Add(new DockablePanel { Key = "save_snapshot" });

            appManager.ProgressHandler.Progress("", 0, "Go to the extension manager to find additional extensions!");
        }

        private void LoadCustomBranding(Properties.Settings settings)
        {
            if (!String.IsNullOrWhiteSpace(settings.CustomAppIconPath) &&
                System.IO.File.Exists(settings.CustomAppIconPath))
            {
                var ico = new System.Drawing.Icon(settings.CustomAppIconPath);
                this.Icon = ico;
            }

            if (!String.IsNullOrWhiteSpace(settings.CustomMainFormTitle))
            {
                Text = settings.CustomMainFormTitle;
            }
        }

        /// <summary>
        /// Gets or sets the appManager
        /// </summary>
        public AppManager appManager { get; set; }

        //Added this event to prevent so white line fading that would happen when I would move the the legend in the MainForm off the edge of the
        //screen.  I am still confused as to why it was happening, but this locationChanged event seems to fix the problem just fine by constantly 
        //redrawing the legend when the form is moving.
        private void MainForm_LocationChanged(object sender, EventArgs e)
        {
            if (appManager.Map != null && appManager.Map.Legend != null)
            {
               // appManager.Map.Legend.RefreshNodes();
            }
        }    
    }
}