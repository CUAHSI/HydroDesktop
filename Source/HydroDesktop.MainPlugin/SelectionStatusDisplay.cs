using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;

namespace HydroDesktop.Main
{
    /// <summary>
    /// Displays info about active layer and about number of selected features
    /// </summary>
    public class SelectionStatusDisplay
    {
        public SelectionStatusDisplay(AppManager app)
        {
            App = app;

            SelectionStatusPanel = new StatusPanel();
            SelectionStatusPanel.Width = 400;
            app.ProgressHandler.Add(SelectionStatusPanel);

            App.Map.SelectionChanged += new EventHandler(Map_SelectionChanged);
        }

        void Map_SelectionChanged(object sender, EventArgs e)
        {
            if (App.Map.MapFrame.IsSelected)
            {
                SelectionStatusPanel.Caption = "All Layers Selected";
            }
            else
            {
                if (App.Map.Layers.SelectedLayer != null)
                {
                    string layName = App.Map.Layers.SelectedLayer.LegendText;
                    IMapFeatureLayer mfl = App.Map.Layers.SelectedLayer as IMapFeatureLayer;
                    if (mfl != null)
                    {
                        SelectionStatusPanel.Caption = String.Format("layer: {0} Selected: {1}", layName, mfl.Selection.Count);
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the AppManager
        /// </summary>
        public AppManager App { get; set; }

        /// <summary>
        /// Gets or sets the selection status panel
        /// </summary>
        public StatusPanel SelectionStatusPanel { get; set; }


    }
}
