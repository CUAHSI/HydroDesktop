using System;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using HydroDesktop.Search.LayerInformation.PopupControl;

namespace HydroDesktop.Search.LayerInformation
{
    class SearchInformer
    {
        #region Fields

        private readonly IServiceInfoExtractor _serviceInfoExtractor;
        private Map _map;
        private IMapFeatureLayer _layer;
        private readonly Popup toolTip;
        private readonly CustomToolTipControl customToolTip;

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

            toolTip = new Popup(customToolTip = new CustomToolTipControl());
            customToolTip.Popup = toolTip;
            toolTip.AutoClose = true;
            toolTip.FocusOnOpen = false;
            toolTip.ShowingAnimation = toolTip.HidingAnimation = PopupAnimations.Blend;
        }

        private Popup ToolTip
        {
            get { return toolTip; }
        }
 
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
            var toolTipPointInfo = ((CustomToolTipControl)ToolTip.Content).PointInfo;
            if (ToolTip.Visible && toolTipPointInfo.Equals(pInfo))
                return;

            HideToolTip();

            toolTipPointInfo.DataSource = pInfo.DataSource;
            toolTipPointInfo.SiteName = pInfo.SiteName;
            toolTipPointInfo.ValueCount = pInfo.ValueCount;
            toolTipPointInfo.ServiceDesciptionUrl = pInfo.ServiceDesciptionUrl;
            
            ToolTip.Show(_map, e.Location);   
        }

        private void HideToolTip()
        {
            ToolTip.Close();
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
                            var value = getColumnValue(fld.ColumnName);
                            int val;
                            pInfo.ValueCount = !Int32.TryParse(value, out val) ? (int?) null : val;
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
    }
}
