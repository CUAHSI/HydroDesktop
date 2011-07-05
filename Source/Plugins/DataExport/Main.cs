using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;

using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;

using HydroDesktop.Database;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;

namespace HydroDesktop.ExportToCSV
{
    [Plugin("Data Export", Author = "Jingqi Dong", UniqueName = "mw_ExportToCSV_1", Version = "1")]
    public class Main : Extension, IMapPlugin
    {
        #region Variables

        //reference to the main application and it's UI items
        private IMapPluginArgs _mapArgs;

        private string _panelName = "Data Export";

        private RibbonPanel _DataExportPnl;

        private RibbonButton _DataExportBnt;
                
        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        protected override void OnDeactivate()
        {
            _mapArgs.Ribbon.Tabs[1].Panels[1].Items.Remove(_DataExportBnt);
            _mapArgs.Ribbon.Tabs[1].Panels.Remove(_DataExportPnl);

            base.OnDeactivate();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        #endregion
		
        #region IPlugin Members

        /// <summary>
        /// Initialize the DotSpatial 6 plugin
        /// </summary>
        /// <param name="args">The plugin arguments to access the main application</param>
        public void Initialize(IMapPluginArgs args)
        {
            _mapArgs = args;

            _DataExportPnl = new RibbonPanel(_panelName, RibbonPanelFlowDirection.Bottom);
            args.Ribbon.Tabs[1].Panels.Add(_DataExportPnl);
            _DataExportPnl.ButtonMoreVisible = false;
            _DataExportPnl.ButtonMoreEnabled = false;

            //Add "DataExport" button to the new "Data Export" Panel in "Data" ribbon tab
            _DataExportBnt = new RibbonButton("Export");

            _DataExportBnt.Image = Properties.Resources.archive;
			_DataExportBnt.SmallImage = Properties.Resources.archive_16;
            _DataExportBnt.ToolTip = "Export Time Series Data";

            _DataExportBnt.Click += new EventHandler(_DataExportBnt_Click);

            _DataExportPnl.Items.Add(_DataExportBnt);
        }

        #endregion

        #region Event Handlers

        void _DataExportBnt_Click(object sender, EventArgs e)
        {
            DbOperations _db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);

            ThemeExportDialog dlg_3 = new ThemeExportDialog(_db);
            dlg_3.ShowDialog();
        }

        #endregion
    }
}
