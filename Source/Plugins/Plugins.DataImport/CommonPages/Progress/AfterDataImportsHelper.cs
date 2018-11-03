using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using Hydrodesktop.Common;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces.ObjectModel;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;

namespace HydroDesktop.Plugins.DataImport.CommonPages.Progress
{
    static class AfterDataImportsHelper
    {
        public static void CreateLayer(IList<Series> series, IProgressHandler ph, IWizardImporterSettings settings)
        {
            if (series.Count == 0)
            {
                ph.ReportProgress(100, "Finished.");
                return;
            }

            var progress = settings.MaxProgressPercentWhenImport + 1;

            var layerName = settings.LayerName;
            var layer = settings.Map.GetAllLayers()
                .OfType<IMapPointLayer>()
                .FirstOrDefault(l => l.LegendText == layerName);

            if (layer != null)
            {
                ph.ReportProgress(progress++, "Adding series to existing layer...");
                foreach (var s in series)
                {
                    AddSeriesToFeatureSet(s, layer.DataSet);
                }
            }
            else
            {
                ph.ReportProgress(progress++, "Creating map layer...");
                var featureSet = InitializeFeatureSet();
                featureSet.Projection = KnownCoordinateSystems.Geographic.World.WGS1984;
                foreach (var s in series)
                {
                    AddSeriesToFeatureSet(s, featureSet);
                }
                featureSet.Reproject(settings.Map.Projection);
               
                var fileName = Path.Combine(Settings.Instance.CurrentProjectDirectory,
                                            string.Format("{0}.shp", layerName));
                featureSet.Filename = fileName;
                featureSet.Save();
                featureSet = FeatureSet.Open(fileName); //re-open the featureSet from the file

                var myLayer = new MapPointLayer(featureSet)
                                  {
                                      LegendText = layerName
                                  };
                var dataSitesGroup = settings.Map.GetDataSitesLayer(true);
                dataSitesGroup.Add(myLayer);
            }

            ph.ReportProgress(progress, "Refreshing series selector...");
            settings.SeriesSelector.RefreshSelection();

            ph.ReportProgress(100, "Finished.");
        }

        private static IFeatureSet InitializeFeatureSet()
        {
            IFeatureSet fs = new FeatureSet(FeatureType.Point);
            var table = fs.DataTable;
            table.Columns.Add("DataSource", typeof(string));
            table.Columns.Add("SeriesID", typeof(int));
            table.Columns.Add("SiteName", typeof(string));
            table.Columns.Add("Latitude", typeof(double));
            table.Columns.Add("Longitude", typeof(double));
            table.Columns.Add("SiteCode", typeof(string));
            table.Columns.Add("VarName", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("SampleMed", typeof(string));
            table.Columns.Add("VarCode", typeof(string));
            table.Columns.Add("Units", typeof(string));
            table.Columns.Add("Method", typeof(string));
            table.Columns.Add("QCLevel", typeof(string));
            table.Columns.Add("ServiceCode", typeof(string));
            table.Columns.Add("StartDate", typeof(string));
            table.Columns.Add("EndDate", typeof(string));
            table.Columns.Add("ValueCount", typeof(int));
            return fs;
        }

        private static void AddSeriesToFeatureSet(Series series, IFeatureSet fs)
        {
            double lat = series.Site.Latitude;
            double lon = series.Site.Longitude;
            var pt = new Point(lon, lat);
            var newFeature = fs.AddFeature(pt);

            var featureRow = newFeature.DataRow;
            featureRow["DataSource"] = series.Source.Organization;
            featureRow["SeriesID"] = series.Id;
            featureRow["SiteName"] = series.Site.Name;
            featureRow["Latitude"] = series.Site.Latitude;
            featureRow["Longitude"] = series.Site.Longitude;
            featureRow["SiteCode"] = series.Site.Code;
            featureRow["VarName"] = series.Variable.Name;
            featureRow["DataType"] = series.Variable.DataType;
            featureRow["SampleMed"] = series.Variable.SampleMedium;
            featureRow["VarCode"] = series.Variable.Code;
            featureRow["Units"] = series.Variable.VariableUnit.Name;
            featureRow["Method"] = series.Method.Description;
            featureRow["QCLevel"] = series.QualityControlLevel.Definition;
            featureRow["StartDate"] = ConvertTime(series.BeginDateTime);
            featureRow["EndDate"] = ConvertTime(series.EndDateTime);
            featureRow["ValueCount"] = series.ValueCount;
            featureRow["ServiceCode"] = series.Variable.Code;
        }

        private static string ConvertTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }
    }
}