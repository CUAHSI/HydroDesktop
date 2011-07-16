using System;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;

namespace HydroDesktop.Search.LayerInformation
{
    class SearchInformer
    {
        #region Fields

        private readonly IServiceInfoExtractor _serviceInfoExtractor;
        private Map _map;
        private IMapFeatureLayer _layer;
        private CustomToolTip _toolTip;

        #endregion

        /// <summary>
        /// Constuctor of <see cref="SearchInformer"/>
        /// </summary>
        /// <param name="serviceInfoExtractor">Instance of IServiceInfoExtractor</param>
        /// <exception cref="ArgumentNullException">serviceInfoExtractor must be not null.</exception>
        public SearchInformer(IServiceInfoExtractor serviceInfoExtractor)
        {
            if (serviceInfoExtractor == null) throw new ArgumentNullException("serviceInfoExtractor");
            _serviceInfoExtractor = serviceInfoExtractor;
        }

        private CustomToolTip ToolTip
        {
            get { return _toolTip ?? (_toolTip = new CustomToolTip()); }
        }

        public void Stop()
        {
            if (_map != null)
            {
                _map.GeoMouseMove -= _map_GeoMouseMove;
                _map.Layers.LayerRemoved -= Layers_LayerRemoved;
            }

            if (_layer != null)
            {
                _layer.VisibleChanged -= layer_VisibleChanged;
            }
        }

        public void Start(Map map, IMapFeatureLayer layer)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (layer == null) throw new ArgumentNullException("layer");

            Stop();

            _map = map;
            _layer = layer;
            
            _map.Layers.LayerRemoved += Layers_LayerRemoved;
            layer.VisibleChanged += layer_VisibleChanged;
            layer_VisibleChanged(layer, EventArgs.Empty);
        }

        void layer_VisibleChanged(object sender, EventArgs e)
        {
            if (!_layer.IsVisible)
            {
                HideToolTip();
                _map.GeoMouseMove -= _map_GeoMouseMove;
                return;
            }
            _map.GeoMouseMove -= _map_GeoMouseMove;
            _map.GeoMouseMove += _map_GeoMouseMove;
        }

        private void HideToolTip()
        {
            if (ToolTip.IsVisible)
            {
                ToolTip.Hide(_map);
            }
        }

        void _map_GeoMouseMove(object sender, GeoMouseArgs e)
        {
            if (!_layer.IsVisible)
            {
                return;
            }

            var rtol = new Rectangle(e.X - 8, e.Y - 8, 0x10, 0x10);
            var tolerant = e.Map.PixelToProj(rtol);

            var pInfo = Identify(_layer, tolerant);
            if (pInfo == null)
            {
                HideToolTip();
                return;
            }
            var info = pInfo.ToString();
            if (string.IsNullOrEmpty(info))
            {
                HideToolTip();
                return;
            }

            if (ToolTip.IsVisible && info == ToolTip.GetToolTip(_map)) return;
            
            ToolTip.Show(info, _map, e.Location);
        }
     
        private PointInfo Identify(IMapFeatureLayer layer, Extent tolerant)
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
                        case "ServiceURL":
                            pInfo.ServiceDesciptionUrl =
                                _serviceInfoExtractor.GetServiceDesciptionUrlByServiceUrl(getColumnValue(fld.ColumnName));
                            break;
                        /*case "ServiceID":
                            pInfo.ServiceDesciptionUrl =
                                _serviceInfoExtractor.GetServiceDescriptionUrlByServiceID(getColumnValue(fld.ColumnName));
                            break;*/
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
            public string ServiceDesciptionUrl { private get; set; }

            public override string ToString()
            {
                const string unknown = "Unknown";
                return string.Format("{1}{0}{2}{0}{3}{0}{4}",
                                     Environment.NewLine,
                                     string.IsNullOrWhiteSpace(DataSource) ? unknown + "DataSource" : DataSource,
                                     string.IsNullOrWhiteSpace(SiteName) ? unknown + "SiteName" : SiteName,
                                     string.IsNullOrWhiteSpace(ValueCount) ? unknown + " ValueCount" : ValueCount + " values",
                                     string.IsNullOrWhiteSpace(ServiceDesciptionUrl) ? unknown + " ServiceURL" : ServiceDesciptionUrl);
            }
        }

        #endregion
    }
}
