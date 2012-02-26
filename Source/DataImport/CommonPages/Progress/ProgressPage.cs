using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using HydroDesktop.Common.Tools;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces.ObjectModel;
using Wizard.UI;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;

namespace DataImport.CommonPages.Progress
{
    public partial class ProgressPage : InternalWizardPage, IProgressHandler
    {
        #region Fields

        private readonly WizardContext _context;
        private BackgroundWorker _backgroundWorker;

        #endregion

        /// <summary>
        /// Create new instance of <see cref="ProgressPage"/>
        /// </summary>
        /// <param name="context"></param>
        public ProgressPage(WizardContext context)
        {
            _context = context;
            InitializeComponent();
            lblInfo.Text = string.Empty;
        }

        private void ProgressPage_SetActive(object sender, CancelEventArgs e)
        {
            SetWizardButtons(WizardButtons.None);

            _backgroundWorker = new BackgroundWorker {WorkerSupportsCancellation = true, WorkerReportsProgress = true};
            _backgroundWorker.RunWorkerCompleted += delegate(object s, RunWorkerCompletedEventArgs args)
                                         {
                                             CreateLayer((IList<Series>)args.Result);

                                             SetWizardButtons(WizardButtons.Next);
                                             PressButton(WizardButtons.Next);
                                         };
            _backgroundWorker.DoWork += delegate(object s, DoWorkEventArgs args)
                                            {
                                                var ph = (IProgressHandler) this;
                                                ph.ReportMessage("Reading all data into DataTable...");
                                                _context.Importer.UpdateData(_context.Settings);
                                                _context.Settings.MaxProgressPercentWhenImport = 97;
                                                var importer = _context.Importer.GetImporter();
                                                importer.ProgressHandler = ph;
                                                var result = importer.Import(_context.Settings);

                                                // Some work need in the UI thread
                                                args.Result = result;
                                            };
            
            _backgroundWorker.RunWorkerAsync();
        }

        private void CreateLayer(IList<Series> series)
        {
            var ph = (IProgressHandler)this;

            if (series.Count == 0)
            {
                ph.ReportProgress(100, "Finished.");
                return;
            }

            var progress = _context.Settings.MaxProgressPercentWhenImport + 1;
            ph.ReportProgress(progress++, "Creating map layer...");

            var featureSet = InitializeFeatureSet();
            featureSet.Projection = KnownCoordinateSystems.Geographic.World.WGS1984;
            foreach (var s in series)
            {
                AddSeriesToFeatureSet(s, featureSet);
            }
            featureSet.Reproject(_context.Settings.Map.Projection);

            var legendText = string.Format("Imported Data ({0})", Path.GetFileNameWithoutExtension(_context.Settings.PathToFile));
            var fileName = Path.Combine(Settings.Instance.CurrentProjectDirectory,
                                        string.Format("{0}.shp", legendText));
            featureSet.Filename = fileName;
            featureSet.Save();
            featureSet = FeatureSet.Open(fileName); //re-open the featureSet from the file

            var myLayer = new MapPointLayer(featureSet)
                              {
                                  LegendText = legendText
                              };
            _context.Settings.Map.Layers.Add(myLayer);

            ph.ReportProgress(progress, "Refreshing series selector...");
            _context.Settings.SeriesSelector.RefreshSelection();

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
            featureRow["ValueCount"] = series.ValueCount;
            featureRow["ServiceCode"] = series.Variable.Code;
        }

        private static  string ConvertTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd hh:mm", CultureInfo.InvariantCulture);
        }

        #region IProgressHandler implementation

        public void ReportProgress(int persentage, object state)
        {
            progressBar.UIThread(() => progressBar.Value = persentage);
            lblInfo.UIThread(() => lblInfo.Text = state != null ? state.ToString() : string.Empty);
        }

        public void CheckForCancel()
        {
            var bw = _backgroundWorker;
            if (bw != null && bw.WorkerSupportsCancellation)
            {
                bw.CancelAsync();
            }
        }

        public void ReportMessage(string message)
        {
            lblInfo.UIThread(() => lblInfo.Text = message);
        }

        #endregion
    }
}
