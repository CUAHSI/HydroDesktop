using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;
using HydroDesktop.Configuration;
using System.ComponentModel.Composition;
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
        private const string _optionsGroupCaption = "Options";
        private const string _optionsToggleGroupButtonKey = "kTableViewModeOptions";
        private const string _optionsParallelMode = "Parallel";
        private const string _optionsSequenceMode = "Sequence";

        private cTableView tableViewControl;

        private bool ignoreRootSelected = false;

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

            //event for adding the dockable panel
            //App.DockManager.PanelAdded += new EventHandler<DockablePanelEventArgs>(DockManager_PanelAdded);

            #region initialize the Table Ribbon TabPage and related controls

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

            // Options buttons
            var sequenceModeAction = new SimpleActionItem(_optionsSequenceMode, TableViewModeChanged);
            sequenceModeAction.RootKey = kTableView;
            sequenceModeAction.ToolTipText = "Show all fields in sequence";
            sequenceModeAction.LargeImage = Properties.Resources.series_sequence_32;
            sequenceModeAction.SmallImage = Properties.Resources.series_sequence_32;
            sequenceModeAction.GroupCaption = _optionsGroupCaption;
            sequenceModeAction.ToggleGroupKey = _optionsToggleGroupButtonKey;
            App.HeaderControl.Add(sequenceModeAction);

            var parallelModeAction = new SimpleActionItem(_optionsParallelMode, TableViewModeChanged);
            parallelModeAction.RootKey = kTableView;
            parallelModeAction.ToolTipText = "Show just values in parallel";
            parallelModeAction.LargeImage = Properties.Resources.series_parallel_32;
            parallelModeAction.SmallImage = Properties.Resources.series_parallel_32;
            parallelModeAction.GroupCaption = _optionsGroupCaption;
            parallelModeAction.ToggleGroupKey = _optionsToggleGroupButtonKey;
            App.HeaderControl.Add(parallelModeAction);
            //-----
             
            #endregion initialize the Table Ribbon TabPage and related controls

            AddTableViewPanel();

            SeriesControl.Refreshed += SeriesControl_Refreshed;

            //event when ribbon tab is changed
            App.HeaderControl.RootItemSelected += new EventHandler<RootItemEventArgs>(HeaderControl_RootItemSelected);

            base.Activate();
        }

        //when the table root item is selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            //if (ignoreRootSelected) return;
            
            //if (e.SelectedRootKey == kTableView)
            //{
            //    App.DockManager.SelectPanel(kTableView);
            //}
        }

        //void DockManager_PanelAdded(object sender, DockablePanelEventArgs e)
        //{
        //    //the 'Table' dockable panel should follow after 'Map'
        //    if (e.ActivePanelKey == "kMap")
        //        AddTableViewPanel();
        //}

        void AddTableViewPanel()
        {
            // Add "Table View Plugin" dock panel to the SeriesView
            tableViewControl = new cTableView(SeriesControl);
            tableViewControl.Dock = DockStyle.Fill;
            var tableViewPanel = new DockablePanel
            {
                Key = kTableView,
                Caption = _tablePanelName,
                InnerControl = tableViewControl,
                Dock = DockStyle.Fill,
                DefaultSortOrder = 10
            };
            App.DockManager.Add(tableViewPanel);

            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
        }

        private void TableViewModeChanged(object sender, EventArgs e)
        {
            var actionItem = sender as SimpleActionItem;
            if (actionItem == null) return;

            switch(actionItem.Caption)
            {
                case _optionsSequenceMode:
                    tableViewControl.ViewMode = TableViewMode.SequenceView;
                    break;
                case _optionsParallelMode:
                    tableViewControl.ViewMode = TableViewMode.JustValuesInParallel;
                    break;
            }
        }

        void SeriesControl_Refreshed(object sender, EventArgs e)
        {
            RefreshDatabasePath();
        }

        private void Refresh()
        {
            SeriesControl.RefreshSelection();
        }

        void DockManager_ActivePanelChanged(object sender, DotSpatial.Controls.Docking.DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == kTableView)
            {
                App.DockManager.SelectPanel("kHydroSeriesView");
                //ignoreRootSelected = true;
                
                //App.HeaderControl.SelectRoot(kTableView);
                //ignoreRootSelected = false;
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
    }
}
