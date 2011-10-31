using System;
using DotSpatial.Data;
using DotSpatial.Projections;
using HydroDesktop.WebServices;

namespace Search3.Settings
{
    public class AreaSettings
    {
        public bool HasAnyArea
        {
            get
            {
                return AreaRectangle != null ||
                       (Polygons != null && Polygons.Features.Count > 0);
            }
        }

        private Box _areaRectangle;
        public Box AreaRectangle
        {
            get { return _areaRectangle; }
            private set
            {
                _areaRectangle = value;
                RaiseAreaRectangleChanged();
            }
        }
        public ProjectionInfo RectangleProjection { get; private set; }

        public void SetAreaRectangle(Box areaRectangle, ProjectionInfo rectangleProjection)
        {
            RectangleProjection = rectangleProjection;
            AreaRectangle = areaRectangle;
        }

        private FeatureSet _polygons;
        public FeatureSet Polygons
        {
            get { return _polygons; }
            set
            {
                _polygons = value;
                RaisePolygonsChanged();
            }
        }

        public event EventHandler AreaRectangleChanged;
        public event EventHandler PolygonsChanged;

        private void RaiseAreaRectangleChanged()
        {
            var handler = AreaRectangleChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
        private void RaisePolygonsChanged()
        {
            var handler = PolygonsChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}