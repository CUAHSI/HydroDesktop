using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using ElevationGraph.Properties;

namespace ElevationGraph
{
    public class ElevationGraphPlugin : Extension
    {
		#region Fields
		private LineDrawing lineDrawing;
		private SimpleActionItem newButton;
		private bool active = false;
		
		#endregion Fields

		#region Plugin operations

		//Activates the plugin
		public override void Activate() 
		{
			// Initialize lineDrawing object
			lineDrawing = new LineDrawing((Map)App.Map);

			// Create and add a button
			newButton = new SimpleActionItem("Elevation Graph", buttonClick);
			newButton.RootKey = HeaderControl.HomeRootItemKey;
			newButton.ToolTipText = "Draw a line on the map to create an elevation cross" + 
				" section graph.  Right-click or double-click to stop drawing.";
			newButton.GroupCaption = "Map Tool";
			newButton.LargeImage = Resources.line_chart_icon;
			newButton.ToggleGroupKey = "";
			App.HeaderControl.Add(newButton);

			base.Activate();
		}

		// Deactivates the plugin
		public override void Deactivate() 
		{
            lineDrawing.DeactivateLine();
			active = false;

			// Remove ribbon tab
			App.HeaderControl.RemoveAll();
			base.Deactivate();
		}

		#endregion Plugin operations

		#region Event Handlers

        //Event handler that responds to newButton being clicked
		void buttonClick(object sender, EventArgs e) 
		{
            //Checks to see if a raster layer is loaded
			if (App.Map.GetRasterLayers().Count() == 0) {
				newButton.Toggle();
				MessageBox.Show("Please add a Digital Elevation Model raster layer to the map");
				return;
			}
            
			if(!active)
			{
				lineDrawing.ActivateLine();
				active = true;
			}
			else
			{
				lineDrawing.DeactivateLine();
				active = false;
			}
		}

		#endregion Event Handlers
	}
}
