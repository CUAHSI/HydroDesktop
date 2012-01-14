using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using Hydrodesktop.Common;

namespace HydroDesktop.Search
{
    class SearchLayerCreator
    {
        #region Fields

        private readonly IMap _map;
        private readonly SearchResult _searchResult;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="SearchLayerCreator"/>
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="searchResult">Search result</param>
        public SearchLayerCreator(IMap map, SearchResult searchResult)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (searchResult == null) throw new ArgumentNullException("searchResult");

            _map = map;
            _searchResult = searchResult;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create search layer
        /// </summary>
        public void Create()
        {
            var root = GetSearchResultLayerGroup() ?? new MapGroup(_map, LayerConstants.SearchGroupName);
            foreach(var item in _searchResult.Features)
            {
                var subResultLayer = CreateSearchResultLayer(item.Key, item.Value, item.Key);
                root.Add(subResultLayer);
            }
            _map.Refresh();

            //assign the projection again
            foreach (var item in _searchResult.Features)
                item.Value.Reproject(_map.Projection);

            for (int i = 0; i < root.Layers.Count; i++)
            {
                var layer = root[i];
                var state = i == 0;
                var rendItem = layer as IRenderableLegendItem;
                if (rendItem != null)
                {
                    rendItem.IsVisible = state; // force a re-draw in the case where we are talking about layers.
                }
                else
                {
                    layer.Checked = state;
                }
            }

            _map.Refresh();
        }

        #endregion

        #region Private methods

        private MapPointLayer CreateSearchResultLayer(string servCode, IFeatureSet featureSet, string legendName)
        {
            var myLayer = new MapPointLayer(featureSet);
            myLayer.LegendText = legendName;
            return myLayer;
        }
        
        private MapGroup GetSearchResultLayerGroup()
        {
            MapGroup layer = null;
            foreach (var lay in _map.Layers)
            {
                if (lay is MapGroup &&
                    lay.LegendText.ToLower() == LayerConstants.SearchGroupName.ToLower())
                {
                    layer = lay as MapGroup;
                    break;
                }
            }
            return layer;
        }

        #endregion
    }
}
