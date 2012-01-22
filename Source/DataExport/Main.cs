using System;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using HydroDesktop.Database;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using DotSpatial.Controls.Header;

namespace HydroDesktop.ExportToCSV
{
    /// <summary>
    /// The main data export plugin class
    /// </summary>
    public class Main : Extension, IDataExportPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        /// <summary>
        /// The key of the "Table" ribbon tab
        /// </summary>
        public const string TableTabKey = "kHydroTable";
        /// <summary>
        /// The name of the "Data Export" panel on the table ribbon
        /// </summary>
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
        /// activate the data export plugin
        /// </summary>
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
            Export(null);
        }

        #endregion

        /// <summary>
        /// starts the data export of all series from the layer
        /// </summary>
        /// <param name="layer">the feature layer from which to export data</param>
        public void Export(IFeatureLayer layer)
        {
            var db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
            var dlg_3 = new ThemeExportDialog(db, layer == null? null : new []{layer.LegendText});
            dlg_3.ShowDialog();
        }
    }
}
