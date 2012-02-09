using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using HydroDesktop.Common.Tools;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using IProgressHandler = HydroDesktop.Common.IProgressHandler;
using PointShape = DotSpatial.Symbology.PointShape;

namespace HydroDesktop.DataDownload.DataAggregation.UI
{
    /// <summary>
    /// Settings form for aggregation
    /// </summary>
    public partial class AggregationSettingsDialog : Form, IProgressHandler
    {
        #region Fields

        private readonly IFeatureLayer _layer;
        private readonly AggregationSettings _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="AggregationSettingsDialog"/>
        /// </summary>
        /// <param name="layer">Layer to aggregate</param>
        public AggregationSettingsDialog(IFeatureLayer layer)
        {
            if (layer == null) throw new ArgumentNullException("layer");
            Contract.EndContractBlock();

            InitializeComponent();

            _layer = layer;
            _settings = new AggregationSettings();

            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs eventArgs)
        {
            // Set bindings
            cmbMode.DataSource = Enum.GetValues(typeof(AggregationMode));
            cmbMode.Format += delegate(object s, ListControlConvertEventArgs args)
            {
                args.Value = ((AggregationMode)args.ListItem).Description();
            };
            cmbMode.DataBindings.Clear();
            cmbMode.DataBindings.Add("SelectedItem", _settings, "AggregationMode", true,
                                     DataSourceUpdateMode.OnPropertyChanged);

            dtpStartTime.DataBindings.Clear();
            dtpStartTime.DataBindings.Add("Value", _settings, "StartTime", true, DataSourceUpdateMode.OnPropertyChanged);

            dtpEndTime.DataBindings.Clear();
            dtpEndTime.DataBindings.Add("Value", _settings, "EndTime", true, DataSourceUpdateMode.OnPropertyChanged);

            cmbVariable.DataBindings.Clear();
            cmbVariable.DataBindings.Add("SelectedItem", _settings, "VariableCode", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

            chbCreateNewLayer.DataBindings.Clear();
            chbCreateNewLayer.DataBindings.Add("Checked", _settings, "CreateNewLayer", true,
                                               DataSourceUpdateMode.OnPropertyChanged);

            // Set initial CreateNewLayer
            _settings.CreateNewLayer = true;

            // Set initial StartTime, EndTime
            var minStartTime = DateTime.MaxValue;
            var maxEndTime = DateTime.MinValue;

            foreach (var feature in _layer.DataSet.Features)
            {
                var startDateRow = feature.DataRow["StartDate"];
                var endDateRow = feature.DataRow["EndDate"];

                var startDate = Convert.ToDateTime(startDateRow);
                var endDate = Convert.ToDateTime(endDateRow);

                if (minStartTime > startDate)
                {
                    minStartTime = startDate;
                }
                if (maxEndTime < endDate)
                {
                    maxEndTime = endDate;
                }
            }
            if (minStartTime == DateTime.MaxValue)
            {
                minStartTime = DateTime.Now;
            }
            if (maxEndTime == DateTime.MinValue)
            {
                maxEndTime = DateTime.Now;
            }
            _settings.StartTime = minStartTime;
            _settings.EndTime = maxEndTime;

            // Get all variables associated with current layer
            var seriesRepo = RepositoryFactory.Instance.Get<IDataSeriesRepository>(DatabaseTypes.SQLite,
                                                                                   Settings.Instance.
                                                                                       DataRepositoryConnectionString);
            var uniqueVariables = new List<string>();
            foreach (var feature in _layer.DataSet.Features)
            {
                var seriesIDValue = feature.DataRow["SeriesID"];
                if (seriesIDValue == null || seriesIDValue == DBNull.Value)
                    continue;
                var seriesID = Convert.ToInt64(seriesIDValue);
                var series = seriesRepo.GetSeriesByID(seriesID);
                if (series == null) continue;

                var curVar = series.Variable.Code;
                if (!uniqueVariables.Contains(curVar))
                {
                    uniqueVariables.Add(curVar);
                }
            }
            if (uniqueVariables.Count > 0)
            {
                _settings.VariableCode = uniqueVariables[0];
            }
            cmbVariable.DataSource = uniqueVariables;

            //
            btnOK.Enabled = _layer.DataSet.Features.Count > 0;
        }

        #endregion

        #region Private methods

        private BackgroundWorker _backgroundWorker;

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetControlsToCalculation();

            _backgroundWorker = new BackgroundWorker { WorkerReportsProgress = true };
            _backgroundWorker.DoWork += delegate(object o, DoWorkEventArgs args)
                                 {
                                     var aggregator = new Aggregator(_settings, _layer)
                                                          {
                                                              ProgressHandler = this,
                                                              MaxPercentage = 97,
                                                          };
                                     args.Result = aggregator.Calculate();
                                 };
            _backgroundWorker.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args)
                                             {
                                                 if (args.Error != null)
                                                 {
                                                     MessageBox.Show("Error occured:" + Environment.NewLine + 
                                                                     args.Error.Message, "Aggregation",
                                                                     MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                 }
                                                 else
                                                 {
                                                     // This actions must be executed only in UI thread
                                                     var result = (AggregationResult) args.Result;
                                                     var featureSet = result.FeatureSet;
                                                     var columnName = result.ResultColumnName;

                                                     // Save updated data
                                                     ReportProgress(98, "Saving data");
                                                     Application.DoEvents();
                                                     if (!string.IsNullOrEmpty(featureSet.Filename))
                                                     {
                                                         featureSet.Save();
                                                     }
                                                     
                                                     if (_settings.CreateNewLayer)
                                                     {
                                                         ReportProgress(99, "Adding layer to map");
                                                         Application.DoEvents();
                                                         
                                                         var mapLayer = new MapPointLayer(featureSet) { LegendText = Path.GetFileNameWithoutExtension(featureSet.Filename) };
                                                         _layer.MapFrame.Add(mapLayer);
                                                         
                                                         UpdateLabeling(mapLayer, columnName);
                                                         UpdateSymbology(mapLayer, columnName);
                                                     }

                                                     ReportProgress(100, "Finished");
                                                     Application.DoEvents();

                                                     DialogResult = DialogResult.OK;
                                                     Close();   
                                                 }
                                             };
            _backgroundWorker.RunWorkerAsync();
        }

