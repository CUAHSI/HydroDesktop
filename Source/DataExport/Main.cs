using System;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;

namespace HydroDesktop.ExportToCSV
{
    public class Main : Extension
    {
        #region Variables

        //reference to the main application and it's UI items
        public const string TableTabKey = "kHydroTable";
        private string _panelName = "Data Export";
                
        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            App.HeaderControl.RemoveAll();

            base.Deactivate();
        }

        #endregion
		
        #region IPlugin Members

        /// <summary>
        /// Initialize the DotSpatial 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public override void Activate()
        {
            //Add "DataExport" button to the new "Data Export" Panel in "Data" ribbon tab
            var dataExportBtn = new SimpleActionItem("Export", dataExportBtn_Click);
            dataExportBtn.RootKey = TableTabKey;
            dataExportBtn.LargeImage = Properties.Resources.archive;
            dataExportBtn.SmallImage = Properties.Resources.archive_16;
            dataExportBtn.ToolTipText = "Export Time Series Data";
            dataExportBtn.GroupCaption = _panelName;
            App.HeaderControl.Add(dataExportBtn);

            base.Activate();
        }

        #endregion

        #region Event Handlers

        void dataExportBtn_Click(object sender, EventArgs e)
        {
            DbOperations db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);

            var dlg_3 = new ThemeExportDialog(db);
            dlg_3.ShowDialog();
        }

        #endregion
    }
}
