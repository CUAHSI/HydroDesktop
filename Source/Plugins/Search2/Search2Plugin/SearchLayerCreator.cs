using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using HydroDesktop.Controls.Themes;

namespace HydroDesktop.Search
{
    class SearchLayerCreator : SymbologyCreator
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
            : base(Global.GetHISCentralURL())
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
        /// <returns>Search layer</returns>
        public MapGroup Create()
        {
            var root = GetSearchResultLayerGroup() ?? new MapGroup(_map, Global.SEARCH_RESULT_LAYER_NAME);

            //var creator = new SymbologyCreator(Global.GetHISCentralURL());
            foreach(var item in _searchResult.Features)
            {
                var subResultLayer = CreateSearchResultLayer(item.Key, item.Value);
                //SetUpLabeling(subResultLayer);

                // Set up symbology
                //SetUpSymbology(laySearchResult);

                subResultLayer.LegendText = item.Key;

                root.Add(subResultLayer);

                /*
                var subResultLayer = creator.CreateSearchResultLayer(item.Value);
                SetUpLabeling(subResultLayer);

                // Set up symbology
                //SetUpSymbology(laySearchResult);

                subResultLayer.LegendText = item.Key;

                root.Add(subResultLayer);*/
            }
            //_map.Invalidate();
            _map.Refresh();

            return root;
        }

        #endregion

        #region Private methods

        private MapPointLayer CreateSearchResultLayer(string servCode, IFeatureSet fs)
        {
            var myLayer = new MapPointLayer(fs) {LegendText = "data series"};

            var scheme = new PointScheme();
            scheme.ClearCategories();

            //TODO: implement me
            var settings = scheme.EditorSettings;
            settings.ClassificationType = ClassificationType.Quantities;
            settings.FieldName = "ValueCount";
            settings.UseSizeRange = true;
            settings.StartSize = 5.0;
            settings.EndSize = 25.0;
            settings.RampColors = false;
            scheme.CreateCategories(fs.DataTable);
            var myImage = GetImageForService(servCode);
            foreach(var categorie in scheme.Categories)
            {
                categorie.Symbolizer = new PointSymbolizer(myImage, categorie.Symbolizer.GetSize().Height);
                categorie.SelectionSymbolizer.SetFillColor(Color.Yellow);
            }
            myLayer.Symbology = scheme;

            //VALUECOUNT] > 415 AND [VALUECOUNT] <= 571

            /*
            //assign the categories (could be done with 'editorSettings')
            foreach (var servCode in serviceCodes)
            {
                string filterEx = "[ServiceCode] = '" + servCode + "'";
                var myImage = GetImageForService(servCode);
                var mySymbolizer = new PointSymbolizer(myImage, 14.0);
                var myCategory = new PointCategory(mySymbolizer)
                                     {
                                         FilterExpression = filterEx,
                                         LegendText = servCode,
                                         SelectionSymbolizer = new PointSymbolizer(myImage, 16.0)
                                     };
                myCategory.SelectionSymbolizer.SetFillColor(Color.Yellow);
                myScheme.AddCategory(myCategory);
            }
            myLayer.Symbology = myScheme;
             */ 

            SetUpLabeling(myLayer);

            return myLayer;
        }


        /// <summary>
        /// Create the 'Search Results' layer with image symbology
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public override MapPointLayer CreateSearchResultLayer(IFeatureSet fs)
        {
            return base.CreateSearchResultLayer(fs);
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

        private static void SetUpSymbology(IFeatureLayer layer)
        {
            Debug.Assert(layer != null);

            var _original = layer.Symbology;
            _original.SuspendEvents();

            var scheme = _original.Copy();

            var settings = scheme.EditorSettings;
            settings.ClassificationType = ClassificationType.Quantities;
            settings.FieldName = "ValueCount";
            settings.UseSizeRange = true;
            settings.StartSize = 5.0;
            settings.EndSize = 25.0;
            settings.RampColors = true;
            scheme.CreateCategories(layer.DataSet.DataTable);
            //---

            _original.CopyProperties(scheme);
            _original.ResumeEvents();
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
