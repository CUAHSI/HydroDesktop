using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.Search.Download
{
    /// <summary>
    /// DownloadManager UI implementation
    /// </summary>
    internal partial class DownloadManagerUI : Form
    {
        #region Fields

        private readonly DownloadManager _manager;
        private bool _closeAfterCompleted;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of DownloadManagerUI
        /// </summary>
        /// <param name="manager">Instance of DownloadManager</param>
        /// <exception cref="ArgumentNullException">manager must be not null</exception>
        public DownloadManagerUI(DownloadManager manager)
        {
            InitializeComponent();

            if (!DesignMode)
            {
                if (manager == null) throw new ArgumentNullException("manager");
            }
            
            lblTotalInfo.Text = string.Empty;

            _manager = manager;
            _manager.ProgressChanged += _manager_ProgressChanged;
            _manager.Completed += _manager_Completed;
            _manager.Canceled += _manager_Canceled;
            _manager.OnMessage += _manager_OnMessage;

            FormClosing += DownloadManagerUI_FormClosing;

            BindDownloadInfoTable();
            BindDownloadProgressInfo();
            ShowHideDetails(); // by default details is not shown
            dgvDownloadData.CellFormatting += dgvDownloadData_CellFormatting;

            //
            btnSendError.Enabled = false;
        }

        #endregion

        #region Private methods

        void dgvDownloadData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0 ||
                dgvDownloadData.Columns[e.ColumnIndex].DataPropertyName != "Status") return;

            var blist = (BindingList<DownloadInfo>)dgvDownloadData.DataSource;
            var dInfo = blist[e.RowIndex];

            if (dInfo.Status == DownloadInfoStatus.Error)
            {
                e.CellStyle.BackColor = Color.Red;
            }
            else if (dInfo.Status == DownloadInfoStatus.Ok)
            {
                e.CellStyle.BackColor = Color.Green;
            }
            else if (dInfo.Status == DownloadInfoStatus.OkWithWarnings)
            {
                e.CellStyle.BackColor = Color.Orange;
            }
        }


        private void BindDownloadProgressInfo()
        {
            var dpInfo = _manager.DownloadProgressInfo; // to avoid long names
            if (dpInfo == null)
            {
                lcDownloadedAndSavedInfo.Text = null;
                lcWithErrorInfo.Text = null;
                lcTotalSeriesInfo.Text = null;
                lcRemainingSeriesInfo.Text = null;
                lcEstimatedTimeInfo.Text = null;
                return;
            }

            dpInfo.PropertyChanged += dpInfo_PropertyChanged;
            // Set current values
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("DownloadedAndSaved"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("WithError"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("TotalSeries"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("RemainingSeries"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("EstimatedTime"));

            // Cross thread databinding not works correct
            /* 
            lcDownloadedAndSavedInfo.DataBindings.Add(new Binding("Text", dpInfo, "DownloadedAndSaved"));
            lcWithErrorInfo.DataBindings.Add(new Binding("Text", dpInfo, "WithError"));
            lcTotalSeriesInfo.DataBindings.Add(new Binding("Text", dpInfo, "TotalSeries"));
            lcRemainingSeriesInfo.DataBindings.Add(new Binding("Text", dpInfo, "RemainingSeries"));
            lcEstimatedTimeInfo.DataBindings.Add(new Binding("Text", dpInfo, "EstimatedTime"));
             */
        }

        void dpInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var dpInfo = _manager.DownloadProgressInfo; // to avoid long names
            if (e.PropertyName == "DownloadedAndSaved")
            {
                ThreadSafeSetText(lcDownloadedAndSavedInfo, dpInfo.DownloadedAndSaved.ToString());
            }
            else if (e.PropertyName == "WithError")
            {
                ThreadSafeSetText(lcWithErrorInfo, dpInfo.WithError.ToString());
            }
            else if (e.PropertyName == "TotalSeries")
            {
                ThreadSafeSetText(lcTotalSeriesInfo, dpInfo.TotalSeries.ToString());
            }
            else if (e.PropertyName == "RemainingSeries")
            {
                ThreadSafeSetText(lcRemainingSeriesInfo, dpInfo.RemainingSeries.ToString());
            }
            else if (e.PropertyName == "EstimatedTime")
            {
                ThreadSafeSetText(lcEstimatedTimeInfo, dpInfo.EstimatedTime.ToString());
            }
        }

        private static void ThreadSafeSetText(Label label, string value)
        {
            if (label.InvokeRequired)
            {
                label.Invoke((Action<Label, string>)SetTextToLabel, label, value);
            }
            else
                SetTextToLabel(label, value);
        }
        private static void SetTextToLabel(Label label, string value)
        {
            label.Text = value;
        }


        private void BindDownloadInfoTable()
        {
            if (_manager.CurrentDownloadArg == null)
            {
                dgvDownloadData.DataSource = null;
                return;
            }

            dgvDownloadData.AutoGenerateColumns = false;

            var serviceUrlColumn = new DataGridViewTextBoxColumn { DataPropertyName = "Wsdl", HeaderText = "ServiceUrl" };
            var fullSiteCodeColumn = new DataGridViewTextBoxColumn { DataPropertyName = "FullSiteCode", HeaderText = "SiteCode" };
            var fullVariableCodeColumn = new DataGridViewTextBoxColumn { DataPropertyName = "FullVariableCode", HeaderText = "VariableCode" };
            var siteNameColumn = new DataGridViewTextBoxColumn { DataPropertyName = "SiteName", HeaderText = "SiteName" };
            var variableNameColumn = new DataGridViewTextBoxColumn { DataPropertyName = "VariableName", HeaderText = "VariableName" };
            var statusColumn = new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status" };

            dgvDownloadData.Columns.Clear();
            dgvDownloadData.Columns.Add(serviceUrlColumn);
            dgvDownloadData.Columns.Add(fullSiteCodeColumn);
            dgvDownloadData.Columns.Add(fullVariableCodeColumn);
            dgvDownloadData.Columns.Add(siteNameColumn);
            dgvDownloadData.Columns.Add(variableNameColumn);
            dgvDownloadData.Columns.Add(statusColumn);

            dgvDownloadData.DataSource = new BindingList<DownloadInfo>(_manager.CurrentDownloadArg.DownloadList);
        }

        void _manager_OnMessage(object sender, LogMessageEventArgs e)
        {
            var split = e.Message.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            split[0] = DateTime.Now.ToLongTimeString() + " " + split[0];
            foreach (var mes in split)
                lbOutput.Items.Add(mes);

            if (e.Exception != null)
            {
                var message = string.Format("Exception details:" + Environment.NewLine +
                                            "Message: {0}" + Environment.NewLine + 
                                            "Stacktrace: {1}", e.Exception.Message,
                                            e.Exception.StackTrace);
                split = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var mes in split)
                    lbOutput.Items.Add(mes);

                if (e.Exception.InnerException != null)
                    lbOutput.Items.Add("Inner exception: " + e.Exception.InnerException.Message);
            }
        }

        void _manager_Canceled(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
        }

        void _manager_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            _manager.ProgressChanged -= _manager_ProgressChanged;
            _manager.Completed -= _manager_Completed;
            _manager.Canceled -= _manager_Canceled;
            _manager.OnMessage -= _manager_OnMessage;

            _manager.DownloadProgressInfo.PropertyChanged -= dpInfo_PropertyChanged;

            btnCancel.Enabled = false;

            if (_closeAfterCompleted)
                Close();
        }

        void _manager_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbTotal.Value = e.ProgressPercentage;
            lblTotalInfo.Text = e.UserState != null? e.UserState.ToString() : string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DoCancel();
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private bool DoCancel()
        {
            if (MessageBox.Show("Downloading in progress. Do you want to cancel it?", 
                                "Cancel downloading",
                                MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _manager.Cancel();
                return true;
            }

            return false;
        }

        void DownloadManagerUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (_manager.IsBusy)
                {
                    // Ask user to apply for closing
                    if (DoCancel())
                    {
                        _closeAfterCompleted = true;
                    }

                    e.Cancel = true;
                }
            }
        }

        private bool _showHideDetails;
        private void btnDetails_Click(object sender, EventArgs e)
        {
            ShowHideDetails();
        }

        private void ShowHideDetails()
        {
            if (_showHideDetails)
            {
                tlpMain.RowStyles[3].SizeType = SizeType.Percent;
                tlpMain.RowStyles[3].Height = 100F - tlpMain.RowStyles[1].Height;
            }else
            {
                tlpMain.RowStyles[3].SizeType = SizeType.Absolute;
                tlpMain.RowStyles[3].Height = 0;
            }

            _showHideDetails = !_showHideDetails;
        }

        private void btnCopyLog_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            
            foreach(var item in lbOutput.Items)
            {
                sb.Append(lbOutput.GetItemText(item) + Environment.NewLine);
            }

            Clipboard.SetText(sb.ToString());
        }

        #endregion
    }
}
