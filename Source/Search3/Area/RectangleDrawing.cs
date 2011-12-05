using System;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using System.Data;
using DotSpatial.Symbology;
using System.Drawing;
using System.Windows.Forms;

namespace Search3.Area
{
    /// <summary>
    /// This class is responsible for drawing the area rectangle
    /// </summary>
    public class RectangleDrawing
    {
        //todo: Copied from Search2. Need to be refactored.

        #region Fields

        private readonly Map _mainMap;
        private MapPolygonLayer _rectangleLayer;
        private bool _isActive;
        private int _numClicks;
        private Coordinate _startPoint;

        #endregion

        public RectangleDrawing(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            _mainMap = map;
        }

        public event EventHandler RectangleCreated;

        /// <summary>
        /// The extent of the area rectangle
        /// </summary>
        public Extent RectangleExtent
        {
            get
            {
                if (_rectangleLayer.DataSet.Features.Count == 1)
                {
                    IFeature polyF = _rectangleLayer.DataSet.Features[0];
                    return polyF.Envelope.ToExtent();
                }
                else
                {
                    return new Extent(_mainMap.ViewExtents.ToEnvelope());
                }
            }
        }

        public bool IsActivated
        {
            get { return _isActive; }
        }

        /// <summary>
        /// Activates the rectangle drawing function
        /// </summary>
        public void Activate()
        {
            if (!_isActive)
            {
                _mainMap.MouseDown += mainMap_MouseDown;
                _mainMap.MouseUp += mainMap_MouseUp;
            }
            _numClicks = 0;
            _isActive = true;
            _mainMap.FunctionMode = FunctionMode.Select;
            _mainMap.Cursor = Cursors.Cross;
            AddRectangleLayer();
            DisableLayerSelection();
        }

        /// <summary>
        /// Deactivates the rectangle drawing function
        /// </summary>
        public void Deactivate()
        {
            _mainMap.MouseDown -= mainMap_MouseDown;
            _mainMap.MouseUp -= mainMap_MouseUp;
            _numClicks = 0;
            _isActive = false;

            if (_rectangleLayer != null)
            {
                if (_rectangleLayer.DataSet.Features != null)
                {
                    _rectangleLayer.DataSet.Features.Clear();
                }
                RemoveRectangleLayer();
            }
            _mainMap.ResetBuffer();
            _mainMap.FunctionMode = FunctionMode.Select;
            EnableLayerSelection();
        }

        private void DisableLayerSelection()
        {
            foreach (IMapLayer lay in _mainMap.GetAllLayers())
            {
                if (lay.LegendText != Properties.Resources.RectangleLayerName)
                {
                    lay.IsSelected = false;
                    lay.SelectionEnabled = false;
                }
                else
                {
                    lay.IsSelected = true;
                }
            }
        }

        private void EnableLayerSelection()
        {
            foreach (IMapLayer lay in _mainMap.GetAllLayers())
            {
                lay.SelectionEnabled = true;
            }
        }

        void mainMap_MouseUp(object sender, MouseEventArgs e)
        {
            //only modify rectangle drawing if function mode is Select
            if (_mainMap.FunctionMode != FunctionMode.Select) return;
            
            if (_numClicks == 1)
            {
                Coordinate endPoint = new Coordinate(_mainMap.PixelToProj(e.Location));

                _rectangleLayer.DataSet.Features.Clear();

                Coordinate[] array = new Coordinate[5];
                array[0] = _startPoint;
                array[1] = new Coordinate(_startPoint.X, endPoint.Y);
                array[2] = endPoint;
                array[3] = new Coordinate(endPoint.X, _startPoint.Y);
                array[4] = _startPoint;
                LinearRing shell = new LinearRing(array);
                Polygon poly = new Polygon(shell);
                IFeature newF = _rectangleLayer.DataSet.AddFeature(poly);
                newF.DataRow["ID"] = 1;
                _numClicks = 0;
                
                _mainMap.ResetBuffer();
                //Deactivate();
                OnRectangleCreated();
            }
        }

