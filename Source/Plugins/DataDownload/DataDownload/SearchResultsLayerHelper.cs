using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Xml;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Controls.Themes;
using HydroDesktop.DataDownload.LayerInformation;
using HydroDesktop.DataDownload.WebServices;

namespace HydroDesktop.DataDownload
{
    static class SearchResultsLayerHelper
    {
        #region Fields

        private static readonly WebServicesList _webServicesList = new WebServicesList();
        private static XmlDocument _webServList;

        #endregion

        #region Public methods

        /// <summary>
        /// Check layer for search attributes
        /// </summary>
        /// <param name="layer">Layer to check</param>
        /// <returns>True - layer is search layer, otherwise - false.</returns>
        /// <exception cref="ArgumentNullException"><para>layer</para> must be not null.</exception>
        public static bool IsSearchLayer(ILayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");

            var featureLayer = layer as IMapFeatureLayer;
            if (featureLayer == null) return false;

            if (featureLayer is PointLayer) return true;

            var searchColumns = new[] { "DataSource", "ServiceURL", "SiteCode", "VarCode", "StartDate", "EndDate", "ValueCount" };
            var layerColumns = featureLayer.DataSet.GetColumns();
            
            foreach (var sColumn in searchColumns)
            {
                var hasColumn = layerColumns.Any(dataColumn => dataColumn.ColumnName == sColumn);
                if (!hasColumn)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Add some specific features to search results layer 
        /// </summary>
        /// <param name="layer">Search results layer</param>
        /// <param name="map">Map</param>
        /// <exception cref="ArgumentNullException"><para>layer</para>, <para>map</para> must be not null.</exception>
        public static void AddCustomFeaturesToSearchLayer(IFeatureLayer layer, Map map)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            if (map == null) throw new ArgumentNullException("map");

            UpdateSymbolizing(layer, map);
            AttachPopup(layer, map);
        }

        #endregion

        #region Private methods

        private static Dictionary<string, string> _services;
        private static Dictionary<string, string> Services
        {
            get
            {
                if (_webServList == null)
                    _webServList = _webServicesList.GetWebServicesList(false, true);
                if (_services == null || _services.Count == 0)
                    _services = GetServices(_webServList.DocumentElement);
                return _services;
            }
        }

        private static void AttachPopup(IFeatureLayer layer, Map map)
        {
            var extractor = new HISCentralInfoExtractor(Services);
            var searchInformer = new SearchLayerInformer(extractor);
            searchInformer.Start(map, (IMapFeatureLayer)layer);
        }

        private static Dictionary<string, string> GetServices(XmlNode node)
        {
            var res = new Dictionary<string, string>();

            foreach (XmlNode childNode1 in node.ChildNodes)
            {
                if (childNode1.Name != "ServiceInfo") continue;
                var nodeInfo = new NodeInfo();
                foreach (XmlNode childNode2 in childNode1.ChildNodes)
                {
                    if (childNode2.Name == "ServiceDescriptionURL")
                    {
                        nodeInfo.DescriptionUrl = childNode2.InnerText;
                    }
                    else if (childNode2.Name == "servURL")
                    {
                        nodeInfo.ServiceUrl = childNode2.InnerText;
                    }
                }
                try
                {
                    res.Add(nodeInfo.ServiceUrl, nodeInfo.DescriptionUrl);
                }catch
                {
                    continue;
                }
            }

            return res;
        }

        private static void UpdateSymbolizing(IFeatureLayer layer, IMap map)
        {
            Debug.Assert(layer != null);
            Debug.Assert(map != null);

            SetUpLabeling(layer, map);
            
            if (layer.DataSet.NumRows() > 0 && 
                layer.DataSet.GetColumn("DataSource") != null)
            {
                // assume that layer has same data source in all rows
                var servCode = layer.DataSet.GetFeature(0).DataRow["DataSource"].ToString();
                layer.Symbology = CreateSymbology(servCode, layer.DataSet);
            }
        }

        private static void SetUpLabeling(IFeatureLayer layer, IMap map)
        {
            Debug.Assert(layer != null);
            Debug.Assert(map != null);

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

            map.AddLabels(layer, string.Format("[{0}]", attributeName),
                                          string.Format("[ValueCount] > {0}", 10),
                                          symb, "Category Default");
        }

        private static IPointScheme CreateSymbology(string servCode, IFeatureSet featureSet)
        {
            Debug.Assert(featureSet != null);

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

            var symbCreator = new SymbologyCreator(Global.GetHISCentralURL()); // we need it only to get image
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

        #endregion

        #region Helpers

        class NodeInfo
        {
            public String DescriptionUrl { get; set; }
            public String ServiceUrl { get; set; }
        }

        #endregion
    }
}
