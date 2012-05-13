using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using Hydrodesktop.Common;

namespace Search3.Searching
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
        public IEnumerable<IMapPointLayer> Create()
        {
            if (!_searchResult.ResultItems.Any())
            {
                return new List<IMapPointLayer>();
            }

            var root = _map.GetDataSitesLayer(true);

            var layersToSelect = new List<MapPointLayer>();
            var result = new List<IMapPointLayer>();
            foreach(var item in _searchResult.ResultItems)
            {
                var subResultLayer = CreateSearchResultLayer(item, root);
                result.Add(subResultLayer);
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
            return result;
        }

        #endregion

        #region Private methods

        private MapPointLayer CreateSearchResultLayer(SearchResultItem item, IMapGroup root)
        {
            var myLayer = new MapPointLayer(item.FeatureSet);

            // Get Title of web-server
            var webService = Settings.SearchSettings.Instance.WebServicesSettings.WebServices.FirstOrDefault(
                ws => ws.ServiceCode == item.ServiceCode);
            var defaulLegendText = webService != null? webService.Title : item.ServiceCode;

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
       
        #endregion
    }
}
