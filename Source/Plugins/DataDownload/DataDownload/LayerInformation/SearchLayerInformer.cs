using System;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using HydroDesktop.DataDownload.LayerInformation.PopupControl;

namespace HydroDesktop.DataDownload.LayerInformation
{
    /// <summary>
    /// Provides access to services in search layer
    /// </summary>
    class SearchLayerInformer
    {
        #region Fields

        private readonly IServiceInfoExtractor _serviceInfoExtractor;
        private Map _map;
        private IMapFeatureLayer _layer;
        private readonly Popup toolTip;
        private readonly CustomToolTipControl customToolTip;

        #endregion

        #region Constructors

        /// <summary>
        /// Create instance of <see cref="SearchLayerInformer"/>
        /// </summary>
        /// <param name="serviceInfoExtractor">Instance of IServiceInfoExtractor</param>
        /// <exception cref="ArgumentNullException">serviceInfoExtractor must be not null.</exception>
        public SearchLayerInformer(IServiceInfoExtractor serviceInfoExtractor)
        {
            if (serviceInfoExtractor == null) throw new ArgumentNullException("serviceInfoExtractor");
            _serviceInfoExtractor = serviceInfoExtractor;

            toolTip = new Popup(customToolTip = new CustomToolTipControl());
            customToolTip.Popup = toolTip;
            toolTip.AutoClose = false;
            toolTip.FocusOnOpen = false;
            toolTip.ShowingAnimation = toolTip.HidingAnimation = PopupAnimations.Blend;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Stop engine
        /// </summary>
        public void Stop()
        {
            if (_map != null)
            {
                _map.MouseMove -= _map_MouseMove;
                _map.Layers.LayerRemoved -= Layers_LayerRemoved;
            }

            if (_layer != null)
            {
                _layer.VisibleChanged -= layer_VisibleChanged;
            }
        }

        /// <summary>
        /// Start engine
        /// </summary>
        /// <param name="map">Map to process</param>
        /// <param name="layer">Layer to process</param>
        /// <exception cref="ArgumentNullException"><para>map</para>, <para>layer</para> must be not null.</exception>
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

        #endregion

        #region Private methods

        void layer_VisibleChanged(object sender, EventArgs e)
        {
            if (!_layer.IsVisible)
            {
                HideToolTip();
                _map.MouseMove -= _map_MouseMove;
                return;
            }
            _map.MouseMove -= _map_MouseMove;
            _map.MouseMove += _map_MouseMove;
        }

        void _map_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!_layer.IsVisible)
            {
                return;
            }

            var rtol = new Rectangle(e.X - 8, e.Y - 8, 0x10, 0x10);
            var tolerant = _map.PixelToProj(rtol);

            var pInfo = Identify(_layer, tolerant);
            if (pInfo == null || pInfo.IsEmpty)
            {
                HideToolTip();
                return;
            }

            // If already visible same tooltip, not show again
            var toolTipPointInfo = ((CustomToolTipControl)toolTip.Content).PointInfo;
            if (toolTip.Visible && toolTipPointInfo.Equals(pInfo))
                return;

            HideToolTip();

            toolTipPointInfo.DataSource = pInfo.DataSource;
            toolTipPointInfo.SiteName = pInfo.SiteName;
            toolTipPointInfo.ValueCount = pInfo.ValueCount;
            toolTipPointInfo.ServiceDesciptionUrl = pInfo.ServiceDesciptionUrl;
            toolTipPointInfo.EndDate = pInfo.EndDate;
            toolTipPointInfo.Latitude = pInfo.Latitude;
            toolTipPointInfo.Longitude = pInfo.Longitude;
            toolTipPointInfo.ServiceUrl = pInfo.ServiceUrl;
            toolTipPointInfo.SiteCode = pInfo.SiteCode;
            toolTipPointInfo.StartDate = pInfo.StartDate;
            toolTipPointInfo.VarCode = pInfo.VarCode;
            toolTipPointInfo.VarName = pInfo.VarName;
            
            toolTip.Show(_map, e.Location);
        }

        private void HideToolTip()
        {
            toolTip.Close();
        }

        private ServiceInfo Identify(IMapFeatureLayer layer, Extent tolerant)
        {
            Debug.Assert(layer != null);

            foreach (var feature in layer.DataSet.Select(tolerant))
            {
                if (feature.DataRow == null)
                {
                    feature.ParentFeatureSet.FillAttributes();
                }

                IFeature feature1 = feature;
                var getColumnValue = (Func<string, string>) (column => (feature1.DataRow[column].ToString()));

                var pInfo = new ServiceInfo();
                foreach (var fld in feature.ParentFeatureSet.GetColumns())
                {
                    var strValue = getColumnValue(fld.ColumnName);

                    switch (fld.ColumnName)
                    {
                        case "DataSource":
                            pInfo.DataSource = strValue;
                            break;
                        case "SiteName":
                            pInfo.SiteName = strValue;
                            break;
                        case "SiteCode":
                            pInfo.SiteCode = strValue;
                            break;
                        case "VarCode":
                            pInfo.VarCode = strValue;
                            break;
                        case "ValueCount":
                            {
                                int val;
                                pInfo.ValueCount = !Int32.TryParse(strValue, out val) ? (int?) null : val;
                            }
                            break;
                        case "ServiceURL":
                            pInfo.ServiceUrl = strValue;
                            pInfo.ServiceDesciptionUrl =
                                _serviceInfoExtractor.GetServiceDesciptionUrl(pInfo.ServiceUrl);
                            break;
                        case "StartDate":
                            {
                                DateTime val;
                                pInfo.StartDate = !DateTime.TryParse(strValue, out val) ? DateTime.MinValue : val;
                            }
                            break;
                        case "EndDate":
                            {
                                DateTime val;
                                pInfo.EndDate = !DateTime.TryParse(strValue, out val) ? DateTime.MinValue : val;
                            }
                            break;
                        case "VarName":
                            pInfo.VarName = strValue;
                            break;
                        case "Latitude":
                            {
                                double val;
                                pInfo.Latitude = !Double.TryParse(strValue, out val) ? 0 : val;
                            }
                            break;
                        case "Longitude":
                            {
                                double val;
                                pInfo.Longitude = !Double.TryParse(strValue, out val) ? 0 : val;
                            }
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

        #endregion
    }
}