        private void UpdateLabeling(IFeatureLayer mapLayer, string columnName)
        {
            var symbolizer = new LabelSymbolizer
            {
                FontColor = Color.Black,
                FontSize = 8,
                FontFamily = "Arial Unicode MS",
                PreventCollisions = true,
                HaloEnabled = true,
                HaloColor = Color.White,
                Orientation = ContentAlignment.MiddleRight,
            };
            ((Map)((IMapFrame)_layer.MapFrame).Parent).AddLabels(mapLayer, string.Format("[{0}]", columnName), string.Empty, symbolizer, "Category Default");
        }

        private void UpdateSymbology(IFeatureLayer mapLayer, string columnName)
        {
            var scheme = new PointScheme();
            scheme.ClearCategories();

            var settings = scheme.EditorSettings;
            settings.ClassificationType = ClassificationType.Custom;

            var colors = new[]
                             {
                                 Color.Aqua, Color.Blue, Color.Brown, Color.Cyan, Color.Fuchsia, Color.LightSalmon,
                                 Color.Olive, Color.Wheat, Color.DodgerBlue
                             };
            
            // Find min/max value in valueField 
            var minValue = double.MaxValue;
            var maxValue = double.MinValue;
            foreach (DataRow row in mapLayer.DataSet.DataTable.Rows)
            {
                double value;
                try
                {
                    value = Convert.ToDouble(row[columnName]);
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
            const double EPSILON = 0.00001;
            if (Math.Abs(minValue - double.MaxValue) < EPSILON) minValue = 0.0;
            if (Math.Abs(maxValue - double.MinValue) < EPSILON) maxValue = 0.0;

            var fracLength = maxValue - minValue > 10? 0 : 1;
            
            // Set number of categories
            int categoriesCount  = 3;
            var categorieStep = (maxValue - minValue) / categoriesCount;    // value step in filter

            const int imageStep = 5;
            var imageSize = 10; // start image size

            var imageColor = colors[new Random().Next(0, colors.Length - 1)];
            for (int i = 0; i < categoriesCount; i++)
            {
                var min = minValue + categorieStep*i;
                var max = min + categorieStep;
                if (max >= maxValue)
                    max = maxValue + 1;

                min = Math.Round(min, fracLength);
                max = Math.Round(max, fracLength);

                imageSize += imageStep;
                var baseFilter = string.Format("[{0}] >= {1} and [{0}] < {2}", columnName,
                                               fracLength == 0 ? (int)min : min,
                                               fracLength == 0 ? (int)max : max);
                var legendText = string.Format("[{0}, {1})",
                                               fracLength == 0 ? (int)min : min,
                                               fracLength == 0 ? (int)max : max);
                var mySymbolizer = new PointSymbolizer(imageColor, PointShape.Ellipse, imageSize);
                var myCategory = new PointCategory(mySymbolizer)
                {
                    FilterExpression = baseFilter,
                    LegendText = legendText,
                };
                scheme.AddCategory(myCategory);
            }

            mapLayer.Symbology = scheme;
        }

        private void SetControlsToCalculation()
        {
            paSettings.Enabled = false;
            pbProgress.Visible = lblProgress.Visible = true;
            btnOK.Enabled = btnCancel.Enabled = false;
        }

        #endregion

        #region IProgressHandler implementation

        public void ReportProgress(int persentage, object state)
        {
            pbProgress.UIThread(() => pbProgress.Value = persentage);
            lblProgress.UIThread(() => lblProgress.Text = state != null ? state.ToString() : string.Empty);
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
            lblProgress.UIThread(() => lblProgress.Text = message);
        }

        #endregion
    }
}
