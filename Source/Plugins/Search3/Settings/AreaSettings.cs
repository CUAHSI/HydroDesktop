using System;
using DotSpatial.Data;

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

        private AreaRectangle _areaRectangle;
        public AreaRectangle AreaRectangle
        {
            get { return _areaRectangle; }
            set
            {
                _areaRectangle = value;
                RaiseAreaRectangleChanged();
            }
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