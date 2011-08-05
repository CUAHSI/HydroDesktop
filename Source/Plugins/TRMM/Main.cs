using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Web.Services;

using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using DotSpatial.Controls;
using DotSpatial.Controls.RibbonControls;
using DotSpatial.Projections;

using HydroDesktop.Database;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;


namespace trmm
{
    public class Main : Extension
    {
        #region Variables

        private RibbonTab _rtSatellite;

        private RibbonPanel _rPanelSatellite;

        private RibbonButton _btSeries;

        private RibbonButton _btMap;

        private DateTimePicker _dpStart;
        private DateTimePicker _dpEnd;

        private RibbonHost _rhStartDate;
        private RibbonHost _rhEndDate;

        private BackgroundWorker _bgw;

        private ProjectionInfo _defaultProjection;

        private ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;


        #endregion

        #region IExtension Members

        /// <summary>
        /// Fires when the plugin should become inactive
        /// </summary>
        public override void Deactivate()
        {
            _rtSatellite.Panels[0].Items.Remove(_btMap);
            _rtSatellite.Panels[0].Items.Remove(_btSeries);
            _rtSatellite.Panels.Remove(_rPanelSatellite);

            base.Deactivate();
        }

        public override void Activate()
        {
            // Handle code for switching the page content
            //Setup background worker
            _bgw = new BackgroundWorker();
            _bgw.WorkerSupportsCancellation = false;
            _bgw.WorkerReportsProgress = false;
            _bgw.DoWork += new DoWorkEventHandler(_bgw_DoWork);
            _bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_bgw_RunWorkerCompleted);

            //Setup the Ribbon TAB
            _rtSatellite = App.Ribbon.Tabs[0];

            _defaultProjection = KnownCoordinateSystems.Projected.World.WebMercator;

            //Setup the Panel and Add it to the MapView tab
            _rPanelSatellite = new RibbonPanel("TRMM", RibbonPanelFlowDirection.Bottom);
            _rPanelSatellite.ButtonMoreEnabled = false;
            _rPanelSatellite.ButtonMoreVisible = false;
            _rtSatellite.Panels.Add(_rPanelSatellite);

            //Setup Map Button
            _btMap = new RibbonButton();
            _btMap.Text = "Map";
            _btMap.ToolTip = "TRMM Satellite Precipitation Map";
            _btMap.Image = Properties.Resources.Satellite_1;
            _btMap.CheckOnClick = true;
            _btMap.Click += new EventHandler(map_Click);

            //Add it into the panel
            _rPanelSatellite.Items.Add(_btMap);

            //Setup Series Button
            _btSeries = new RibbonButton();
            _btSeries.Text = "Time Series";
            _btSeries.ToolTip = "Daily Satellite Precipitation Time Series";
            _btSeries.Image = Properties.Resources.Satellite_2;
            _btSeries.CheckOnClick = true;
            _btSeries.Click += new EventHandler(series_Click);

            //Add it into the panel
            _rPanelSatellite.Items.Add(_btSeries);

            //Setup the start date dateTimePicker
            _rhStartDate = new RibbonHost();
            _rhStartDate.Text = "Start Date:";
            _dpStart = new DateTimePicker();
            _dpStart.Format = DateTimePickerFormat.Short;
            _dpStart.Value = new DateTime(2010, 8, 1);
            _dpStart.Width = 100;
            _rhStartDate.HostedControl = _dpStart;
            _rPanelSatellite.Items.Add(_rhStartDate);

            //Setup the start date dateTimePicker
            _rhEndDate = new RibbonHost();
            _rhEndDate.Text = "End Date:";

            _dpEnd = new DateTimePicker();
            _dpEnd.Format = DateTimePickerFormat.Short;
            _dpEnd.Value = new DateTime(2010, 9, 1);
            _dpEnd.Width = 100;
            _rhEndDate.ToolTip = "End Date";
            _rhEndDate.HostedControl = _dpEnd;
            _rPanelSatellite.Items.Add(_rhEndDate);


