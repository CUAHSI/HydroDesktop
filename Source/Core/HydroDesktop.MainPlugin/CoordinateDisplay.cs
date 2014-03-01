using System;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Controls.Header;
using DotSpatial.Projections;
using DotSpatial.Topology;

namespace HydroDesktop.Main
{
    /// <summary>
    /// This class is responsible for
    /// displaying the Lat, Lon coordinates
    /// in the status bar
    /// </summary>
    public class CoordinateDisplay
    {
        private Map mainMap = null;
        private AppManager mainApp = null;

        ProjectionInfo wgs84Projection = ProjectionInfo.FromEsriString(Properties.Resources.wgs_84_esri_string);
        ProjectionInfo currentMapProjection = null;
        StatusPanel latLonStatusPanel = null;
        bool isWgs84 = true;
        bool _showCoordinates = false;

        public CoordinateDisplay(AppManager app)
        {
            latLonStatusPanel = new StatusPanel();
            latLonStatusPanel.Width = 400;
            app.ProgressHandler.Add(latLonStatusPanel);

            mainApp = app;
            mainMap = app.Map as Map;
            if (mainMap == null) return;

            string mapProjEsriString = mainMap.Projection.ToEsriString();
            isWgs84 = (mapProjEsriString == Properties.Resources.wgs_84_esri_string);
            currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);

            mainMap.MouseMove +=mainMap_MouseMove;
            mainMap.ProjectionChanged += mainMap_ProjectionChanged;
        }

        void mainMap_ProjectionChanged(object sender, EventArgs e)
        {
            string mapProjEsriString = mainMap.Projection.ToEsriString();
            isWgs84 = (mapProjEsriString == Properties.Resources.wgs_84_esri_string);
            currentMapProjection = ProjectionInfo.FromEsriString(mapProjEsriString);
        }

        public bool ShowCoordinates
        {
            get 
            { 
                return _showCoordinates;
            }
            set
            {
                _showCoordinates = value;

                if (_showCoordinates == false)
                {
                    //mainApp.ProgressHandler.Remove(
                    latLonStatusPanel.Caption = String.Empty;
                }
                else
                {
                    //mainApp.ProgressHandler.Add(latLonStatusPanel);
                }
                //latLonStatusPanel.Caption = String.Empty;
            }
        }

        public string MapProjectionString
        {
            get { return currentMapProjection.ToEsriString(); }
            set 
            { 
                currentMapProjection = ProjectionInfo.FromEsriString(value);
                isWgs84 = (currentMapProjection.ToEsriString() == Properties.Resources.wgs_84_esri_string);
            }
        }

        #region Coordinate Display

        private void mainMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ShowCoordinates)
            {
                return;
            }
            
            var projCor = new Coordinate();
            var _mouseLocation = new System.Drawing.Point();
            _mouseLocation.X = e.X;
            _mouseLocation.Y = e.Y;
            projCor = mainMap.PixelToProj(_mouseLocation);

            var xy = new double[2];
            xy[0] = projCor.X;
            xy[1] = projCor.Y;

            var z = new double[1];
            if (!isWgs84)
            {
                Reproject.ReprojectPoints(xy, z, currentMapProjection, wgs84Projection, 0, 1);
            }

            //Convert to Degrees Minutes Seconds
            double[] coord = new double[2];
            coord[0] = Math.Abs(xy[0]);
            coord[1] = Math.Abs(xy[1]);

            double[] d = new double[2];
            double[] m = new double[2];
            double[] s = new double[2];

            d[0] = Math.Floor(coord[0]);
            coord[0] -= d[0];
            coord[0] *= 60;

            m[0] = Math.Floor(coord[0]);
            coord[0] -= m[0];
            coord[0] *= 60;

            s[0] = Math.Floor(coord[0]);

            d[1] = Math.Floor(coord[1]);
            coord[1] -= d[1];
            coord[1] *= 60;

            m[1] = Math.Floor(coord[1]);
            coord[1] -= m[1];
            coord[1] *= 60;

            s[1] = Math.Floor(coord[1]);

            string Long;
            string Lat;

            if (projCor.X > 0) Long = "E";
            else if (projCor.X < 0) Long = "W";
            else Long = " ";

            if (projCor.Y > 0) Lat = "N";
            else if (projCor.Y < 0) Lat = "S";
            else Lat = " ";

            latLonStatusPanel.Caption = "Longitude: " + d[0].ToString() + "°" + m[0].ToString("00") + "'" + s[0].ToString("00") + "\"" + Long + ", Latitude: " + d[1].ToString() + "°" + m[1].ToString("00") + "'" + s[1].ToString("00") + "\"" + Lat;
        }

        #endregion Coordinate Display
    }
}
