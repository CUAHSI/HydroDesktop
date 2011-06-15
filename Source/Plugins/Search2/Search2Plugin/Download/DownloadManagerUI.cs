using System;
using System.ComponentModel;
using System.Drawing;
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

            lblCurrentInfo.Text = string.Empty;
            lblTotalInfo.Text = string.Empty;

            _manager = manager;
            _manager.ProgressChanged += _manager_ProgressChanged;
            _manager.Completed += _manager_Completed;
            _manager.Canceled += _manager_Canceled;
            _manager.OnMessage += _manager_OnMessage;

            FormClosing += DownloadManagerUI_FormClosing;

            BindDownloadInfoTable();
            ShowHideDetails(); // by default details is not shown
            dgvDownloadData.CellFormatting += dgvDownloadData_CellFormatting;

            //
            btnShowFullLog.Enabled = false;
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
            lbOutput.Items.Add(e.Message);
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

        #endregion

    }
}
