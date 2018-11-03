using System;
using System.Collections.Generic;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Symbology;
using Hydrodesktop.Common;
using DotSpatial.Data;
using HydroDesktop.Plugins.Search.Settings;
using System.Diagnostics;

namespace HydroDesktop.Plugins.Search.Searching
{
    class SearchLayerCreator
    {
        #region Fields

        private readonly IMap _map;
        private readonly SearchResult _searchResult;
        private readonly SearchSettings _searchSettings;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="SearchLayerCreator"/>
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="searchResult">Search result</param>
        /// <param name="searchSettings">Seacrh settings </param>
        public SearchLayerCreator(IMap map, SearchResult searchResult, SearchSettings searchSettings)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (searchResult == null) throw new ArgumentNullException("searchResult");
            if (searchSettings == null) throw new ArgumentNullException("searchSettings");

            _map = map;
            _searchResult = searchResult;
            _searchSettings = searchSettings;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create search layer
        /// </summary>
        public IEnumerable<IMapPointLayer> Create()
        {
            var ext = new Extent();
            Debug.WriteLine("Starting Create method");
            if (!_searchResult.ResultItems.Any())
            {
                Debug.WriteLine("Returning new point layer");
                return new List<IMapPointLayer>();
            }

            Debug.WriteLine("Getting data sites layer...");
            var root = _map.GetDataSitesLayer(true);
            Debug.WriteLine("Done");

            var layersToSelect = new List<MapPointLayer>();
            var result = new List<IMapPointLayer>();
            Debug.WriteLine("Starting loop, count: " + _searchResult.ResultItems.Count());
            foreach(var item in _searchResult.ResultItems)
            {
                try
                {
                    Debug.WriteLine("creating search result layer");
                    var subResultLayer = CreateSearchResultLayer(item, root);
                    Debug.WriteLine("Done; adding subResultLayer to list of result layers");
                    result.Add(subResultLayer);
                    Debug.WriteLine("Done; adding subResultLayer to root");
                    root.Add(subResultLayer);
                    Debug.WriteLine("Done; adding subResultLayer to layersToSelect");
                    layersToSelect.Add(subResultLayer);
                    Debug.WriteLine("Done with loop iteration");
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Exception: " + e.Message);
                    Debug.WriteLine(e.StackTrace);
                }
            }
            Debug.WriteLine("Loop Done, refreshing map");
            _map.Refresh();
            Debug.WriteLine("Done");

            Debug.WriteLine("Starting another loop");
            //assign the projection again
            foreach (var item in _searchResult.ResultItems)
            {
                item.FeatureSet.Reproject(_map.Projection);
                ext.ExpandToInclude(item.FeatureSet.Extent);
            }
            Debug.WriteLine("Loop done. Now looping through layers: " + root.Layers.Count);
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
            Debug.WriteLine("End loop");

            ext.ExpandBy(_map.ViewExtents.Width / 100);
            _map.ViewExtents = ext;
            _map.Refresh();

            Debug.WriteLine("Return result");
            return result;
        }

        #endregion

        #region Private methods

        private MapPointLayer CreateSearchResultLayer(SearchResultItem item, IMapGroup root)
        {
            Debug.WriteLine("Starting method CreateSearchResultLayer");
            var myLayer = new MapPointLayer(item.FeatureSet);

            // Get Title of web-server
            var webService = _searchSettings.WebServicesSettings.WebServices.FirstOrDefault(
                ws => ws.ServiceCode == item.ServiceCode);
            var defaulLegendText = webService != null? webService.Title : item.ServiceCode;

            // Build legend text 
            var legendText = defaulLegendText;
            int nameIndex = 1;
            Debug.WriteLine("Starting while loop.");
            while(true)
            {
                // Check if legend text is already used
                var nameUsed = root.Layers.Any(layer => layer.LegendText == legendText);
                if (!nameUsed)
                {
                    Debug.WriteLine("Exiting while loop");
                    break;
                }

                // Create new legend text
                nameIndex++;
                legendText = string.Format("{0} ({1})", defaulLegendText, nameIndex);
            }

            myLayer.LegendText = legendText;
            Debug.WriteLine("Returning myLayer");
            return myLayer;
        }
       
        #endregion
    }
}
