using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Configuration;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.DataDownload.LayerInformation;
using HydroDesktop.Interfaces;

namespace HydroDesktop.DataDownload.SearchLayersProcessing
{
    /// <summary>
    /// Class conatains methods for modifing "search layer"
    /// </summary>
    class SearchLayerModifier
    {

        #region Public methods

        /// <summary>
        /// Check layer for search attributes
        /// </summary>
        /// <param name="layer">Layer to check</param>
        /// <returns>True - layer is search layer, otherwise - false.</returns>
        /// <exception cref="ArgumentNullException"><para>layer</para> must be not null.</exception>
        /// <remarks>If layer is search layer, it can be casted at least to IFeatureLayer</remarks>
        public bool IsSearchLayer(ILayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");

            var featureLayer = layer as PointLayer;
            if (featureLayer == null) return false;

            var searchColumns = new[] { "SiteCode", "VarCode", "ServiceCode", "ServiceURL", "StartDate", "EndDate", "ValueCount" };
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
        /// Add some specific features to layer, if this layer is search results layer
        /// </summary>
        /// <param name="layer">Layer</param>
        /// <param name="map">Map</param>
        /// <exception cref="ArgumentNullException"><para>layer</para>, <para>map</para> must be not null.</exception>
        public bool AddCustomFeaturesToSearchLayer(ILayer layer, Map map)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            if (map == null) throw new ArgumentNullException("map");

            if (!IsSearchLayer(layer)) return false;

            

            SetUpLabeling((IFeatureLayer)layer, map);
            UpdateSymbolizing((IFeatureLayer)layer);
            AttachPopup((IFeatureLayer)layer, map);

            return true;
        }

        public void UpdateSearchLayerAfterDownloading(IFeatureLayer searchLayer, IFeatureSet downloadedFeatureSet, 
                                                      DownloadManager downloadManager)
        {
            if (searchLayer == null) throw new ArgumentNullException("searchLayer");
            if (downloadedFeatureSet == null) throw new ArgumentNullException("downloadedFeatureSet");
            if (downloadManager == null) throw new ArgumentNullException("downloadManager");

            UpdateDataTable(searchLayer, downloadedFeatureSet, downloadManager);
            UpdateSymbolizing(searchLayer);
        }

        public void RemoveCustomFeaturesFromLayer(ILayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            if (!IsSearchLayer(layer)) return;

            var feature = (IFeatureLayer) layer;
            SearchLayerInformer infrormer;
            if (_searchInformersPerLayes.TryGetValue(feature, out infrormer))
            {
                infrormer.Stop();
                _searchInformersPerLayes.Remove(feature);
            }
        }

        #endregion

        #region Private methods

        private void UpdateDataTable(IFeatureLayer searchLayer, IFeatureSet downloadedFeatureSet, DownloadManager downloadManager)
        {
            // Add all columns from downloadedFeatureSet, wich not exists in searchLayer
            foreach (DataColumn column in downloadedFeatureSet.DataTable.Columns)
            {
                if (!searchLayer.DataSet.DataTable.Columns.Contains(column.ColumnName))
                {
                    var copy = new DataColumn(column.ColumnName, column.DataType);
                    searchLayer.DataSet.DataTable.Columns.Add(copy);
                }
            }

            // Update values in search layer to corresponding from downloaded features
            foreach (var dInfo in downloadManager.GetSavedData())
            {
                var searchFeature = dInfo.SourceFeature;
                var series = dInfo.ResultSeries.First();

                // Find downloaded feature
                var downloadedFeature = downloadedFeatureSet.Features.FirstOrDefault(feature =>
                                    (string)feature.DataRow["SiteCode"] == series.Site.Code &&
                                    (string)feature.DataRow["VariableCod"] == series.Variable.Code &&
                                    (string)feature.DataRow["VariableNam"] == series.Variable.Name &&
                                    (string)feature.DataRow["DataType"] == series.Variable.DataType &&
                                    (string)feature.DataRow["Method"] == series.Method.Description &&
                                    (string)feature.DataRow["QualityCont"] == series.QualityControlLevel.Definition);


                if (downloadedFeature == null) continue;

                // updating...
                foreach (DataColumn column in downloadedFeatureSet.DataTable.Columns)
                {
                    searchFeature.DataRow[column.ColumnName] = downloadedFeature.DataRow[column.ColumnName];
                }
            }

            // Save update data into file
            if (!string.IsNullOrEmpty(searchLayer.DataSet.Filename))
            {
                searchLayer.DataSet.Save();
            }
        }

