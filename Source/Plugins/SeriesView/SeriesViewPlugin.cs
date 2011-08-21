﻿namespace SeriesView
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

    public class SeriesViewPlugin : Extension
    {
        public const string SeriesViewKey = "kHydroSeriesView";

        [Export("SeriesControl")]
        private SeriesSelector MainSeriesSelector = new SeriesSelector();
        
        public override void Activate()
        {
            //add the series selector
            MainSeriesSelector.Dock = DockStyle.Fill;

            App.DockManager.Add(SeriesViewKey, "time series", MainSeriesSelector, DockStyle.Left);

            
            base.Activate();
        }

        public override void  Deactivate()
        {
 	        App.HeaderControl.RemoveItems();
            App.DockManager.Remove(SeriesViewKey);
            
            base.Deactivate();
        }

        void btn_Click(object sender, EventArgs e) { }
       
    }
}
