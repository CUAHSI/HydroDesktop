namespace SeriesView
{
    using System.Windows.Forms;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using System.ComponentModel.Composition;
    using HydroDesktop.Interfaces;
    using DotSpatial.Controls.Docking;

    public class SeriesViewPlugin : Extension
    {
        private const string SeriesViewKey = "kHydroSeriesView";

        [Export("SeriesControl")]
        private ISeriesSelector MainSeriesSelector = new SeriesSelector();

        private RootItem tableRoot;

        bool firstTimeActivating = true;
        
        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            
            AddSeriesDockPanel();

            tableRoot = new RootItem("kHydroTable", "Table") {SortOrder = 20};
            App.HeaderControl.Add(tableRoot);

            Global.PluginEntryPoint = this;

            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == "kHydroTable")
            {
                App.HeaderControl.SelectRoot("kHydroTable");
            }
        }

        public override void  Deactivate()
        {
 	        App.HeaderControl.RemoveAll();
            App.DockManager.Remove(SeriesViewKey);

            App.DockManager.PanelAdded -= DockManager_PanelAdded;
            App.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;

            Global.PluginEntryPoint = null;
            
            base.Deactivate();
        }


        void DockManager_PanelAdded(object sender, DockablePanelEventArgs e)
        {
            if (!firstTimeActivating) return;
            
            if (e.ActivePanelKey == "kLegend")
            {
                AddSeriesDockPanel();
            }
        }

        void AddSeriesDockPanel()
        {
            //add the series selector
            ((SeriesSelector)MainSeriesSelector).Dock = DockStyle.Fill;

            var timeSeriesPanel = new DockablePanel
            {
                Key = SeriesViewKey,
                Caption = "time series",
                InnerControl = (SeriesSelector)MainSeriesSelector,
                Dock = DockStyle.Left,
                SmallImage = Properties.Resources.timeSeries,
                DefaultSortOrder = 1000
            };

            App.DockManager.Add(timeSeriesPanel);
            firstTimeActivating = false;
        }
    }
}
