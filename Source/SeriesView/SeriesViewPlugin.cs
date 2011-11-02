namespace SeriesView
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;

    using DotSpatial.Controls;
    using DotSpatial.Controls.Header;
    using System.ComponentModel.Composition;
    using HydroDesktop.Interfaces;
    using DotSpatial.Controls.Docking;

    public class SeriesViewPlugin : Extension
    {
        public const string SeriesViewKey = "kHydroSeriesView";

        [Export("SeriesControl")]
        private ISeriesSelector MainSeriesSelector = new SeriesSelector();

        private RootItem tableRoot = null;

        bool firstTimeActivating = true;
        
        public override void Activate()
        {
            App.DockManager.ActivePanelChanged += new EventHandler<DockablePanelEventArgs>(DockManager_ActivePanelChanged);
            
            //event handler for adding the series selector
            //if (firstTimeActivating)
            //{
            //    App.DockManager.PanelAdded += new EventHandler<DockablePanelEventArgs>(DockManager_PanelAdded);
            //}
            //else
            //{
                AddSeriesDockPanel();
            //}

            tableRoot = new RootItem("kHydroTable", "Table");
            tableRoot.SortOrder = 20;
            App.HeaderControl.Add(tableRoot);

            base.Activate();
        }

        void DockManager_ActivePanelChanged(object sender, DockablePanelEventArgs e)
        {
            if (e.ActivePanelKey == "kHydroTable")
            {
                tableRoot.Visible = true;
            }
            else if (e.ActivePanelKey != "kHydroSeriesView")
            {
                tableRoot.Visible = false;
            }
        }

        public override void  Deactivate()
        {
 	        App.HeaderControl.RemoveAll();
            App.DockManager.Remove(SeriesViewKey);

            App.DockManager.PanelAdded -= DockManager_PanelAdded;
            App.DockManager.ActivePanelChanged -= DockManager_ActivePanelChanged;
            
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
                SmallImage = Properties.Resources.timeSeries
            };

            App.DockManager.Add(timeSeriesPanel);
            firstTimeActivating = false;
        }
    }
}
