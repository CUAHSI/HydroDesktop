using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Common.Tools;
using HydroDesktop.DataDownload.Downloading;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Interfaces.PluginContracts;
using HydroDesktop.WebServices;

namespace HydroDesktop.DataDownload.SearchLayersProcessing
{
    /// <summary>
    /// Class contains methods for modifying "search layer"
    /// </summary>
    class SearchLayerModifier
    {
        #region Fields

        private readonly FeatureLayer _layer;
        private readonly Map _map;
        private readonly DataDownloadPlugin _downloadPlugin;

        private static readonly string[] _searchColumns = new[]
                                                       {
                                                           "SiteCode", "VarCode", "ServiceCode", "ServiceURL", "StartDate",
                                                           "EndDate", "ValueCount"
                                                       };

        #endregion

        #region Constructors

        private SearchLayerModifier(FeatureLayer layer, Map map, DataDownloadPlugin downloadPlugin)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            if (map == null) throw new ArgumentNullException("map");
            if (downloadPlugin == null) throw new ArgumentNullException("downloadPlugin");
            Contract.EndContractBlock();

            _layer = layer;
            _map = map;
            _downloadPlugin = downloadPlugin;
        }

        #endregion

        #region Public methods

        public static SearchLayerModifier Create(ILayer layer, Map map, DataDownloadPlugin downloadPlugin)
        {
            if (!IsSearchLayer(layer)) return null;
            return new SearchLayerModifier((FeatureLayer)layer, map, downloadPlugin);
        }

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

            var layerColumns = featureLayer.DataSet.GetColumns();

            return
                _searchColumns.Select(sColumn => layerColumns.Any(dataColumn => dataColumn.ColumnName == sColumn)).All(
                    hasColumn => hasColumn);
        }

        public static bool LayerHaveDownlodedData(IFeatureLayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            Contract.EndContractBlock();

            if (!IsSearchLayer(layer)) return false;
            
            return layer.DataSet.DataTable.Columns.Contains("SeriesID") &&
                   layer.DataSet.Features.Any(f => f.DataRow["SeriesID"] != DBNull.Value);
        }

        public void UpdateContextMenu()
        {
            var searchLayer = _layer;
            var dataGroupMenu = searchLayer.ContextMenuItems.FirstOrDefault(item => item.Name == "Data");
            if (dataGroupMenu == null)
                return;

            if (LayerHaveDownlodedData(searchLayer))
            {
                var exportPlugin = _downloadPlugin.App.Extensions.OfType<IDataExportPlugin>().FirstOrDefault();
                if (exportPlugin != null)
                {
                    dataGroupMenu.AddMenuItem("Export Time Series Data", delegate { exportPlugin.Export(searchLayer); });
                }
                dataGroupMenu.AddMenuItem("Update Values from Server",
                                          delegate { _downloadPlugin.StartDownloading(searchLayer); });
            }
        }

        public void UpdateDataTable(IFeatureSet downloadedFeatureSet, DownloadManager downloadManager)
        {
            var searchLayer = _layer;

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
                foreach (var series in dInfo.ResultSeries)
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
                    searchFeature.DataRow["QCLevel"] != DBNull.Value)
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
                foreach (var series in seriesToProcess)
                {
                    var sFeature = searchLayer.DataSet.Features.FirstOrDefault(
                       feature =>
                                  (string)feature.DataRow["SiteCode"] == series.Site.Code &&
                                  (string)feature.DataRow["VarCode"] == series.Variable.Code &&
                                  (string)feature.DataRow["VarName"] == series.Variable.Name &&
                                  (string)feature.DataRow["DataType"] == series.Variable.DataType &&
                                  feature.DataRow["Method"] != DBNull.Value && (string)feature.DataRow["Method"] == series.Method.Description &&
                                  feature.DataRow["QCLevel"] != DBNull.Value && (string)feature.DataRow["QCLevel"] == series.QualityControlLevel.Definition
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

        public void UpdateSymbolizing()
        {
            var layer = _layer;
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

        public void UpdateLabeling()
        {
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

            _map.AddLabels(_layer, string.Format("[{0}]", attributeName),
                                          string.Format("[ValueCount] > {0}", 10),
                                          symb, "Category Default");
            _layer.ShowLabels = false;
            
        }

        #endregion

        #region Private methods

        private void UpdateFeatureFromFeature(IFeature searchFeature, IFeatureSet featureSet, Series series)
        {
            // Find downloaded feature
            var downloadedFeature = featureSet.Features.FirstOrDefault(feature =>
                                (string)feature.DataRow["SiteCode"] == series.Site.Code &&
                                (string)feature.DataRow["VarCode"] == series.Variable.Code &&
                                (string)feature.DataRow["VarName"] == series.Variable.Name &&
                                (string)feature.DataRow["DataType"] == series.Variable.DataType &&
                                (feature.DataRow["Method"] == DBNull.Value ? null : feature.DataRow["Method"].ToString()) == series.Method.Description &&
                                (string)feature.DataRow["QCLevel"] == series.QualityControlLevel.Definition);
            if (downloadedFeature == null) return;

            // updating...
            foreach (DataColumn column in featureSet.DataTable.Columns)
            {
                // Do not update ServiceURL
                if (column.ColumnName == "ServiceURL") continue;
                
                searchFeature.DataRow[column.ColumnName] = downloadedFeature.DataRow[column.ColumnName];
            }
        }

        private IPointScheme CreateSymbology(string servCode, IFeatureSet featureSet)
        {
            Debug.Assert(featureSet != null);

            var scheme = new PointScheme();
            scheme.ClearCategories();

            var settings = scheme.EditorSettings;
            settings.ClassificationType = ClassificationType.Custom;
            settings.IntervalMethod = IntervalMethod.Manual;

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
            if (minValue == int.MaxValue || minValue <= 0) minValue = 1;
            if (maxValue < minValue) maxValue = minValue + 1;

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
            
            var image = ServiceIconHelper.Instance.GetImageForService(servCode);

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

                var legendText = string.Format("({0} - {1}]", min, max);
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
            scheme.AppearsInLegend = true;
            scheme.LegendText = "Number of Observations";

            return scheme;
        }

        #endregion
    }
}
