using System;
using System.Diagnostics;
using System.Drawing;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using HydroDesktop.Controls.Themes;

namespace HydroDesktop.Search
{
    class SearchLayerCreator
    {
        #region Fields

        private readonly IMap _map;
        private readonly IFeatureSet _featureSet;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of <see cref="SearchLayerCreator"/>
        /// </summary>
        /// <param name="map">Map</param>
        /// <param name="featureSet">Feature set</param>
        public SearchLayerCreator(IMap map, IFeatureSet featureSet)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (featureSet == null) throw new ArgumentNullException("featureSet");

            _map = map;
            _featureSet = featureSet;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Create search layer
        /// </summary>
        /// <returns>Search layer</returns>
        public IMapPointLayer Create()
        {
            var hisCentralURL = Global.GetHISCentralURL();
            var creator = new SymbologyCreator(hisCentralURL);
            IMapPointLayer laySearchResult = creator.CreateSearchResultLayer(_featureSet);

            // Set up labels
            SetUpLabeling(laySearchResult);

            // Set up symbology
            SetUpSymbology(laySearchResult);

            laySearchResult.LegendText = Global.SEARCH_RESULT_LAYER_NAME;

            return laySearchResult;
        }

        #endregion

        #region Private methods

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
            _map.Refresh();
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

        #endregion
    }
}
