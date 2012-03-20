using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces.ObjectModel;
using Hydrodesktop.Common;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;

namespace DataImport.CommonPages.Progress
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
            ph.ReportProgress(progress++, "Creating map layer...");

            var featureSet = InitializeFeatureSet();
            featureSet.Projection = KnownCoordinateSystems.Geographic.World.WGS1984;
            foreach (var s in series)
            {
                AddSeriesToFeatureSet(s, featureSet);
            }
            featureSet.Reproject(settings.Map.Projection);

            var legendText = string.Format("Imported Data ({0})", Path.GetFileNameWithoutExtension(settings.PathToFile));
            var fileName = Path.Combine(Settings.Instance.CurrentProjectDirectory,
                                        string.Format("{0}.shp", legendText));
            featureSet.Filename = fileName;
            featureSet.Save();
            featureSet = FeatureSet.Open(fileName); //re-open the featureSet from the file

            var myLayer = new MapPointLayer(featureSet)
                              {
                                  LegendText = legendText
                              };
            var dataSitesGroup = settings.Map.GetDataSitesLayer(true);
            dataSitesGroup.Add(myLayer);

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
            table.Columns.Add("VariableName", typeof(string));
            table.Columns.Add("DataType", typeof(string));
            table.Columns.Add("SampleMedium", typeof(string));
            table.Columns.Add("VariableCode", typeof(string));
            table.Columns.Add("Units", typeof(string));
            table.Columns.Add("Method", typeof(string));
            table.Columns.Add("QualityControl", typeof(string));
            table.Columns.Add("ServiceCode", typeof(string));
            table.Columns.Add("StartDate", typeof(string));
            table.Columns.Add("EndDate", typeof(string));
            table.Columns.Add("BeginDateTi", typeof(string)); // it need for data aggregation
            table.Columns.Add("EndDateTime", typeof(string)); // it need for data aggregation
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
            featureRow["VariableName"] = series.Variable.Name;
            featureRow["DataType"] = series.Variable.DataType;
            featureRow["SampleMedium"] = series.Variable.SampleMedium;
            featureRow["VariableCode"] = series.Variable.Code;
            featureRow["Units"] = series.Variable.VariableUnit.Name;
            featureRow["Method"] = series.Method.Description;
            featureRow["QualityControl"] = series.QualityControlLevel.Definition;
            featureRow["StartDate"] = ConvertTime(series.BeginDateTime);
            featureRow["EndDate"] = ConvertTime(series.EndDateTime);
            featureRow["BeginDateTi"] = ConvertTime(series.BeginDateTime);
            featureRow["EndDateTime"] = ConvertTime(series.EndDateTime);
            featureRow["ValueCount"] = series.ValueCount;
            featureRow["ServiceCode"] = series.Variable.Code;
        }

        private static string ConvertTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }
    }
}