        private static Dictionary<string, string> _services;
        private static Dictionary<string, string> Services
        {
            get
            {
                if (_services == null || _services.Count == 0)
                {
                    _services = new Dictionary<string, string>();

                    var wss = Global.PluginEntryPoint.App.Extensions.OfType<IWebServicesStore>().FirstOrDefault();
                    if (wss != null)
                    {
                        var infos = wss.GetWebServices();
                        if (infos != null)
                        {
                            foreach (var info in infos)
                            {
                                _services.Add(info.EndpointURL, info.DescriptionURL);
                            }
                        }
                    }
                }
                return _services;
            }
        }

        private readonly Dictionary<IFeatureLayer, SearchLayerInformer> _searchInformersPerLayes = new Dictionary<IFeatureLayer, SearchLayerInformer>();

        private void AttachPopup(IFeatureLayer layer, Map map)
        {
            var extractor = new HISCentralInfoExtractor(Services);
            var searchInformer = new SearchLayerInformer(extractor);
            if (_searchInformersPerLayes.ContainsKey(layer)) return;

            searchInformer.Start(map, layer);
            _searchInformersPerLayes.Add(layer, searchInformer);
        }

        private static void UpdateSymbolizing(IFeatureLayer layer)
        {
            Debug.Assert(layer != null);
            
            if (layer.DataSet.NumRows() > 0 && 
                layer.DataSet.GetColumn("ServiceCode") != null)
            {
                // assume that layer has same data source in all rows
                var servCode = layer.DataSet.GetFeature(0).DataRow["ServiceCode"].ToString();
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
                    value = Convert.ToInt32(row[valueField]);
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

            var imageHelper = new HydroDesktop.WebServices.ServiceIconHelper(Settings.Instance.SelectedHISCentralURL); // we need it only to get image
            var image = imageHelper.GetImageForService(servCode);

            const string seriesID = "SeriesID";
            var needDownloadedCategories = featureSet.DataTable.Columns.Contains(seriesID);

            for (int i = 0; i < categoriesCount; i++)
            {
                var min = minValue - 1;
                var max = minValue + categorieStep;
                if (max > maxValue)
                    max = maxValue;
                minValue = max + 1;

                imageSize += imageStep;

                var baseFilter = string.Format("[{0}] > {1} and [{0}] <= {2}", valueField, min, max);

                var filterEx = needDownloadedCategories
                                   ? baseFilter + string.Format(" AND ([{0}] is null)", seriesID)
                                   : baseFilter;
                    
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

                // add categorie for downloaded
                if (needDownloadedCategories)
                {
                    mySymbolizer = new PointSymbolizer(image, imageSize);
                    mySymbolizer.SetOutline(Color.Green, 3);

                    filterEx = string.Format("{0} AND not([{1}] is null)", baseFilter, seriesID);
                    legendText = myCategory.LegendText + " (downloaded)";
                    var categorieForDownload = new PointCategory(mySymbolizer)
                    {
                        FilterExpression = filterEx,
                        LegendText = legendText,
                        SelectionSymbolizer = new PointSymbolizer(image, imageSize + 2)
                    };
                    categorieForDownload.SelectionSymbolizer.SetFillColor(Color.Yellow);
                    scheme.AddCategory(categorieForDownload);
                }
            }

            return scheme;
        }

        #endregion
    }
}
