using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Controls.Themes;

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
            var root = GetSearchResultLayerGroup() ?? new MapGroup(_map, Global.SEARCH_RESULT_LAYER_NAME);
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
           // myLayer.Symbology = CreateSymbology(servCode, featureSet);
           // SetUpLabeling(myLayer);

            return myLayer;
        }

        private IPointScheme CreateSymbology(string servCode, IFeatureSet featureSet)
        {
            var scheme = new PointScheme();
            scheme.ClearCategories();

            var settings = scheme.EditorSettings;
            settings.ClassificationType = ClassificationType.Custom;

            const string valueField = "ValueCount";

            // Find min/max value in valueField 
            var minValue = int.MaxValue;
            var maxValue = int.MinValue;
            //foreach (var feature in featureSet.Features)
            foreach (DataRow row in featureSet.DataTable.Rows)
            {
                int value;
                try
                {
                    value = (int)row[valueField];
                }
                catch
                {
                    value = 0;
                }
                if (value < minValue)
                    minValue = value;
                if (value > maxValue)
                    maxValue = value;
            }
            if (minValue == int.MaxValue) minValue = 0;
            if (maxValue == int.MinValue) maxValue = 0;

            // Calculate number of categories
            int categoriesCount; 
            var length = maxValue - minValue;
            if (length < 50)
                categoriesCount = 1;
            else if (length < 100)
                categoriesCount = 2;
            else
                categoriesCount = 3;

            var categorieStep = (maxValue - minValue) / categoriesCount + 1;    // value step in filter
            const int imageStep = 5;
            var imageSize = 5;

            var symbCreator = new  SymbologyCreator(Global.GetHISCentralURL()); // we need it only to get image
            var image = symbCreator.GetImageForService(servCode);
            for (int i = 0; i < categoriesCount; i++)
            {
                var min = minValue - 1;
                var max = minValue + categorieStep;
                if (max > maxValue)
                    max = maxValue;
                minValue = max + 1;

                imageSize += imageStep;

                var filterEx = string.Format("[{0}] > {1} and [{0}] <= {2}", valueField, min, max);
                var legendText = string.Format("({0}, {1}]", min, max);
                var mySymbolizer = new PointSymbolizer(image, imageSize);
                var myCategory = new PointCategory(mySymbolizer)
                {
                    FilterExpression = filterEx,
                    LegendText = legendText,
                    SelectionSymbolizer = new PointSymbolizer(image, imageSize + 2)
                };
                myCategory.SelectionSymbolizer.SetFillColor(Color.Yellow);
                scheme.AddCategory(myCategory);
            }

            return scheme;
        }

        private void SetUpLabeling(IFeatureLayer layer)
        {
            Debug.Assert(layer != null);

            const string attributeName = "SiteName";

            var symb = new LabelSymbolizer
            {
                FontColor = Color.Black,
                FontSize = 8,
                FontFamily = "Arial Unicode MS",
                PreventCollisions = true,
                HaloEnabled = true,
                HaloColor = Color.White,
                Orientation = ContentAlignment.MiddleRight,
                OffsetX = 0.0f,
                OffsetY = 0.0f,
            };

            _map.AddLabels(layer, string.Format("[{0}]", attributeName),
                                          string.Format("[ValueCount] > {0}", 10),
                                          symb, "Category Default");
        }

        private MapGroup GetSearchResultLayerGroup()
        {
            MapGroup layer = null;
            foreach (var lay in _map.Layers)
            {
                if (lay is MapGroup &&
                    lay.LegendText.ToLower() == Global.SEARCH_RESULT_LAYER_NAME.ToLower())
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
