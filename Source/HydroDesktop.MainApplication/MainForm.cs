using System;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using DotSpatial.Controls.Docking;
using DotSpatial.Controls;

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
            appManager = new AppManager();
            appManager.Map = new Map();
            appManager.Legend = new Legend();
            LoadCustomBranding(HydroDesktop.MainApplication.Properties.Settings.Default);

            appManager.SatisfyImportsExtensionsActivated +=
                delegate(object sender, EventArgs e)
                {
                    // we use this event to ensure that legend and map dockable panels are
                    // added to the DockManager before any other dockable panels.
                    this.appManager.DockManager.Add(new DockablePanel("kLegend", "Legend", (Legend)appManager.Legend, DockStyle.Left) { SmallImage = HydroDesktop.MainApplication.Properties.Resources.legend_16x16 });
                    this.appManager.DockManager.Add(new DockablePanel("kMap", "Map", (Map)appManager.Map, DockStyle.Fill) { SmallImage = HydroDesktop.MainApplication.Properties.Resources.map_16x16 });
                };

            Shell = this;
            appManager.LoadExtensions();

            this.appManager.ProgressHandler.Progress("", 0, "Go to the extension manager to find additional extensions!");
        }

        private void LoadCustomBranding(HydroDesktop.MainApplication.Properties.Settings settings)
        {
            if (!String.IsNullOrWhiteSpace(settings.CustomAppIconPath) &&
                System.IO.File.Exists(settings.CustomAppIconPath))
            {
                System.Drawing.Icon ico = new System.Drawing.Icon(settings.CustomAppIconPath);
                this.Icon = ico;
            }

            if (!String.IsNullOrWhiteSpace(settings.CustomMainFormTitle))
            {
                this.Text = settings.CustomMainFormTitle;
            }
        }

        /// <summary>
        /// Gets or sets the appManager
        /// </summary>
        public AppManager appManager { get; set; }

       
    }
}