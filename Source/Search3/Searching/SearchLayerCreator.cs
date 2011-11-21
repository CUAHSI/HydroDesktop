using System;
using System.Collections.Generic;
using System.Diagnostics;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using System.Linq;

namespace Search3.Searching
{
    class SearchLayerCreator
    {
        #region Fields

        private readonly IMap _map;
        private readonly SearchResult _searchResult;
        private readonly string _searchGroupName;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="SearchLayerCreator"/>
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="searchResult">Search result</param>
        /// <param name="searchGroupName">Name of search layers group.</param>
        public SearchLayerCreator(IMap map, SearchResult searchResult, string searchGroupName)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (searchResult == null) throw new ArgumentNullException("searchResult");
            if (searchGroupName == null) throw new ArgumentNullException("searchGroupName");

            _map = map;
            _searchResult = searchResult;
            _searchGroupName = searchGroupName;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create search layer
        /// </summary>
        public void Create()
        {
            if (_searchResult.ResultItems.Count() == 0) return;

            var root = GetSearchResultLayerGroup() ?? new MapGroup(_map, _searchGroupName);

            var layersToSelect = new List<MapPointLayer>();
            foreach(var item in _searchResult.ResultItems)
            {
                var subResultLayer = CreateSearchResultLayer(item, root);
                root.Add(subResultLayer);
                layersToSelect.Add(subResultLayer);
            }
            _map.Refresh();

            //assign the projection again
            foreach (var item in _searchResult.ResultItems)
                item.FeatureSet.Reproject(_map.Projection);

            for (int i = 0; i < root.Layers.Count; i++)
            {
                var layer = root[i];
                var state = layersToSelect.Contains(layer);
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

        private MapPointLayer CreateSearchResultLayer(SearchResultItem item, MapGroup root)
        {
            var myLayer = new MapPointLayer(item.FeatureSet);

            // Get Title of web-server
            var webService = Settings.SearchSettings.Instance.WebServicesSettings.WebServices.FirstOrDefault(
                ws => ws.ServiceCode == item.SeriesDataCart.ServCode);
            var defaulLegendText = webService != null? webService.Title : item.SeriesDataCart.ServCode;

            // Build legend text 
            var legendText = defaulLegendText;
            int nameIndex = 1;
            while(true)
            {
                // Check if legend text is already used
                var nameUsed = root.Layers.Any(layer => layer.LegendText == legendText);
                if (!nameUsed)
                {
                    break;
                }

                // Create new legend text
                nameIndex++;
                legendText = string.Format("{0} ({1})", defaulLegendText, nameIndex);
            }

            myLayer.LegendText = legendText;
            return myLayer;
        }
        
        private MapGroup GetSearchResultLayerGroup()
        {
            MapGroup layer = null;
            foreach (var lay in _map.Layers)
            {
                if (lay is MapGroup &&
                    lay.LegendText.ToLower() == _searchGroupName.ToLower())
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
