namespace HydroDesktop.Plugins.SeriesView
{
    using HydroDesktop.Common;
    using System.ComponentModel.Composition;
    using System.Windows.Forms;
    using DotSpatial.Controls;
    using DotSpatial.Controls.Docking;
    using DotSpatial.Controls.Header;
    using HydroDesktop.Interfaces;

    public class SeriesViewPlugin : Extension
    {
        private readonly string SeriesViewKey = SharedConstants.SeriesViewKey;
        private readonly string _tableRootKey = SharedConstants.TableRootKey;

        [Export("SeriesControl")]
        private ISeriesSelector MainSeriesSelector = new SeriesSelector();

        public SeriesViewPlugin()
        {
            DeactivationAllowed = false;
        }

        public override void Activate()
        {
            ((SeriesSelector) MainSeriesSelector).ParentPlugin = this;

            //add the series selector
            App.DockManager.ActivePanelChanged += DockManager_ActivePanelChanged;
            ((SeriesSelector) MainSeriesSelector).Dock = DockStyle.Fill;
            var timeSeriesPanel = new DockablePanel
                {
                    Key = SeriesViewKey,
                    Caption = "time series",
                    InnerControl = (SeriesSelector) MainSeriesSelector,
                    Dock = DockStyle.Left,
                    SmallImage = Properties.Resources.timeSeries,
                    DefaultSortOrder = 1000
                };
            App.DockManager.Add(timeSeriesPanel);

            App.HeaderControl.Add(new RootItem(_tableRootKey, "Table") {SortOrder = 20});

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
            
            base.Deactivate();
        }
    }
}
