using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Controls;
using DotSpatial.Controls.Header;
using HydroDesktop.Controls.Themes;
using HydroDesktop.Configuration;

namespace TableView
{
    public class Main : Extension
    {
        #region IMapPlugin Members

        #region Variables

        //the seriesView component
        private ISeriesView _seriesView;

        //this is the tab page which will be added to the main
        //tab control by the table view plug-in
        private RibbonTab _tableViewTabPage ;

        private const string _tablePanelName = "Table";

        public const string TableTabKey = "kHydroTable";

        //private RibbonButton ribbonBnt;

        #endregion

        public override void Deactivate()
        {
            ////remove the plugin panel
            try
            {
                _seriesView.RemovePanel(_tablePanelName);
                App.HeaderControl.RemoveItems();
            }
            catch { }
            // This line ensures that "Enabled" is set to false.
            base.Deactivate();
        }

        public override void Activate()
        {
            HydroAppManager manager = App as HydroAppManager;
            if (manager == null) return;

            _seriesView = manager.SeriesView;

            #region initialize the Table Ribbon TabPage and related controls

            IHeaderControl header = manager.HeaderControl;
            
            //Table Tab
            App.HeaderControl.Add(new RootItem(TableTabKey, _tablePanelName));
            
            //Workaround - when the selected ribbon tab is changed
            App.Ribbon.ActiveTabChanged += new EventHandler(Ribbon_ActiveTabChanged);


            //RefreshTheme
            var refreshThemeButton = new SimpleActionItem("Refresh", rbRefreshTheme_Click);
            refreshThemeButton.RootKey = TableTabKey;
            refreshThemeButton.LargeImage = Properties.Resources.refreshTheme;
            refreshThemeButton.SmallImage = Properties.Resources.refreshTheme_16x16;
            refreshThemeButton.ToolTipText = "Refresh Themes";
            refreshThemeButton.GroupCaption = _tablePanelName;
            App.HeaderControl.Add(refreshThemeButton);

            //DeleteTheme
            var deleteThemeButton = new SimpleActionItem("Delete", rbDeleteTheme_Click);
            deleteThemeButton.RootKey = TableTabKey;        
            deleteThemeButton.LargeImage = Properties.Resources.delete;
            deleteThemeButton.SmallImage = Properties.Resources.delete_16x16;
            deleteThemeButton.ToolTipText = "Delete Theme from Database";
            deleteThemeButton.GroupCaption = _tablePanelName;
            App.HeaderControl.Add(deleteThemeButton);

            //Change Database
            var changeDatabaseButton = new SimpleActionItem("Change", rbChangeDatabase_Click);
            changeDatabaseButton.RootKey = TableTabKey;
            changeDatabaseButton.ToolTipText = "Change Database";
            changeDatabaseButton.LargeImage = Properties.Resources.changeDatabase;
            changeDatabaseButton.SmallImage = Properties.Resources.changeDatabase_16x16;
            changeDatabaseButton.GroupCaption = "Database";
            App.HeaderControl.Add(changeDatabaseButton);

            //New Database
            var newDatabaseButton = new SimpleActionItem("New", rbNewDatabase_Click);
            newDatabaseButton.RootKey = TableTabKey;
            newDatabaseButton.ToolTipText = "Create New Database";
            newDatabaseButton.LargeImage = Properties.Resources.newDatabase;
            newDatabaseButton.SmallImage = Properties.Resources.newDatabase_16x16;
            newDatabaseButton.GroupCaption = "Database";
            App.HeaderControl.Add(newDatabaseButton);

            #endregion initialize the Table Ribbon TabPage and related controls

            // Add "Table View Plugin" panel to the SeriesView
            cTableView tableViewControl = new cTableView(_seriesView.SeriesSelector);
            _seriesView.AddPanel(_tablePanelName, tableViewControl);

            base.Activate();
        }

        //workaround method - changing the ribbon tab changes the main content
        void Ribbon_ActiveTabChanged(object sender, EventArgs e)
        {
            RibbonTab myTab = App.Ribbon.Tabs.Find(t => t.Text == _tablePanelName);
            
            if (myTab.Active)
            {
                if (App.TabManager != null)
                {
                    App.TabManager.SelectedTabName = "Series View";
                    _seriesView.VisiblePanelName = _tablePanelName;
                }
            }
        }

        private void rbRefreshTheme_Click(object sender, EventArgs e)
        {
            RefreshAllThemes();
            _seriesView.SeriesSelector.RefreshSelection();
            //this.tabContainer.SelectedIndex = 1;
        }

        private void rbDeleteTheme_Click(object sender, EventArgs e)
        {
            DeleteTheme();
        }

        /// <summary>
        /// Reads all themes from the database and displays them on the map
        /// </summary>
        public void RefreshAllThemes()
        {
            ThemeManager manager = new ThemeManager(Settings.Instance.DataRepositoryConnectionString);
            manager.RefreshAllThemes(App.Map as Map);
        }

        /// <summary>
        /// Delete the theme and all related records in the database.
        /// </summar y>
        /// <param name="themeId"></param>
        private void DeleteTheme()
        {
            DbOperations db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
            DeleteThemeForm frm = new DeleteThemeForm(db);
            frm.ShowDialog();
            if (frm.DialogResult == DialogResult.OK)
            {
                _seriesView.SeriesSelector.RefreshSelection();
                RefreshAllThemes();
            }
        }

        #endregion

        #region Database reconfiguration

        private void rbChangeDatabase_Click(object sender, EventArgs e)
        {
            ChangeDatabase();
            RefreshAllThemes();
        }

        /// <summary>
        /// Change the default database used by HydroDesktop
        /// </summary>
        /// <returns></returns>
        private void ChangeDatabase()
        {
            ChangeDatabaseForm frmChangeDatabase = new ChangeDatabaseForm(App as IHydroAppManager);
            //frmChangeDatabase.Owner = this;
            frmChangeDatabase.ShowDialog();
        }

        private void rbNewDatabase_Click(object sender, EventArgs e)
        {
            CreateNewDatabase();
        }

        /// <summary>
        /// Creates a new database to be used by the HydroDesktop application
        /// </summary>
        private void CreateNewDatabase()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "SQLite Database|*.sqlite";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string newDbFileName = saveDialog.FileName;
                try
                {
                    if (SQLiteHelper.CreateSQLiteDatabase(newDbFileName))
                    {
                        string connString = SQLiteHelper.GetSQLiteConnectionString(newDbFileName);
                        DatabaseHasChanged(connString);

                        MessageBox.Show("New database has been created successfully.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unable to create new database. " +
                        ex.Message);
                }
            }
        }

        /// <summary>
        /// When setting up a new database, this reconfigures the managers
        /// </summary>
        /// <param name="connString"></param>
        private void DatabaseHasChanged(string connString)
        {
            //TODO call SeriesSelector directly
            _seriesView.SeriesSelector.SetupDatabase();

            // Originally from NewDatabase
            Settings.Instance.DataRepositoryConnectionString = connString;

            RefreshAllThemes();
        }

        # endregion

    }
}
