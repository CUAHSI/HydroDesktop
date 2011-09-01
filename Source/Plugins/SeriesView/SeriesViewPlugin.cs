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
        
        public override void Activate()
        {
            //add the series selector
            ((SeriesSelector)MainSeriesSelector).Dock = DockStyle.Fill;

            App.DockManager.Add(new DockablePanel(SeriesViewKey, "time series", (SeriesSelector)MainSeriesSelector, DockStyle.Left));

            base.Activate();
        }

        public override void  Deactivate()
        {
 	        App.HeaderControl.RemoveAll();
            App.DockManager.Remove(SeriesViewKey);
            
            base.Deactivate();
        }
    }
}
