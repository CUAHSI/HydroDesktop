using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;
using HydroDesktop.Configuration;
using System.ComponentModel.Composition;
using HydroDesktop.Controls.Themes;

namespace TableView
{
    public class TableViewPlugin : Extension
    {
        #region IMapPlugin Members

        #region Variables

        //the seriesView component
        [Import("SeriesControl", typeof(ISeriesSelector))]
        internal ISeriesSelector SeriesControl { get; set; }

        private const string _tablePanelName = "Table";
        private const string kTableView = "kHydroTableView";

        private RootItem _tableViewRoot = new RootItem(kTableView, _tablePanelName);

        #endregion

        public override void Deactivate()
        {
            App.HeaderControl.RemoveItems();

            App.DockManager.Remove(kTableView);

            base.Deactivate();
        }

        public override void Activate()
        {
            if (SeriesControl == null)
            {
                MessageBox.Show("Cannot activate the TableView plugin. SeriesView not found.");
                return;
            }

            #region initialize the Table Ribbon TabPage and related controls

            IHeaderControl header = App.HeaderControl;
            
            //Table Tab
            App.HeaderControl.Add(_tableViewRoot);

            //RefreshTheme
            var refreshThemeButton = new SimpleActionItem("Refresh", rbRefreshTheme_Click);
            refreshThemeButton.RootKey = kTableView;
            refreshThemeButton.LargeImage = Properties.Resources.refreshTheme;
            refreshThemeButton.SmallImage = Properties.Resources.refreshTheme_16x16;
            refreshThemeButton.ToolTipText = "Refresh Themes";
            refreshThemeButton.GroupCaption = _tablePanelName;
            App.HeaderControl.Add(refreshThemeButton);

            //DeleteTheme
            var deleteThemeButton = new SimpleActionItem("Delete", rbDeleteTheme_Click);
            deleteThemeButton.RootKey = kTableView;        
            deleteThemeButton.LargeImage = Properties.Resources.delete;
            deleteThemeButton.SmallImage = Properties.Resources.delete_16x16;
            deleteThemeButton.ToolTipText = "Delete Theme from Database";
            deleteThemeButton.GroupCaption = _tablePanelName;
            App.HeaderControl.Add(deleteThemeButton);

            //Change Database
            var changeDatabaseButton = new SimpleActionItem("Change", rbChangeDatabase_Click);
            changeDatabaseButton.RootKey = kTableView;
            changeDatabaseButton.ToolTipText = "Change Database";
            changeDatabaseButton.LargeImage = Properties.Resources.changeDatabase;
            changeDatabaseButton.SmallImage = Properties.Resources.changeDatabase_16x16;
            changeDatabaseButton.GroupCaption = "Database";
            App.HeaderControl.Add(changeDatabaseButton);

            //New Database
            var newDatabaseButton = new SimpleActionItem("New", rbNewDatabase_Click);
            newDatabaseButton.RootKey = kTableView;
            newDatabaseButton.ToolTipText = "Create New Database";
            newDatabaseButton.LargeImage = Properties.Resources.newDatabase;
            newDatabaseButton.SmallImage = Properties.Resources.newDatabase_16x16;
            newDatabaseButton.GroupCaption = "Database";
            App.HeaderControl.Add(newDatabaseButton);

            #endregion initialize the Table Ribbon TabPage and related controls

            // Add "Table View Plugin" dock panel to the SeriesView
            cTableView tableViewControl = new cTableView(SeriesControl);
            tableViewControl.Dock = DockStyle.Fill;
            App.DockManager.Add(kTableView, _tablePanelName, tableViewControl, DockStyle.Fill);
            App.DockManager.ActivePanelChanged += new EventHandler<DotSpatial.Controls.Docking.ActivePanelChangedEventArgs>(DockManager_ActivePanelChanged);

            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.ActivePanelChangedEventArgs e)
        {
            if (e.ActivePanelKey == kTableView)
            {
                App.DockManager.SelectPanel("kHydroSeriesView");
                App.HeaderControl.SelectRoot(_tableViewRoot);
            }
        }

        private void rbRefreshTheme_Click(object sender, EventArgs e)
        {
            RefreshAllThemes();
            SeriesControl.RefreshSelection();
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
                SeriesControl.RefreshSelection();
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
            ChangeDatabaseForm frmChangeDatabase = new ChangeDatabaseForm(SeriesControl, App.Map as Map);
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
            SeriesControl.RefreshSelection();

            // Originally from NewDatabase
            Settings.Instance.DataRepositoryConnectionString = connString;

            RefreshAllThemes();
        }

        # endregion

    }
}
