using HydroDesktop.Common;

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
        private readonly string SeriesViewKey = SharedConstants.SeriesViewKey;
        private readonly string _tableRootKey = SharedConstants.TableRootKey;

        [Export("SeriesControl")]
        private ISeriesSelector MainSeriesSelector = new SeriesSelector();
        
        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            
            AddSeriesDockPanel();
            App.HeaderControl.Add(new RootItem(_tableRootKey, "Table") { SortOrder = 20 });

            Global.PluginEntryPoint = this;

            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == _tableRootKey)
            {
                App.HeaderControl.SelectRoot(_tableRootKey);
            }
        }

        public override void  Deactivate()
        {
 	        App.HeaderControl.RemoveAll();
            App.DockManager.Remove(SeriesViewKey);
            
            App.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;

            Global.PluginEntryPoint = null;
            
            base.Deactivate();
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
        }
    }
}
