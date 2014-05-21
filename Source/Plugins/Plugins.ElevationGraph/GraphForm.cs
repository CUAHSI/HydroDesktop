using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Topology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace ElevationGraph {
	public partial class GraphForm : Form {
		int numberofpoints = 1000;
		Map map;
		IMapRasterLayer[] rasterLayers;
		double totalDist;
		List<PointPairList> elevationList;
		PointPair pointPair;
		List<Color> colors;
		enum Units { Meters, Kilometers, Feet, Miles };
        private double[] distanceUnitFactors = new[] { .001, 0.000621371192, 3.2808399 };//km, miles, feet
		Units xUnits;
		Units yUnits;
		List<Coordinate> coordinates;

		/// <summary>
		/// Creates a Zed Graph Form, but doesn't display it.
		/// </summary>
		/// <param name="m">The map that contains the raster layers</param>
		public GraphForm(Map m) {
			InitializeComponent();
			map = m;
			map.MapFrame.VisibleChanged += updateGraph;
			xUnits = Units.Meters;
			yUnits = Units.Meters;
        }

		/// <summary>
		/// Draw an elevation graph using a list of coordinates
		/// </summary>
		/// <param name="coordinates">the coordinates to use in creating the graph</param>
		public void drawGraph(List<Coordinate> coords){
			coordinates = coords;
			rasterLayers = map.GetRasterLayers();
			elevationList = new List<PointPairList>();

			//initialize elevationList entries
			for (int i = 0; i < rasterLayers.Length; i++)
				elevationList.Add(new PointPairList());

			getDistance(coordinates);
			getElevationData(coordinates);
			pickColors(rasterLayers.Length);

			updateGraph(null, null);
		}

		//creates the graph using the elevationList and colors list, called by drawGraph or by map.MapFrame.VisibleChanged.
		private void updateGraph(object sender, EventArgs e) {
			if (zedGraphControl1.IsDisposed)
				return;

			RectangleF rect = new RectangleF(0, 0, zedGraphControl1.Size.Width, zedGraphControl1.Size.Height);
			String xAxisLabel = "Distance (" + xUnits + ")";
			String yAxisLabel = "Elevation (" + yUnits + ")";
			zedGraphControl1.GraphPane = new GraphPane(rect, "Elevation Graph", xAxisLabel , yAxisLabel);
            if (totalDist != 0){
                switch (xUnits)
                {
                    case Units.Meters:
                        zedGraphControl1.GraphPane.XAxis.Scale.Max = totalDist;
                        break;
                    case Units.Kilometers:
                        zedGraphControl1.GraphPane.XAxis.Scale.Max = totalDist * distanceUnitFactors[0];
                        break;
                    case Units.Miles:
                        zedGraphControl1.GraphPane.XAxis.Scale.Max = totalDist * distanceUnitFactors[1];
                        break;
                    case Units.Feet:
                        zedGraphControl1.GraphPane.XAxis.Scale.Max = totalDist * distanceUnitFactors[2];
                        break;
                }
            }

			//draw curves
			for (int i = 0; i < rasterLayers.Length; i++) {
				if (rasterLayers[i].IsVisible) {
					ZedGraph.LineItem myCurve = zedGraphControl1.GraphPane.AddCurve(rasterLayers[i].LegendText, elevationList[i], colors[i], SymbolType.None);
					myCurve.Line.IsAntiAlias = true;
					myCurve.Line.Width = 2f;
					myCurve.Line.Fill.IsVisible = false;
				}
			}

			//refresh the graph
			zedGraphControl1.AxisChange();
			zedGraphControl1.Refresh();
		}

		//fills the colors list with the colors to use for the lines in the graph
		private void pickColors(int numberOfColors) {
			double golden_ratio_conjugate = 0.618033988749895 * 360;
			Random rand = new Random();
			double hue = (double) rand.Next(360);
			colors = new List<Color>();

			for (int i = 0; i < numberOfColors; i++) {
				hue += golden_ratio_conjugate % 360;
				colors.Add(ColorFromHSV(hue, .99, .99));
			}
		}

		//converts HSV color to a System.Drawing.Color, used by pickColors
		private Color ColorFromHSV(double hue, double saturation, double value) {
			int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
			double f = hue / 60 - Math.Floor(hue / 60);

			value = value * 255;
			int v = Convert.ToInt32(value);
			int p = Convert.ToInt32(value * (1 - saturation));
			int q = Convert.ToInt32(value * (1 - f * saturation));
			int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

			if (hi == 0)
				return Color.FromArgb(255, v, t, p);
			else if (hi == 1)
				return Color.FromArgb(255, q, v, p);
			else if (hi == 2)
				return Color.FromArgb(255, p, v, t);
			else if (hi == 3)
				return Color.FromArgb(255, p, q, v);
			else if (hi == 4)
				return Color.FromArgb(255, t, p, v);
			else
				return Color.FromArgb(255, v, p, q);
		}

		//gets the total distance of the path from the first to the last coordinate
		private void getDistance(List<Coordinate> coordinates) {
			Coordinate a, b;
			totalDist = 0;

			for(int i = 0; i < coordinates.Count - 1; i++){
				a = coordinates[i];
				b = coordinates[i + 1];
                totalDist += distanceTo(a, b);
			}
		}

		//finds the distance between two projection coordinates
        private double distanceTo(Coordinate c1, Coordinate c2)
        {
            double dx = Math.Abs(c2.X - c1.X);
            double dy = Math.Abs(c2.Y - c1.Y);
            double dist;
            if (map.Projection != null)
            {
                if (map.Projection.IsLatLon)
                {
                    double y = (c2.Y + c1.Y) / 2;
                    double factor = Math.Cos(y * Math.PI / 180);
                    dx *= factor;
                    dist = Math.Sqrt(dx * dx + dy * dy);
                    dist = dist * 111319.5;
                }
                else
                {
                    dist = Math.Sqrt(dx * dx + dy * dy);
                    dist *= map.Projection.Unit.Meters;
                }
            }
            else
            {
                dist = Math.Sqrt(dx * dx + dy * dy);
            }
            return dist;
        }

		//Extracts the elevation data from the raster layers that lie on the path from the first to the last coordinate
		private void getElevationData(List<Coordinate> coordinates) {
			double curX;
			double curY;
			double curElevation = 0;
			double constXdif, constYdif;
			double xAxisValue = 0;
			Coordinate a, b, temp;
			int numberOfRasterLayers = rasterLayers.Length;
			int pointsOnThisLine;

			//iterate through all line segments
			for (int i = 0; i < coordinates.Count - 1; i++) {
				a = coordinates[i];
				b = coordinates[i + 1];
				pointsOnThisLine = (int) (distanceTo(a, b) / totalDist * numberofpoints);
				constXdif = (Math.Abs(a.X - b.X) / pointsOnThisLine);
				constYdif = (Math.Abs(a.Y - b.Y) / pointsOnThisLine);

				//extract the appropriate number of points for this line
				for (int j = 0; j < pointsOnThisLine; j++) {
					//x coordinate on graph for this location
                    switch (xUnits)
                    {
                        case Units.Meters:
                            xAxisValue += totalDist / numberofpoints;
                            break;
                        case Units.Kilometers:
                            xAxisValue += totalDist / numberofpoints * distanceUnitFactors[0];
                            break;
                        case Units.Miles:
                            xAxisValue += totalDist / numberofpoints * distanceUnitFactors[1];
                            break;
                        case Units.Feet:
                            xAxisValue += totalDist / numberofpoints * distanceUnitFactors[2];
                            break;
                    }
					

					//Get map coordinates
					if(a.X < b.X)
						curX = a.X + j * constXdif;
					else
						curX = a.X - j * constXdif;
					
					if(a.Y < b.Y)
						curY = a.Y + j * constYdif;
					else
						curY = a.Y - j * constYdif;

					temp = new Coordinate(curX, curY);
					RcIndex rowColumn = new RcIndex();
                    
					//get the elevation data for each raster layer at point temp
					for (int k = 0; k < numberOfRasterLayers; k++) {
						//Calculate raster cell coordinates
						rowColumn = rasterLayers[k].DataSet.Bounds.ProjToCell(temp);

						//rasterLayers[k].DataSet.Projection.

						if (rowColumn != RcIndex.Empty && 
								rasterLayers[k].DataSet.Value[rowColumn.Row, rowColumn.Column] != rasterLayers[k].NoDataValue) {//Extract elevation from raster cell
							curElevation = rasterLayers[k].DataSet.Value[rowColumn.Row, rowColumn.Column];
                            switch (yUnits){
                                case Units.Kilometers:
                                    curElevation *= distanceUnitFactors[0];
                                    break;
                                case Units.Miles:
                                    curElevation *= distanceUnitFactors[1];
                                    break;
                                case Units.Feet:
                                    curElevation *= distanceUnitFactors[2];
                                    break;
                            }
                            pointPair = new PointPair(xAxisValue, curElevation);
							elevationList[k].Add(pointPair);
						}
						else {//if the raster layer doesn't have data for this coordinate, set y value to NaN
							pointPair = new PointPair(xAxisValue, double.NaN);
							elevationList[k].Add(pointPair);
						}
					}
				}
			}
		}

		//save button click
		private void button2_Click(object sender, EventArgs e) {
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Bitmap File(*.bmp)|*.bmp|JPG File(*.jpg)|*.jpg|GIF File (*.gif)|*.gif|PNG File (*.png)|*.png";
            saveFileDialog1.FilterIndex = 4;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = saveFileDialog1.FileName;
				zedGraphControl1.GraphPane.GetImage().Save(fileName);
			}
		}

		//close button click
		private void button1_Click(object sender, EventArgs e) {
			this.Hide();
			map.VisibleChanged -= updateGraph;
		}

		//x axis units changed
		private void XcomboBox_SelectedIndexChanged(object sender, EventArgs e) {
			switch ((sender as ComboBox).SelectedIndex) {
				case 0:
					xUnits = Units.Meters;
					break;
				case 1:
					xUnits = Units.Kilometers;
					break;
				case 2:
					xUnits = Units.Feet;
					break;
				case 3:
					xUnits = Units.Miles;
					break;
			}
			drawGraph(coordinates);
		}

		//y axis units changed
		private void YcomboBox_SelectedIndexChanged(object sender, EventArgs e) {
			switch ((sender as ComboBox).SelectedIndex) {
				case 0:
					yUnits = Units.Meters;
					break;
				case 1:
					yUnits = Units.Kilometers;
					break;
				case 2:
					yUnits = Units.Feet;
					break;
				case 3:
					yUnits = Units.Miles;
					break;
			}
			drawGraph(coordinates);
		}

	}
}