        void mainMap_MouseDown(object sender, MouseEventArgs e)
        {
            //only modify rectangle drawing if function mode is Select
            if (_mainMap.FunctionMode != FunctionMode.Select) return;
            
            if (_numClicks == 0)
            {
                //todo: draw point...
                _startPoint = new Coordinate(_mainMap.PixelToProj(e.Location));
                _numClicks = 1;
                
            }
            else if (_numClicks == 1)
            {
                _numClicks = 2;
            }

        }

        /// <summary>
        /// Restores the 'search area rectangle' in the map
        /// </summary>
        public void RestoreSearchRectangle(double minLon, double minLat, double maxLon, double maxLat)
        {
            AddRectangleLayer();
            if (_rectangleLayer != null)
            {
                _rectangleLayer.DataSet.Features.Clear();

                double maxLat1 = Math.Max(minLat, maxLat);
                double minLat1 = Math.Min(minLat, maxLat);
                double maxLon1 = Math.Max(minLon, maxLon);
                double minLon1 = Math.Min(minLon, maxLon);

                //reproject the points
                int numPoints = 4;
                double[] array = new double[8];
                array[0] = minLon1;
                array[1] = minLat1;
                array[2] = minLon1;
                array[3] = maxLat1;
                array[4] = maxLon1;
                array[5] = maxLat1;
                array[6] = maxLon1;
                array[7] = minLat1;

                ProjectionInfo wgs84 = ProjectionInfo.FromEsriString(Properties.Resources.wgs_84_esri_string);
                Reproject.ReprojectPoints(array, new double[] { 0, 0, 0, 0 }, wgs84, _mainMap.Projection, 0, numPoints);

                //form the coordinate array and add rectangle feature
                Coordinate[] coords = new Coordinate[5];
                coords[0] = new Coordinate(array[0], array[1]);
                coords[1] = new Coordinate(array[2], array[3]);
                coords[2] = new Coordinate(array[4], array[5]);
                coords[3] = new Coordinate(array[6], array[7]);
                //add the closing point of the rectangle - same as first point
                coords[4] = new Coordinate(array[0], array[1]);

                //create a polygon feature from the coordinate array
                LinearRing shell = new LinearRing(coords);
                Polygon poly = new Polygon(shell);
                IFeature newF = _rectangleLayer.DataSet.AddFeature(poly);
                newF.DataRow["ID"] = 1;
                _numClicks = 0;

                _mainMap.ResetBuffer();
                //Deactivate();
                OnRectangleCreated();
            }
        }

        /// <summary>
        /// Adds the hidden "Search Rectangle" layer to the map
        /// </summary>
        private void AddRectangleLayer()
        {
            //check for the rectangle layer
            if (_rectangleLayer == null)
            {
                foreach (IMapLayer lay in _mainMap.GetAllLayers())
                {
                    if (lay.LegendText == Properties.Resources.RectangleLayerName)
                    {
                        _rectangleLayer = lay as MapPolygonLayer;
                        break;
                    }
                }
            }

            if (_rectangleLayer == null)
            {
                var rectangleFs = new FeatureSet(FeatureType.Polygon);
                rectangleFs.DataTable.Columns.Add(new DataColumn("ID"));
                rectangleFs.Projection = _mainMap.Projection;

                _rectangleLayer = new MapPolygonLayer(rectangleFs){LegendText = Properties.Resources.RectangleLayerName};
                var redColor = Color.Red.ToTransparent(0.5f);
                _rectangleLayer.Symbolizer = new PolygonSymbolizer(redColor, Color.Red);
                _rectangleLayer.SelectionSymbolizer = _rectangleLayer.Symbolizer;
                _mainMap.Layers.Add(_rectangleLayer);
            }
        }

        /// <summary>
        /// Returns true if the map already contains the rectangle layer..
        /// </summary>
        /// <returns></returns>
        public bool RectangleLayerIsInMap()
        {
            foreach (IMapLayer lay in _mainMap.GetAllLayers())
            {
                if (lay.LegendText == Properties.Resources.RectangleLayerName)
                {
                    return true;
                }
            }
            return false;
        }

        private void RemoveRectangleLayer()
        {
            _mainMap.Layers.Remove(_rectangleLayer);
            _rectangleLayer = null;
        }

        protected void OnRectangleCreated()
        {
            if (RectangleCreated != null) RectangleCreated(this, EventArgs.Empty);
        }
    }
}
