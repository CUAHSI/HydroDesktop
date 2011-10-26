using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Topology;
using DotSpatial.Symbology;
using DotSpatial.Projections;

namespace HydroDesktop.Search
{
    /// <summary>
    /// A customized map function for drawing an extent rectangle on the map
    /// The rectangle is drawn by clicking and dragging the mouse
    /// </summary>
    public class RectangleFunction : MapFunction
    {
        #region Private Variables

        private bool _isDragging;
        
        //this gets set to true after a rectangle has been drawn
        private bool _isFinished = false;
        
        private System.Drawing.Point _startPoint;
        private Coordinate _geoStartPoint;
        private Coordinate _geoEndPoint;
        private System.Drawing.Point _currentPoint;

        private readonly Pen _selectionPen;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of RectangleFunction
        /// </summary>
        public RectangleFunction(IMap inMap)
            : base(inMap)
        {
            _selectionPen = new Pen(Color.Black);
            _selectionPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            //_selectionPen.Width = 2;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the selected extent rectangle in projected coordinates
        /// </summary>
        public Extent ExtentRectangle
        {
            get
            {
                //return new Envelope(_geoStartPoint, _geoEndPoint);
                if (_geoStartPoint != null && _geoEndPoint != null)
                {
                    //to reproject the points:
                    double[] minXY = new double[] { _geoStartPoint.X, _geoStartPoint.Y };
                    double[] maxXY = new double[] { _geoEndPoint.X, _geoEndPoint.Y };
                    double[] minZ = new double[] { 0.0 };
                    double[] maxZ = new double[] { 0.0 };

                    ProjectionInfo wgs84 = KnownCoordinateSystems.Geographic.World.WGS1984;
                    ProjectionInfo webMercator = Map.Projection;

                    Reproject.ReprojectPoints(minXY, minZ, webMercator, wgs84, 0, 1);
                    Reproject.ReprojectPoints(maxXY, maxZ, webMercator, wgs84, 0, 1);
                    
                    return new Extent(minXY[0], minXY[1], maxXY[0], maxXY[1]);
                }
                else
                {
                    return Map.ViewExtents;
                }
            }

        }


        #endregion

        //adds the 'rectangle' layer to the map
        //public void AddRectangleLayer()
        //{
        //    FeatureSet fs = new FeatureSet(FeatureTypes.Polygon);
        //    fs.Features.Add(new Coordinate[] {
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDraw(MapDrawArgs e)
        {
            if (_isDragging) // don't draw anything unless we need to draw a select rectangle
            {
                Rectangle r = Opp.RectangleFromPoints(_startPoint, _currentPoint);
                r.Width -= 1;
                r.Height -= 1;
                e.Graphics.DrawRectangle(Pens.White, r);
                e.Graphics.DrawRectangle(_selectionPen, r);
            }

            if (_isFinished) //draw the extent rectangle
            {
                Rectangle r = Opp.RectangleFromPoints(_startPoint, _currentPoint);
                r.Width -= 2;
                r.Height -= 2;
                e.Graphics.DrawRectangle(Pens.White, r);
                e.Graphics.DrawRectangle(_selectionPen, r);

                Brush fillBrush = new SolidBrush(Color.FromArgb(100, Color.LightBlue));
                e.Graphics.FillRectangle(fillBrush, r);
                fillBrush.Dispose();
            }

            base.OnDraw(e);
        }

        /// <summary>
        /// Handles the MouseDown 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(GeoMouseArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _startPoint = e.Location;
                _geoStartPoint = e.GeographicLocation;
                _isDragging = true;
                _isFinished = false;
            }
            base.OnMouseDown(e);
        }

        /// <summary>
        /// Handles MouseMove
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(GeoMouseArgs e)
        {

            _currentPoint = e.Location;
            if (_isDragging) Map.Invalidate();
            base.OnMouseMove(e);
        }

        /// <summary>
        /// Handles the Mouse Up situation
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(GeoMouseArgs e)
        {
            if (_isDragging == false) return;
            _currentPoint = e.Location;
            _isDragging = false;
            
            _geoEndPoint = new Coordinate(e.GeographicLocation.X, e.GeographicLocation.Y);

            IEnvelope env = new Envelope(_geoStartPoint.X, e.GeographicLocation.X, _geoStartPoint.Y, e.GeographicLocation.Y);
            
            if (Math.Abs(_startPoint.X - e.X) < 16 && Math.Abs(_startPoint.Y - e.Y) < 16)
            {
                Coordinate c1 = e.Map.PixelToProj(new System.Drawing.Point(e.X - 8, e.Y - 8));
                Coordinate c2 = e.Map.PixelToProj(new System.Drawing.Point(e.X + 8, e.Y + 8));
            }

            _isFinished = true;
            
            e.Map.MapFrame.Initialize();
            base.OnMouseUp(e);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _startPoint = new System.Drawing.Point(0, 0);
            _currentPoint = new System.Drawing.Point(0, 0);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            Map.MapFrame.Initialize();
        }
    }
}
