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
using HydroDesktop.DataDownload.DataAggregation.UI;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;

namespace HydroDesktop.DataDownload.SearchLayersProcessing
{
    /// <summary>
    /// Class contains methods for modifying "search layer"
    /// </summary>
    class SearchLayerModifier
    {
        private readonly Map _map;

        public SearchLayerModifier(Map map)
        {
            if (map == null) throw new ArgumentNullException("map");

            _map = map;
        }

        #region Public methods

        /// <summary>
        /// Check layer for search attributes
        /// </summary>
        /// <param name="layer">Layer to check</param>
        /// <returns>True - layer is search layer, otherwise - false.</returns>
        /// <exception cref="ArgumentNullException"><para>layer</para> must be not null.</exception>
        /// <remarks>If layer is search layer, it can be casted at least to IFeatureLayer</remarks>
        public static bool IsSearchLayer(ILayer layer)
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
        /// <exception cref="ArgumentNullException"><para>layer</para>, <para>map</para> must be not null.</exception>
        public void AddCustomFeaturesToSearchLayer(IFeatureLayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");

            if (!IsSearchLayer(layer)) return;

            SetUpLabeling(layer, _map);
            UpdateSymbolizing(layer);
        }

        public void UpdateSearchLayerAfterDownloading(IFeatureLayer searchLayer, IFeatureSet downloadedFeatureSet, 
                                                      DownloadManager downloadManager)
        {
            if (searchLayer == null) throw new ArgumentNullException("searchLayer");
            if (downloadedFeatureSet == null) throw new ArgumentNullException("downloadedFeatureSet");
            if (downloadManager == null) throw new ArgumentNullException("downloadManager");

            UpdateDataTable(searchLayer, downloadedFeatureSet, downloadManager);
            UpdateSymbolizing(searchLayer);
            UpdateContextMenu(searchLayer);
        }

        private void UpdateContextMenu(IFeatureLayer searchLayer)
        {
            const string dataGroupName = "Data";
            var dataGroupMenu = searchLayer.ContextMenuItems.FirstOrDefault(item => item.Name == dataGroupName);
            if (dataGroupMenu == null)
                return;

            const string exportTimeSeries = "Export Time Series Data";
            if (!dataGroupMenu.MenuItems.Exists(item => item.Name == exportTimeSeries))
            {
                var exportPlugin = Global.PluginEntryPoint.App.Extensions.OfType<IDataExportPlugin>().FirstOrDefault();
                // TODO: replace line above with MEF if need
                if (exportPlugin != null)
                {
                    var menuItem = new SymbologyMenuItem(exportTimeSeries, delegate { exportPlugin.Export(searchLayer); });
                    dataGroupMenu.MenuItems.Add(menuItem);
                }
            }

            const string showDataValuesInMap = "Show Data Values in Map";
            if (!dataGroupMenu.MenuItems.Exists(item => item.Name == showDataValuesInMap))
            {
                var menuItem = new SymbologyMenuItem(showDataValuesInMap, delegate
                                                                              {
                                                                                  new AggregationSettingsDialog(searchLayer).ShowDialog();
                                                                              });
                dataGroupMenu.MenuItems.Add(menuItem);
            }
            
        }

        public void RemoveCustomFeaturesFromLayer(ILayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            if (!IsSearchLayer(layer)) return;

            //todo: Undo of SetUpLabeling, UpdateSymbolizing
        }

        #endregion

        #region Private methods

