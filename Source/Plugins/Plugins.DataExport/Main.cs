using System;
using System.Data;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Symbology;
using HydroDesktop.Common;
using HydroDesktop.Interfaces.PluginContracts;

namespace HydroDesktop.Plugins.DataExport
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
        private readonly string TableTabKey = SharedConstants.TableRootKey;

        /// <summary>
        /// The name of the "Data Export" panel on the table ribbon
        /// </summary>
        private const string _panelName = "Data Export";

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
            var dataExportBtn = new SimpleActionItem("Export", dataExportBtn_Click)
                                    {
                                        RootKey = TableTabKey,
                                        LargeImage = Properties.Resources.archive,
                                        SmallImage = Properties.Resources.archive_16,
                                        ToolTipText = "Export Time Series Data",
                                        GroupCaption = _panelName
                                    };
            App.HeaderControl.Add(dataExportBtn);

            base.Activate();
        }

        #endregion

        #region Event Handlers

        void dataExportBtn_Click(object sender, EventArgs e)
        {
            Export((IFeatureLayer)null);
        }

        #endregion

        #region IDataExportPlugin implementation

        public void Export(IFeatureLayer layer)
        {
            var dialog = new ExportDialog(layer == null? null : new []{layer.LegendText});
            dialog.ShowDialog();
        }

        public void Export(DataTable dataTable)
        {
            var dialog = new ExportDialog(dataTable);
            dialog.ShowDialog();
        }

        #endregion
    }
}
