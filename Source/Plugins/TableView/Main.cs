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

namespace TableView
{
    [Plugin("Table View", Author = "ISU", UniqueName = "TableView_1", Version = "1")]
    public class Main : Extension, IMapPlugin
    {
        #region IMapPlugin Members

        #region Variables

        //this is the reference to the main map and application
        private IMapPluginArgs _mapArgs;

        //the seriesView component
        private ISeriesView _seriesView;

        //this is the tab page which will be added to the main
        //tab control by the table view plug-in
        private RibbonTab _tableViewTabPage ;

        private string _panelName = "Table";

        //private RibbonButton ribbonBnt;

        #endregion

        protected override void OnActivate()
        {
            // This line ensures that "Enabled" is set to true.
            base.OnActivate();
        }

        protected override void OnDeactivate()
        {
            ////remove the plugin panel
            try
            {
                _seriesView.RemovePanel(_panelName);
            }
            catch { }
            // This line ensures that "Enabled" is set to false.
            base.OnDeactivate();
        }

        public void Initialize(IMapPluginArgs args)
        {
            _mapArgs = args;
            IHydroAppManager manager = _mapArgs.AppManager as IHydroAppManager;
            if (manager == null) return;

            _seriesView = manager.SeriesView;


            // Add "Table View Plugin" panel to the SeriesView
            cTableView tableViewControl = new cTableView(_seriesView.SeriesSelector);
            _seriesView.AddPanel(_panelName, tableViewControl);

            // Initialize the Ribbon controls in the "Ribbon" ribbon tab
            //_tableViewTabPage = new RibbonTab(_mapArgs.Ribbon, _panelName);
            _tableViewTabPage = _mapArgs.Ribbon.Tabs[1];

            if (!_mapArgs.Ribbon.Tabs.Contains(_tableViewTabPage))
            {
                _mapArgs.Ribbon.Tabs.Add(_tableViewTabPage);
            }

            _tableViewTabPage.ActiveChanged += new EventHandler(_tableViewTabPage_ActiveChanged);
            
        }

        // When the ribbon tab is changed, the Series view panel is changed
        void _tableViewTabPage_ActiveChanged(object sender, EventArgs e)
        {
            if (_tableViewTabPage.Active)
            {
                if (_mapArgs.PanelManager != null)
                {
                    _mapArgs.PanelManager.SelectedTabName = "Series View";
                    _seriesView.VisiblePanelName = _panelName;
                }
            }
        }
        #endregion

 

    }
}