        private void UpdateDataTable(IFeatureLayer searchLayer, IFeatureSet downloadedFeatureSet, DownloadManager downloadManager)
        {
            // Add all columns from downloadedFeatureSet, which not exists in searchLayer
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
                var seriesToProcess = new List<Series>();
                // Find all series with different Method/QualityControlLevel
                foreach(var series in dInfo.ResultSeries)
                {
                    if (!seriesToProcess.Exists(s => s.Site.Code == series.Site.Code &&
                                                     s.Variable.Code == series.Variable.Code &&
                                                     s.Method.Description == series.Method.Description &&
                                                     s.QualityControlLevel.Definition == series.QualityControlLevel.Definition))
                    {
                        seriesToProcess.Add(series);
                    }
                }

                var searchFeature = dInfo.SourceFeature;
                Series firstSeries;
                if (searchFeature.DataRow["Method"] != DBNull.Value &&
                    searchFeature.DataRow["QualityCont"] != DBNull.Value)
                {
                    firstSeries = seriesToProcess.First(s =>
                                                        (string)searchFeature.DataRow["SiteCode"] == s.Site.Code &&
                                                        ((string)searchFeature.DataRow["VarCode"]).StartsWith(s.Variable.Code) &&
                                                        (string)searchFeature.DataRow["DataType"] == s.Variable.DataType);
                }
                else
                    firstSeries = seriesToProcess.First();
                seriesToProcess.Remove(firstSeries);
                
                UpdateFeatureFromFeature(searchFeature, downloadedFeatureSet, firstSeries);

                // Additional series...
                foreach(var series in seriesToProcess)
                {
                    var sFeature = searchLayer.DataSet.Features.FirstOrDefault(
                       feature =>
                                  (string)feature.DataRow["SiteCode"] == series.Site.Code &&
                                  (string)feature.DataRow["VarCode"] == series.Variable.Code &&
                                  (string)feature.DataRow["VarName"] == series.Variable.Name &&
                                  (string)feature.DataRow["DataType"] == series.Variable.DataType &&
                                  feature.DataRow["Method"] != DBNull.Value && (string)feature.DataRow["Method"] == series.Method.Description &&
                                  feature.DataRow["QualityCont"] != DBNull.Value && (string)feature.DataRow["QualityCont"] == series.QualityControlLevel.Definition
                       );
                    // If no such feature in Search shapeFile, then add it...
                    if (sFeature == null) 
                    {
                        sFeature = searchFeature.Copy();
                        searchLayer.DataSet.Features.Add(sFeature);
                    }

                    UpdateFeatureFromFeature(sFeature, downloadedFeatureSet, series);
                }
            }

            // Save update data into file
            if (!string.IsNullOrEmpty(searchLayer.DataSet.Filename))
            {
                searchLayer.DataSet.Save();
            }
        }

        private void UpdateFeatureFromFeature(IFeature searchFeature, IFeatureSet featureSet, Series series)
        {
            // Find downloaded feature
            var downloadedFeature = featureSet.Features.FirstOrDefault(feature =>
                                (string)feature.DataRow["SiteCode"] == series.Site.Code &&
                                (string)feature.DataRow["VariableCod"] == series.Variable.Code &&
                                (string)feature.DataRow["VariableNam"] == series.Variable.Name &&
                                (string)feature.DataRow["DataType"] == series.Variable.DataType &&
                                (string)feature.DataRow["Method"] == series.Method.Description &&
                                (string)feature.DataRow["QualityCont"] == series.QualityControlLevel.Definition);
            if (downloadedFeature == null) return;

            // updating...
            foreach (DataColumn column in featureSet.DataTable.Columns)
            {
                searchFeature.DataRow[column.ColumnName] = downloadedFeature.DataRow[column.ColumnName];
            }
        }
     
        private static void UpdateSymbolizing(IFeatureLayer layer)
        {
            Debug.Assert(layer != null);
            
            if (layer.DataSet.NumRows() > 0 && 
                layer.DataSet.GetColumn("ServiceCode") != null)
            {
                // assume that layer has same data source in all rows
                var servCode = layer.DataSet.GetFeature(0).DataRow["ServiceCode"].ToString();
                var symb = CreateSymbology(servCode, layer.DataSet);

                if (layer.DataSet.FeatureLookup.Count > 0 &&
                    layer.DataSet.FeatureLookup.Count != layer.DataSet.Features.Count)
                {
                    // todo: Sometimes it's true. I don't know why, need additional investigation (Maxim).
                    layer.DataSet.FeatureLookup.Clear();
                    // If not clear FeatureLookup, then exception raised in next line
                }

                layer.Symbology = symb;
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

            var imageHelper = new WebServices.ServiceIconHelper(Settings.Instance.SelectedHISCentralURL); // we need it only to get image
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

                // add category for downloaded
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
