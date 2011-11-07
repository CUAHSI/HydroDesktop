namespace HydroDesktop.Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using System.ComponentModel.Composition;
    using DotSpatial.Controls.Docking;
    using HydroDesktop.Configuration;
    using System.IO;
    using HydroDesktop.Interfaces;
    using HydroDesktop.Database;

    public class HydroDesktopMainPlugin : Extension
    {
        //the seriesView component is the shared HydroDesktop component
        //for database management
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; set; }       

        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            App.ExtensionsActivated += new EventHandler(App_Loaded);

            base.Activate();
        }

        public override void  Deactivate()
        {       
            base.Deactivate();
        }

        //runs when all extensions have been loaded
        void App_Loaded(object sender, EventArgs e)
        {
            //displays the initial welcome screen
            ShowWelcomeScreen();

            //sets-up the database connection and creates a
            //new empty database if required
            SetupDatabases();
        }

        /// <summary>
        /// In the welcome screen, user chooses to create a project from template,
        /// create a new project, or open an existing project
        /// </summary>
        private void ShowWelcomeScreen()
        {
            var welcomeScreen = new WelcomeScreen(App);
            welcomeScreen.StartPosition = FormStartPosition.CenterScreen;
            welcomeScreen.TopMost = true;

            //int x = this.Location.X + this.Width / 2 - _welcomeScreen.Width / 2;
            //int y = this.Location.Y + this.Height / 2 - _welcomeScreen.Height / 2;
            //_welcomeScreen.Location = new System.Drawing.Point(x, y);

            welcomeScreen.Show();

            //if (_welcomeScreen.ShowDialog() == DialogResult.OK)
            //{
            //    bool _isNewProject = _welcomeScreen.NewProjectCreated;
            //}

            //activate the map panel
            App.DockManager.SelectPanel("kMap");
            App.DockManager.SelectPanel("kLegend");
        }

        /// <summary>
        /// This method sets up the default databases.
        /// By default these are created in the temporary directory.
        /// </summary>
        public void SetupDatabases()
        {
            // use the 'default' database path is a temporary db file
            // and only should be used when not working with a project.
            string dataRepositoryTempFile = string.Format("NewProject_{0}_{1}{2}.sqlite",
                DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour, DateTime.Now.Minute);

            string metadataCacheTempFile = string.Format("NewProject_{0}_{1}{2}_cache.sqlite",
               DateTime.Now.Date.ToString("yyyy-MM-dd"), DateTime.Now.Hour, DateTime.Now.Minute);

            string tempDir = Settings.Instance.TempDirectory;
            string dataRepositoryPath = Path.Combine(tempDir, dataRepositoryTempFile);

            string metadataCachePath = Path.Combine(tempDir, metadataCacheTempFile);

            if (HasWriteAccessToFolder(tempDir))
            {
                //create new dataRepositoryDb
                SQLiteHelper.CreateSQLiteDatabase(dataRepositoryPath);
                string conString1 = SQLiteHelper.GetSQLiteConnectionString(dataRepositoryPath);
                Settings.Instance.DataRepositoryConnectionString = conString1;
                Settings.Instance.CurrentProjectFile = Path.ChangeExtension(dataRepositoryPath, ".dspx");

                //create new metadataCacheDb
                SQLiteHelper.CreateMetadataCacheDb(metadataCachePath);
                string conString2 = SQLiteHelper.GetSQLiteConnectionString(metadataCachePath);
                Settings.Instance.MetadataCacheConnectionString = conString2;

                //initialize the Series control (populates its columns)
                //this code will fail if DataRepositoryConnectionString is not set
                this.SeriesControl.SetupDatabase();
            }
            //TODO: find a smart solution when Write access to temp folder is denied
        }

        private bool HasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder.
                // This will raise an exception if the path is read only or do not have access to view the permissions.
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == "kMap")
            {
                App.DockManager.SelectPanel("kLegend");
                App.HeaderControl.SelectRoot(HeaderControl.HomeRootItemKey);
            }
        }
    }
}
