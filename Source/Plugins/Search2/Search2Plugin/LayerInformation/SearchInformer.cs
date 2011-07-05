using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;

namespace HydroDesktop.Search.LayerInformation
{
    class SearchInformer
    {
        private Map _map;
        private IMapFeatureLayer _layer;

        public void Stop()
        {
            if (_map != null)
            {
                _map.GeoMouseMove -= _map_GeoMouseMove;
                _map.Layers.LayerRemoved -= Layers_LayerRemoved;
            }
        }

        public void Start(Map map, IMapFeatureLayer layer)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (layer == null) throw new ArgumentNullException("layer");

            Stop();

            _map = map;
            _layer = layer;

            _map.GeoMouseMove += _map_GeoMouseMove;
            _map.Layers.LayerRemoved += Layers_LayerRemoved;
        }

        void _map_GeoMouseMove(object sender, GeoMouseArgs e)
        {
            if (!_layer.IsVisible) return;

            var rtol = new Rectangle(e.X - 8, e.Y - 8, 0x10, 0x10);
            var tolerant = e.Map.PixelToProj(rtol);

            var pInfo = Identify(_layer, tolerant);
            if (pInfo == null) return;
            var info = pInfo.ToString();
            if (string.IsNullOrEmpty(info)) return;

            var toolTip = new ToolTip
            {
                IsBalloon = true,
                UseAnimation = false,
                ShowAlways = false,
            };

            toolTip.Show(info, _map, e.Location, 1000);
        }

        private static PointInfo Identify(IMapFeatureLayer layer, Extent tolerant)
        {
            Debug.Assert(layer != null);

            foreach (var feature in layer.DataSet.Select(tolerant))
            {
                if (feature.DataRow == null)
                {
                    feature.ParentFeatureSet.FillAttributes();
                }

                IFeature feature1 = feature;
                var getColumnValue = (Func<string, string>)(column => (feature1.DataRow[column].ToString()));

                var pInfo = new PointInfo();
                foreach (var fld in feature.ParentFeatureSet.GetColumns())
                {
                    switch (fld.ColumnName)
                    {
                        case "DataSource":
                            pInfo.DataSource = getColumnValue(fld.ColumnName);
                            break;
                        case "SiteName":
                            pInfo.SiteName = getColumnValue(fld.ColumnName);
                            break;
                        case "ValueCount":
                            pInfo.ValueCount = getColumnValue(fld.ColumnName);
                            break;
                    }
                }
                return pInfo;
            }

            return null;
        }

        void Layers_LayerRemoved(object sender, DotSpatial.Symbology.LayerEventArgs e)
        {
            if (e.Layer == _layer)
            {
                Stop();
            }
        }

        #region Nested types

        class PointInfo
        {
            public string DataSource { private get; set; }
            public string SiteName { private get; set; }
            public string ValueCount { private get; set; }

            public override string ToString()
            {
                return (
                           DataSource + Environment.NewLine +
                           SiteName + Environment.NewLine +
                           ValueCount
                       ).Trim();
            }
        }

        #endregion
    }
}
