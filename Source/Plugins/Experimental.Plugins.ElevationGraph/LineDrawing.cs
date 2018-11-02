using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using Point = System.Drawing.Point;
using System.Drawing.Drawing2D;

namespace ElevationGraph
	{
    /// <summary>
    /// This class draws the line for the ElevationCrossSection plugin
    /// </summary>
	class LineDrawing : MapFunction
		{
			#region Fields
			private readonly Map mainMap;
			private Coordinate latestCoord;
			private Point latestPoint, currentPoint;
			private List<Coordinate> coordinateList;
			private MapLineLayer lineLayer;
			private Color color = Color.Red;
			private Cursor cursor;
			private FunctionMode function;
			private bool currentlyDrawing = false;
			private bool enabled;
			private Rectangle updateRectangle;
			private GraphForm graph;
			private Pen redPen = new Pen(Color.Red, 2);
			#endregion Fields

			#region Constructors
			/// <summary>
			/// Creates a new LineDrawing object
			/// </summary>
			/// <param name="map">The map that the line will be drawn on</param>
			public LineDrawing(Map map) : base(map)
			{
				if (map == null)
					throw new ArgumentNullException("map");
				mainMap = map;
			}

			#endregion Constructors

			#region Public methods
            /// <summary>
            /// Turns on the line drawing mode
            /// </summary>
			public void ActivateLine()
			{
                //add mouse handlers to mainMap's mouse handlers
				mainMap.MouseDown += mouseDown;
				mainMap.MouseMove += mouseMove;

                //Save the current function and cursor to be restored when DeactivateLine is called
				function = mainMap.FunctionMode;
				cursor = mainMap.Cursor;
				mainMap.FunctionMode = FunctionMode.None;
				mainMap.ActivateMapFunction(this);
				this.YieldStyle = (YieldStyles) 0xf;
				mainMap.Cursor = Cursors.Cross;

				this.Enabled = true;
				enabled = true;
				graph = new GraphForm(mainMap);
				graph.Owner = mainMap.ParentForm;
				addLineLayer();
			}
            
            /// <summary>
            /// Turns off line drawing mode
            /// </summary>
			public void DeactivateLine()
			{
				removeLineLayer();
				if (!enabled)
					return;
				enabled = false;

                //remove mouse handlers from mainMap's mouse handlers
				mainMap.MouseDown -= mouseDown;
				mainMap.MouseMove -= mouseMove;

                //restore the function mode and cursor that were active when ActivateLine was called
				mainMap.FunctionMode = function;
				mainMap.Cursor = cursor;
				this.Enabled = false;
			}
		    
            //This is called when another tool is activated.  The line drawing mode is
            //deactivated, but the linelayer is not removed.
			protected override void OnDeactivate() {
				if (!enabled)
					return;
				enabled = false;
				mainMap.MouseDown -= mouseDown;
				mainMap.MouseMove -= mouseMove;
				this.Enabled = false;
			}
            
            //Draws the rubber band line while the left mouse button is still down
            protected override void OnDraw(MapDrawArgs e)
            {
                if (currentlyDrawing)
                {
                    e.Graphics.DrawLine(redPen, latestPoint, currentPoint);
                }
                base.OnDraw(e);
            }

			#endregion Public methods

			#region Private methods

            //draws a line when the left mouse button is pressed
			private void mouseDown(object sender, MouseEventArgs e)
			{
				Coordinate newCoord = new Coordinate(mainMap.PixelToProj(e.Location));;
				Point newPoint = e.Location;

				if(!currentlyDrawing){//start new line
					addLineLayer();
					latestPoint = newPoint;
					latestCoord = newCoord;
					currentlyDrawing = true;
					coordinateList = new List<Coordinate>();
					coordinateList.Add(newCoord);
				}
				else{//continue line
					
					//double click or right click to end line
					if (Math.Abs(latestPoint.X - newPoint.X) < 5 && Math.Abs(latestPoint.Y - newPoint.Y) < 5 || e.Button == MouseButtons.Right) {
						currentlyDrawing = false;
						mainMap.Invalidate();

						//show graph
						if (graph.IsDisposed) {
							graph = new GraphForm(mainMap);
							graph.Owner = mainMap.ParentForm;
						}
						graph.drawGraph(coordinateList);
						if (!graph.Visible)
							graph.Show();
						graph.WindowState = FormWindowState.Normal;
						graph.BringToFront();
						
					}
					else{//add new line segment to map
					List<Coordinate> currentLine = new List<Coordinate>();
					currentLine.Add(latestCoord);
					currentLine.Add(newCoord);
					var line = new LineString(currentLine);
					lineLayer.DataSet.AddFeature(line);
					mainMap.ResetBuffer();
					
					coordinateList.Add(newCoord);
					latestPoint = newPoint;
					latestCoord = newCoord;
					currentlyDrawing = true;
					}
				}
			}
            
            //When the left mouse button is pressed and the mouse moves, this method sets
            //current point and figures out the rectangle of the map that needs to be
            //updated.  Invalidate is then called, and the OnDraw method above uses
            //currentPoint to draw the rubber band line
			private void mouseMove(object sender, MouseEventArgs e)
			{
				if (currentlyDrawing)
				{
					int x = Math.Min(Math.Min(latestPoint.X, currentPoint.X), e.X) - 5;
					int y = Math.Min(Math.Min(latestPoint.Y, currentPoint.Y), e.Y) - 5;
					int mx = Math.Max(Math.Max(latestPoint.X, currentPoint.X), e.X) + 5;
					int my = Math.Max(Math.Max(latestPoint.Y, currentPoint.Y), e.Y) + 5;
					updateRectangle = new Rectangle(x, y, mx - x, my - y);
                    currentPoint = e.Location;
					mainMap.Invalidate(updateRectangle);
				}
				
			}

            //adds the line layer if it doesn't exist, clears it otherwise
			private void addLineLayer()
			{
				if (lineLayer == null) {
					var rectangleFs = new FeatureSet(FeatureType.Line);
					rectangleFs.DataTable.Columns.Add(new DataColumn("ID"));
					rectangleFs.Projection = mainMap.Projection;
					lineLayer = new MapLineLayer(rectangleFs) { LegendText = "Cross Section Line" };
					mainMap.Layers.Add(lineLayer);

					//The symbolizer that controls the look of the final line
					lineLayer.Symbolizer = new LineSymbolizer(Color.Red, 3);
					lineLayer.SelectionSymbolizer = lineLayer.Symbolizer;
				}
				else {
					lineLayer.DataSet.Features.Clear();

					//move linelayer to top if it isn't there
					if (mainMap.Layers.IndexOf(lineLayer) != mainMap.Layers.Count - 1) {
						lineLayer.LockDispose();
						mainMap.Layers.Remove(lineLayer);
						mainMap.Layers.Add(lineLayer);
						lineLayer.UnlockDispose();
					}
					
				}
			}
            
            //removes the line layer when DeactivateLine is called
			private void removeLineLayer()
			{
				if (lineLayer == null)
					return;
				mainMap.Layers.Remove(lineLayer);
				lineLayer = null;
			}

			#endregion Private methods
		}
	}
