using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;
using System.ComponentModel.Composition;
using DotSpatial.Controls.Docking;
using HydroDesktop.Common.Tools;

namespace TableView
{
    public class TableViewPlugin : Extension
    {
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
        private TextEntryActionItem dbTextBox;

        #endregion


        #region Extension Members

        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();
            App.DockManager.Remove(kTableView);
            
            App.HeaderControl.RootItemSelected -= HeaderControl_RootItemSelected;

            tableViewControl = null;
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
            var refreshThemeButton = new SimpleActionItem("Refresh", rbRefreshTheme_Click)
                                         {
                                             RootKey = kTableView,
                                             LargeImage = Properties.Resources.refreshTheme,
                                             SmallImage = Properties.Resources.refreshTheme_16x16,
                                             ToolTipText = "Refresh Themes",
                                             GroupCaption = _tablePanelName
                                         };
            App.HeaderControl.Add(refreshThemeButton);

            //DeleteTheme
            var deleteThemeButton = new SimpleActionItem("Delete", rbDeleteTheme_Click)
                                        {
                                            RootKey = kTableView,
                                            LargeImage = Properties.Resources.delete,
                                            SmallImage = Properties.Resources.delete_16x16,
                                            ToolTipText = "Delete Theme from Database",
                                            GroupCaption = _tablePanelName
                                        };

            App.HeaderControl.Add(deleteThemeButton);

            //Current database
            dbTextBox = new TextEntryActionItem
                            {
                                Caption = "",
                                GroupCaption = "Current Database Path",
                                RootKey = kTableView,
                                Width = 300,
                                ToolTipText = "Path to current database"
                            };
            dbTextBox.PropertyChanged += dbTextBox_PropertyChanged;
            App.HeaderControl.Add(dbTextBox);

            //Change Database
            var changeDatabaseButton = new SimpleActionItem("Change", rbChangeDatabase_Click)
                                           {
                                               RootKey = kTableView,
                                               ToolTipText = "Change Database",
                                               LargeImage = Properties.Resources.changeDatabase,
                                               SmallImage = Properties.Resources.changeDatabase_16x16,
                                               GroupCaption = "Current Database Path"
                                           };
            App.HeaderControl.Add(changeDatabaseButton);

            AddTableViewPanel();

            // Options buttons
            var sequenceModeAction = new SimpleActionItem(_optionsSequenceMode, TableViewModeChanged)
                                         {
                                             RootKey = kTableView,
                                             ToolTipText = "Show all fields in sequence",
                                             LargeImage = Properties.Resources.series_sequence_32,
                                             SmallImage = Properties.Resources.series_sequence_32,
                                             GroupCaption = _optionsGroupCaption,
                                             ToggleGroupKey = _optionsToggleGroupButtonKey
                                         };
            App.HeaderControl.Add(sequenceModeAction);

            var parallelModeAction = new SimpleActionItem(_optionsParallelMode, TableViewModeChanged)
                                         {
                                             RootKey = kTableView,
                                             ToolTipText = "Show just values in parallel",
                                             LargeImage = Properties.Resources.series_parallel_32,
                                             SmallImage = Properties.Resources.series_parallel_32,
                                             GroupCaption = _optionsGroupCaption,
                                             ToggleGroupKey = _optionsToggleGroupButtonKey
                                         };
            App.HeaderControl.Add(parallelModeAction);

            parallelModeAction.Toggling += TableViewModeChanged;
            parallelModeAction.Toggle();

            //-----
             
            #endregion initialize the Table Ribbon TabPage and related controls

            SeriesControl.Refreshed += SeriesControl_Refreshed;

            //event when ribbon tab is changed
            App.HeaderControl.RootItemSelected += HeaderControl_RootItemSelected;

            base.Activate();
        }

        void dbTextBox_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        //when the table root item is selected
        void HeaderControl_RootItemSelected(object sender, RootItemEventArgs e)
        {
            if (e.SelectedRootKey == kTableView)
            {
                App.DockManager.SelectPanel(kTableView);
            }
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
            tableViewControl = new cTableView(SeriesControl) {Dock = DockStyle.Fill};
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

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == kTableView)
            {
                App.DockManager.SelectPanel("kHydroSeriesView");
                App.HeaderControl.SelectRoot(kTableView);
                RefreshDatabasePath();
            }
        }

        private void RefreshDatabasePath()
        {
            if (tableViewControl != null)
            {
                //App.ProgressHandler.Progress(string.Empty, 0, string.Format("Database: {0}", tableViewControl.DatabasePath));
                dbTextBox.Text = tableViewControl.DatabasePath;
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
            using (var frm = new DeleteThemeForm())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Refresh();
                }
            }
        }

        #endregion

        #region Database reconfiguration

        private void rbChangeDatabase_Click(object sender, EventArgs e)
        {
            using (var frmChangeDatabase = new ChangeDatabaseForm(App.Map as Map, App.GetExtension<ISearchPlugin>()))
            {
                frmChangeDatabase.ShowDialog();
            }
        }

        # endregion
    }
}
