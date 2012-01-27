using System;
using System.ComponentModel;
using System.Windows.Forms;
using DotSpatial.Symbology;
using HydroDesktop.Common;
using HydroDesktop.Common.Tools;

namespace HydroDesktop.DataDownload.DataAggregation.UI
{
    /// <summary>
    /// Settings form for aggregation
    /// </summary>
    public partial class AggregationSettingsDialog : Form
    {
        #region Fields

        private readonly IFeatureLayer _layer;
        private readonly AggregationSettings _settings;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new instance of <see cref="AggregationSettingsDialog"/>
        /// </summary>
        /// <param name="layer"></param>
        public AggregationSettingsDialog(IFeatureLayer layer)
        {
            InitializeComponent();

            _layer = layer;
            _settings = new AggregationSettings();

            // Set bindings
            cmbMode.DataSource = Enum.GetValues(typeof(AggregationMode));
            cmbMode.Format += delegate(object sender, ListControlConvertEventArgs args)
                                  {
                                      args.Value = ((AggregationMode) args.ListItem).Description();
                                  };
            cmbMode.DataBindings.Clear();
            cmbMode.DataBindings.Add("SelectedItem", _settings, "AggregationMode", true,
                                     DataSourceUpdateMode.OnPropertyChanged);

            dtpStartTime.DataBindings.Clear();
            dtpStartTime.DataBindings.Add("Value", _settings, "StartTime", true, DataSourceUpdateMode.OnPropertyChanged);

            dtpEndTime.DataBindings.Clear();
            dtpEndTime.DataBindings.Add("Value", _settings, "EndTime", true, DataSourceUpdateMode.OnPropertyChanged);

            // Set initial StartTime, EndTime
            var minStartTime = DateTime.MaxValue;
            var maxEndTime = DateTime.MinValue;

            foreach (var feature in layer.DataSet.Features)
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
        }

        #endregion

        #region Private methods

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetControlsToCalculation();

            var worker = new BackgroundWorker {WorkerReportsProgress = true};
            worker.ProgressChanged += delegate(object o, ProgressChangedEventArgs args)
                                          {
                                              pbProgress.Value = args.ProgressPercentage;
                                              pbProgress.Text = args.UserState != null
                                                                    ? args.UserState.ToString()
                                                                    : string.Empty;
                                          };
            worker.DoWork += delegate
                                 {
                                     var aggregator = new Aggregator(_settings, _layer)
                                                          {
                                                              ProgressHandler = new ProgressHandler(worker),
                                                          };
                                     aggregator.Calculate();
                                 };
            worker.RunWorkerCompleted += delegate
                                             {
                                                 DialogResult = DialogResult.OK;
                                                 Close();
                                             };
            worker.RunWorkerAsync();
        }

        private void SetControlsToCalculation()
        {
            paSettings.Enabled = false;
            pbProgress.Visible = true;
            btnOK.Enabled = btnCancel.Enabled = false;
        }

        #endregion

        #region Nested types

        private class ProgressHandler : IProgressHandler
        {
            private readonly BackgroundWorker _parent;

            public ProgressHandler(BackgroundWorker parent)
            {
                _parent = parent;
            }

            public void ReportProgress(int persentage, object state)
            {
                if (_parent.WorkerReportsProgress)
                {
                    _parent.ReportProgress(persentage, state);
                }
            }

            public void CheckForCancel()
            {
                throw new NotImplementedException();
            }

            public void ReportMessage(string message)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
