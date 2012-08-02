using System;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Symbology;

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
            SelectionStatusPanel.Width = 250;
            app.ProgressHandler.Add(SelectionStatusPanel);
        

            App.Map.SelectionChanged += Map_SelectionChanged;
            App.Map.MapFrame.LayerSelected +=MapFrame_LayerSelected;

            App.SerializationManager.Deserializing += SerializationManager_Deserializing;
            App.SerializationManager.NewProjectCreated += SerializationManager_NewProjectCreated;
        }

        void SerializationManager_NewProjectCreated(object sender, SerializingEventArgs e)
        {
            App.Map.MapFrame.LayerSelected -= MapFrame_LayerSelected;
            App.Map.MapFrame.LayerSelected += MapFrame_LayerSelected;
        }

        void SerializationManager_Deserializing(object sender, SerializingEventArgs e)
        {
            App.Map.MapFrame.LayerSelected -= MapFrame_LayerSelected;
            App.Map.MapFrame.LayerSelected += MapFrame_LayerSelected;
            UpdateStatusPanel();
        }

        void MapFrame_LayerSelected(object sender, DotSpatial.Symbology.LayerSelectedEventArgs e)
        {
            UpdateStatusPanel();
        }

        void Map_SelectionChanged(object sender, EventArgs e)
        {
            UpdateStatusPanel();
        }

        void UpdateStatusPanel()
        {
            if (App.Map.MapFrame.IsSelected)
            {
                SelectionStatusPanel.Caption = "All Layers Selected";
            }
            else
            {
                var selected = App.Map.Layers.SelectedLayer as IMapFeatureLayer; 
                if (selected != null && selected.Selection != null)
                {
                    var layName = selected.LegendText;
                    SelectionStatusPanel.Caption = String.Format("layer: {0} Selected: {1}", layName, selected.Selection.Count);
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