            // This line ensures that "Enabled" is set to true.
            base.Activate();
        }

        #endregion

        #region IPlugin Members

        # endregion

        #region BackgroundWorker Methods

        void _bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
            }

            else if (e.Result == null)
            {
                MessageBox.Show("no result");
            }

            else
            {
                frmResult2 resultForm = new frmResult2();
                object[] objRes = e.Result as object[];

                resultForm.Table = objRes[0] as DataTable;

                Coordinate coor = objRes[1] as Coordinate;
                resultForm.ClickedLat = Convert.ToDouble(coor.Y);
                resultForm.ClickedLon = Convert.ToDouble(coor.X);
                resultForm.App = App;

                resultForm.ShowDialog();
                
                //IList<IFeatureSet> result = e.Result as IList<IFeatureSet>;

                //_defaultProjection = _mapArgs.Map.Projection;

                //DotSpatial.Projections.GeographicCategories.World world = new DotSpatial.Projections.GeographicCategories.World();
                //DotSpatial.Projections.ProjectedCategories.World projWorld = new DotSpatial.Projections.ProjectedCategories.World();

                ////This reprojection procedure is critical and important to finally get the correct projection.
                //foreach (IFeatureSet fs in result)
                //{
                //    fs.Projection = world.WGS1984;
                //    fs.Reproject(projWorld.WebMercator);
                //}

                //AddEPAShapes(result);
                App.Map.Cursor = Cursors.Default;
                //_btstartDelineate.Checked = false;
            }
        }

        void _bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] param = e.Argument as object[];

            BackgroundWorker worker = sender as BackgroundWorker;

            GetSeries(param, _bgw, e);
        }

        # endregion

        #region Click Events

        void series_Click(object sender, EventArgs e)
        {
            Map map = App.Map as Map;

            if (_btSeries.Checked)
            {
                //Check if any other Map Tools are checked
                for (int i = 0; i < App.Ribbon.Tabs[0].Panels[1].Items.Count; i++)
                {
                    if (App.Ribbon.Tabs[0].Panels[1].Items[i].Checked == true)
                    {
                        App.Ribbon.Tabs[0].Panels[1].Items[i].Checked = false;
                        App.Map.FunctionMode = FunctionMode.None;
                    }
                }
            }
            
            try
            {
                map.Cursor = Cursors.Cross;
                map.MouseClick += new MouseEventHandler(Mouse_Click);
                _btSeries.Checked = true;
            }
            catch (Exception ex)
            {
                if (ex == null)
                    App.Map.Cursor = Cursors.Default;
            }
            
            
            //set map cursor to cross
            //map.Cursor = Cursors.Cross;
            //map.FunctionMode = FunctionMode.None;

            //map.MouseDown += new MouseEventHandler(map_MouseDown);
        }

        void map_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show(e.X + " " + e.Y);
        }


        /// <summary>
        /// Called when Mouse Click occurred on the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mouse_Click(object sender, MouseEventArgs e)
        {
            // Make sure we aren't still working on a previous task
            if (_bgw.IsBusy == true)
            {
                //MessageBox.Show("The background worker is busy now. Please try later.");
                return;
            }

            MouseButtons click = e.Button;
            Coordinate projCor = new Coordinate();

            Map _mainMap = App.Map as Map;

            if ((click == MouseButtons.Left) && (App.Map.Cursor == Cursors.Cross) && (_btSeries.Checked == true))
            {
                try
                {
                    App.Map.Cursor = Cursors.WaitCursor;

                    System.Drawing.Point _mouseLocation = new System.Drawing.Point();
                    _mouseLocation.X = e.X;
                    _mouseLocation.Y = e.Y;

                    projCor = App.Map.PixelToProj(_mouseLocation);

                    double[] xy = new double[2];
                    xy[0] = projCor.X;
                    xy[1] = projCor.Y;

                    double[] z = new double[1];
                    //Try to project here
                    Reproject.ReprojectPoints(xy, z, _defaultProjection, wgs84, 0, 1);

                    projCor.X = xy[0];
                    projCor.Y = xy[1];

                    object[] parameters = new object[2];

                    parameters[0] = (object)projCor;
                    parameters[1] = (object)_mainMap;

                    if (_bgw.IsBusy != true)
                    {
                        _bgw.RunWorkerAsync(parameters);
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        Coordinate Pixel2LonLat(System.Drawing.Point pixel)
        {
            Coordinate projC = App.Map.PixelToProj(pixel);

            double[] xy = new double[2];
            xy[0] = projC.X;
            xy[1] = projC.Y;

            double[] z = new double[1];
            //Try to project here
            Reproject.ReprojectPoints(xy, z, _defaultProjection, wgs84, 0, 1);

            projC.X = xy[0];
            projC.Y = xy[1];

            return projC;
        }


        void map_Click(object sender, EventArgs e)
        {

            if (_btMap.Checked == false)
            {
                App.Map.Cursor = Cursors.Default;
            }

            else
            {
                //Check if any other Map Tools are checked
                for (int i = 0; i < App.Ribbon.Tabs[0].Panels[1].Items.Count; i++)
                {
                    if (App.Ribbon.Tabs[0].Panels[1].Items[i].Checked == true)
                    {
                        App.Ribbon.Tabs[0].Panels[1].Items[i].Checked = false;
                        App.Map.FunctionMode = FunctionMode.None;
                    }
                }

                //get the map corner coordinates
                System.Drawing.Point corner1 = new System.Drawing.Point(App.Map.Left, App.Map.Top + App.Map.Height);
                System.Drawing.Point corner2 = new System.Drawing.Point(App.Map.Left + App.Map.Width, App.Map.Top);

                Coordinate c1 = Pixel2LonLat(corner1);
                Coordinate c2 = Pixel2LonLat(corner2);

                DataTable tab = TimeSeriesLoader.GetGrid(c1.X, c2.X, c1.Y, c2.Y, _dpStart.Value, _dpEnd.Value);

                frmResult2 res2 = new frmResult2();
                res2.Table = tab;
                res2.ShowDialog();

                //add the featureset
                FeatureSet fs = TimeSeriesLoader.MakeFeatureSet(tab);

                string hdProjectPath = Settings.Instance.CurrentProjectDirectory;
                string filename = Path.Combine(hdProjectPath, "precipitation.shp");
                fs.Filename = filename;
                fs.Projection = App.Map.Projection;
                fs.FillAttributes();
                //fs.Save();
                //fs = null;

                //IFeatureSet fs2 = FeatureSet.OpenFile(filename);
                //fs2.Projection = _mapArgs.Map.Projection;

                MapPointLayer lay = new MapPointLayer(fs);
                lay.LegendText = "precipitation";

                PointSymbolizer s1 = lay.Symbolizer as PointSymbolizer;
                s1.SetSize(new Size2D(10, 10));

                PointScheme sym = lay.Symbology as PointScheme;
                sym.EditorSettings.ClassificationType = ClassificationType.Quantities;
                
                App.Map.Layers.Add(lay);

                //MessageBox.Show("Satellite Map!");
            }
        }

        #endregion



        #region Methods

        /// <summary>
        /// Call CallWebService Method to get delineated watershed, and also return the start point.
        /// </summary>
        /// <param name="param">Arguments for backgroundworkers</param>
        /// <param name="bgw_worker">Declare a backgroundworker</param>
        /// <param name="e">Do work event</param>
        /// <returns>Return a list of featureset including both point and polygon</returns>
        public void GetSeries(object[] param, BackgroundWorker bgw_worker, DoWorkEventArgs e)
        {
            Coordinate projCor = (Coordinate)param[0];
            Map _mainMap = (Map)param[1];

            object[] res = new object[2];
            
            DataTable dt = TimeSeriesLoader.GetTimeSeries(projCor.Y, projCor.X, _dpStart.Value, _dpEnd.Value);
            res[0] = dt;
            res[1] = projCor;
            e.Result = res;
        }

        #endregion
    }
}
