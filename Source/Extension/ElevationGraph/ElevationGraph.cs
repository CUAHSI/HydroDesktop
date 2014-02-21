using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace ElevationCrossSection 
{
    /// <summary>
    /// Form that displays an elevation graph
    /// </summary>
	public class ElevationGraph : Form
	{
		IMapRasterLayer[] rasterLayers;
        Chart chart;
        List<Double> elevationList;
        private Button save = new Button();
        private Button close = new Button();
        int numberofpoints = 400;
		Map map;

        /// <summary>
        /// Creates an ElevationGraph
        /// </summary>
        /// <param name="rLayer">The digital elevation model raster layer</param>
        /// <param name="a"> The first point on the line</param>
        /// <param name="b"> The last point on the line</param>
		public ElevationGraph(Map m) 
		{
			map = m;
            rasterLayers = map.GetRasterLayers();
		}

		public void drawGraph(Coordinate a, Coordinate b)
		{
			chart = new Chart();
			elevationList = new List<Double>();
			double distance = Math.Round(a.Distance(b), 0);
            bool success = getElevationData(a, b, distance);
            if (success)
            {
                InitializeComponent(distance);
                this.Show();
            }
		}

        //Extracts the elevation data that corresponds to the line from a to b
        // Returns whether it was successful
		private bool getElevationData(Coordinate a, Coordinate b, double distance)
		{
			double curX;
			double curY;
			double curElevation = 0;
			double constXdif = ((b.X - a.X) / numberofpoints);
			double constYdif = ((b.Y - a.Y) / numberofpoints);
            Coordinate coordinate;
            bool success = true;
			int numberOfRasterLayers = rasterLayers.Length;
			int currentRasterLayer = 0;

            
            for (int i = 0; i <= numberofpoints; i++)
            {
                //Get map coordinates
                curX = a.X + i * constXdif;
                curY = a.Y + i * constYdif;
                coordinate = new Coordinate(curX, curY);
				RcIndex rowColumn = new RcIndex();
                bool found = false;

				for (int j = 0; j < numberOfRasterLayers; j++) {
					try {
						//Calculate raster cell coordinates
						rowColumn = rasterLayers[currentRasterLayer].DataSet.Bounds.ProjToCell(coordinate);

						//Extract elevation from raster cell
						curElevation = rasterLayers[currentRasterLayer].DataSet.Value[rowColumn.Row, rowColumn.Column];
						elevationList.Add(curElevation);

						//if elevation was extracted from this coordinate
                        found = true;
						break;
					}
					catch (Exception e) //if wrong raster layer is read, try next one
					{
						currentRasterLayer++;
						if (currentRasterLayer == numberOfRasterLayers)
							currentRasterLayer = 0;
					}
				}

                if (!found)
                {
                    MessageBox.Show("The whole line needs to be inside a DEM");
                    success = false;
                    return success;
                }
            }
			
            return success;
		}

        //Sets up the chart, series, buttons, and form
        private void InitializeComponent(double distance)
        {
            ChartArea chartArea = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            ((System.ComponentModel.ISupportInitialize)(chart)).BeginInit();
            this.SuspendLayout();
            
            // chart
            chartArea.AxisX.Title = "Euclidean Distance = " + distance.ToString("N0") + " meters";
            chartArea.AxisY.Title = "Elevation [meters]";
            chart.ChartAreas.Add(chartArea);
            chart.Location = new System.Drawing.Point(0, 50);
            
            // series
            var series = new Series
                {
                    Color = System.Drawing.Color.Green,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Line
                };
            chart.Series.Add(series);
            double distanceBetweenPoints = distance / numberofpoints;
            for(int i = 0; i < numberofpoints; i++)
            {
                series.Points.AddXY(i * distanceBetweenPoints, elevationList.ElementAt(i));
            }
            
            //buttons
            save.Text = "Save";
            save.AutoSize = true;
            save.Click += new EventHandler(saveClick);
            save.Dock = DockStyle.Right;
            close.Text = "Close";
            close.AutoSize = true;
            close.Click += new EventHandler(closeClick);
            close.Dock = DockStyle.Right;
            GroupBox gbox = new GroupBox();
            gbox.Size = new System.Drawing.Size(100, 50);
            gbox.Controls.Add(save);
            gbox.Controls.Add(close);

            // Form
            this.ClientSize = new System.Drawing.Size(600, 350);
            gbox.Dock = DockStyle.Bottom;
            this.Controls.Add(gbox);
            chart.Dock = DockStyle.Top;
            this.Controls.Add(chart);
            this.Text = "Elevation Chart";
            ((System.ComponentModel.ISupportInitialize)(chart)).EndInit();
            this.ResumeLayout(false);
        }

        //Event handler called when the save button is clicked
        private void saveClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Bitmap File(*.bmp)|*.bmp|JPG File(*.jpg)|*.jpg|GIF File (*.gif)|*.gif|PNG File (*.png)|*.png";
            saveFileDialog1.FilterIndex = 4;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var fileName = saveFileDialog1.FileName;
                var extension = Path.GetExtension(fileName);
                
                switch(extension.ToLower())
                {
                    case ".bmp":
                        chart.SaveImage(fileName, ChartImageFormat.Bmp);
                        break;
                    case ".jpg":
                        chart.SaveImage(fileName, ChartImageFormat.Jpeg);
                        break;
                    case ".gif":
                        chart.SaveImage(fileName, ChartImageFormat.Gif);
                        break;
                    case ".png":
                        chart.SaveImage(fileName, ChartImageFormat.Png);
                        break;
                    default :
                        chart.SaveImage(fileName, ChartImageFormat.Png);
                        break;
                }
            }
        }

        //Event handler called when the close button is clicked
        private void closeClick(object sender, EventArgs e)
        {
            this.Close();
        }
		/*
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
		 * */
	}
}
