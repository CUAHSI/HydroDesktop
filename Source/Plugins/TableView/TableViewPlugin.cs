using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;
using HydroDesktop.Configuration;
using System.ComponentModel.Composition;
using HydroDesktop.Controls.Themes;
using TableView.Extensions;
using DotSpatial.Controls.Docking;

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
        private const string kTableView = "kHydroTable";

        //private readonly RootItem _tableViewRoot = new RootItem(kTableView, _tablePanelName);
        private cTableView tableViewControl;

        #endregion

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(kTableView);

            if (SeriesControl != null)
            {
                SeriesControl.Refreshed -= SeriesControl_Refreshed;
            }

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
            
            //Table Tab
            //App.HeaderControl.Add(_tableViewRoot);

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

            // Options
            var dropDownOptions = new DropDownActionItem("kHydroTableViewDropDown", "Mode");
            dropDownOptions.AllowEditingText = false;
            dropDownOptions.Width = 200;
            dropDownOptions.RootKey = kTableView;
            dropDownOptions.GroupCaption = "Options";
            dropDownOptions.Items.Add(new EnumWrapper(TableViewMode.SequenceView));
            dropDownOptions.Items.Add(new EnumWrapper(TableViewMode.JustValuesInParallel));
            dropDownOptions.SelectedValueChanged += dropDown_SelectedValueChanged;
            App.HeaderControl.Add(dropDownOptions);

            #endregion initialize the Table Ribbon TabPage and related controls

            // Add "Table View Plugin" dock panel to the SeriesView
            tableViewControl = new cTableView(SeriesControl);
            tableViewControl.Dock = DockStyle.Fill;
            App.DockManager.Add(new DockablePanel(kTableView, _tablePanelName, tableViewControl, DockStyle.Fill));
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;

            dropDownOptions.SelectedItem = dropDownOptions.Items[0];

            SeriesControl.Refreshed += SeriesControl_Refreshed;

            base.Activate();
        }

        void SeriesControl_Refreshed(object sender, EventArgs e)
        {
            RefreshDatabasePath();
        }

        private void Refresh()
        {
            RefreshAllThemes();
            SeriesControl.RefreshSelection();
        }

        void dropDown_SelectedValueChanged(object sender, SelectedValueChangedEventArgs e)
        {
            if (tableViewControl == null) return;
            tableViewControl.ViewMode = (TableViewMode) ((EnumWrapper) e.SelectedItem).Value;
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.ActivePanelChangedEventArgs e)
        {
            if (e.ActivePanelKey == kTableView)
            {
                App.DockManager.SelectPanel("kHydroSeriesView");
                //App.HeaderControl.SelectRoot(_tableViewRoot);
                RefreshDatabasePath();
            }
        }

        private void RefreshDatabasePath()
        {
            if (tableViewControl != null)
            {
                App.ProgressHandler.Progress(string.Empty, 0, string.Format("Database: {0}", tableViewControl.DatabasePath));
            }
        }

        private void rbRefreshTheme_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void rbDeleteTheme_Click(object sender, EventArgs e)
        {
            DeleteTheme();
        }

        /// <summary>
        /// Reads all themes from the database and displays them on the map
        /// </summary>
        private void RefreshAllThemes()
        {
            var manager = new ThemeManager(Settings.Instance.DataRepositoryConnectionString);
            manager.RefreshAllThemes(App.Map as Map);
        }

        private void DeleteTheme()
        {
            var db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
            using (var frm = new DeleteThemeForm(db))
            {
                if (frm.ShowDialog() != DialogResult.OK) return;
                Refresh();
            }
        }

        #endregion

        #region Database reconfiguration

        private void rbChangeDatabase_Click(object sender, EventArgs e)
        {
            using(var frmChangeDatabase = new ChangeDatabaseForm(SeriesControl, App.Map as Map))
            {
                if (frmChangeDatabase.ShowDialog() != DialogResult.OK) return;
                Refresh();
            }
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
            var saveDialog = new SaveFileDialog {Filter = "SQLite Database|*.sqlite"};
            if (saveDialog.ShowDialog() != DialogResult.OK) return;
            var newDbFileName = saveDialog.FileName;
            try
            {
                if (SQLiteHelper.CreateSQLiteDatabase(newDbFileName))
                {
                    var connString = SQLiteHelper.GetSQLiteConnectionString(newDbFileName);
                    Settings.Instance.DataRepositoryConnectionString = connString;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to create new database." + Environment.NewLine +
                                ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        # endregion


        #region Helpers

        private class EnumWrapper
        {
            public EnumWrapper(Enum value)
            {
                Value = value;
            }

            public Enum Value { get; private set; }

            public override string ToString()
            {
                return Value.Description();
            }
        }

        #endregion
    }
}
