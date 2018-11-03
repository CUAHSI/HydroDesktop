using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Plugins.DataDownload.LayerInformation.PopupControl;
using HydroDesktop.Plugins.DataDownload.SearchLayersProcessing;

namespace HydroDesktop.Plugins.DataDownload.LayerInformation
{
    /// <summary>
    /// Provides access to services in search layer
    /// </summary>
    class SearchLayerInformer
    {
        #region Fields

        private readonly IServiceInfoExtractor _serviceInfoExtractor;
        private readonly Map _map;
        private readonly Popup toolTip;
        private readonly CustomToolTipControl customToolTip;
        private readonly DataDownloadPlugin _parentPlugin;

        #endregion

        #region Constructors

        /// <summary>
        /// Create instance of <see cref="SearchLayerInformer"/>
        /// </summary>
        /// <param name="parentPlugin">DataDownload plugin</param>
        /// <param name="serviceInfoExtractor">Instance of IServiceInfoExtractor</param>
        /// <param name="map">Map</param>
        /// <exception cref="ArgumentNullException">serviceInfoExtractor must be not null.</exception>
        public SearchLayerInformer(DataDownloadPlugin parentPlugin, IServiceInfoExtractor serviceInfoExtractor, Map map)
        {
            if (parentPlugin == null) throw new ArgumentNullException("parentPlugin");
            if (serviceInfoExtractor == null) throw new ArgumentNullException("serviceInfoExtractor");
            if (map == null) throw new ArgumentNullException("map");

            _parentPlugin = parentPlugin;
            _serviceInfoExtractor = serviceInfoExtractor;
            _map = map;

            toolTip = new Popup(customToolTip = new CustomToolTipControl(_parentPlugin));
            customToolTip.Popup = toolTip;
            toolTip.AutoClose = false;
            toolTip.FocusOnOpen = false;
            toolTip.ShowingAnimation = toolTip.HidingAnimation = PopupAnimations.Blend;
            toolTip.TopLevel = false;
            
            _parentPlugin.ShowPopupsChanged += OnParentPluginOnShowPopupsChanged;
            _map.MouseMove += _map_MouseMove;
            _map.VisibleChanged += MapOnVisibleChanged;
            var parent = _map.Parent;
            while (parent != null)
            {
                parent.VisibleChanged += MapOnVisibleChanged;
                var form = parent as Form;
                if (form != null)
                {
                    toolTip.Parent = form;
                }
                parent = parent.Parent;
            }
            Debug.Assert(toolTip.Parent != null);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Deactivate SearchLayerInformer
        /// </summary>
        public void Deactivate()
        {
            _map.MouseMove -= _map_MouseMove;
            _map.VisibleChanged -= MapOnVisibleChanged;
            var parent = _map.Parent;
            while (parent != null)
            {
                parent.VisibleChanged -= MapOnVisibleChanged;
                parent = parent.Parent;
            }
            _parentPlugin.ShowPopupsChanged -= OnParentPluginOnShowPopupsChanged;
        }
    
        #endregion

        #region Private methods

        private void OnParentPluginOnShowPopupsChanged(object sender, EventArgs e)
        {
            if (!_parentPlugin.ShowPopups)
            {
                HideToolTip();
            }
        }
      
        private void MapOnVisibleChanged(object sender, EventArgs eventArgs)
        {
            var control = sender as Control;
            if (control != null && (!control.Visible))
            {
                HideToolTip();
            }
        }

        void _map_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_parentPlugin.ShowPopups)
            {
                return;
            }

            var visibleLayers = _map.GetAllLayers().OfType<IFeatureLayer>()
                                                   .Where(layer => layer.IsVisible && SearchLayerModifier.IsSearchLayer(layer))
                                                   .ToList();
            if (visibleLayers.Count == 0)
            {
                HideToolTip();
                return;
            }

            var rtol = new Rectangle(e.X - 8, e.Y -8, 0x10, 0x10);
            var tolerant = _map.PixelToProj(rtol);

            var pInfos = visibleLayers.Select(layer => Identify(layer, tolerant))
                                      .Where(pInfo => !pInfo.IsEmpty).ToList();

            if (pInfos.Count == 0)
            {
                HideToolTip();
                return;
            }

            // If already visible same tooltip, not show again
            var control = (CustomToolTipControl) toolTip.Content;
            if (toolTip.Visible && control.IsInfoAlreadySetted(pInfos))
                return;

            HideToolTip();
            control.SetInfo(pInfos);

            Point adjustedLocation = adjustPopupLocation(e.Location);
            toolTip.Show(_map, toolTip.Parent.PointToClient(adjustedLocation));
        }

        //Method used to adjust the location of the tooltip popup in case it goes outside of the map window.
        private Point adjustPopupLocation(Point start)
        {
            int tempX = start.X;
            int tempY = start.Y;

            //If popup goes over right edge of map pull it to the left.
            if (toolTip.Width + start.X > _map.Right)
            {
                tempX = tempX - ((toolTip.Width + start.X) - _map.Right);
            }
            //If popup goes below the edge of the map, make it appear above the cursor.
            if (toolTip.Height + start.Y > _map.Bottom)
            {
                tempY = tempY - toolTip.Height;
            }

            Point adjustedPoint = new Point(tempX, tempY);
            return adjustedPoint;
        }

        private void HideToolTip()
        {
            toolTip.Close();
        }

        private ServiceInfoGroup Identify(IFeatureLayer layer, Extent tolerant)
        {
            Debug.Assert(layer != null);

            var group = new ServiceInfoGroup();
            foreach (var feature in layer.DataSet.Select(tolerant))
            {
                if (feature.DataRow == null)
                {
                    feature.ParentFeatureSet.FillAttributes();
                }
                var pInfo = ClassConvertor.IFeatureToServiceInfo(feature, layer);
                var serviceDescription = _serviceInfoExtractor.GetServiceDesciptionUrl(pInfo.ServiceUrl);
                if (serviceDescription != null)
                {
                    pInfo.ServiceDesciptionUrl = serviceDescription;
                }
                group.AddOverlappedServiceInfo(pInfo);
            }

            return group;
        }

        #endregion
    }
}
