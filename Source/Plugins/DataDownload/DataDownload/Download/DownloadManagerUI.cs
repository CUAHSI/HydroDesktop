using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;

namespace HydroDesktop.DataDownload.Download
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
            InitDownloadProgressInfo();
            InitDownloadInfoTable();

            _manager = manager;
            
            ShowHideDetails(); // by default details is not shown
            dgvDownloadData.CellFormatting += dgvDownloadData_CellFormatting;
            dgvDownloadData.MouseClick += dgvDownloadData_MouseClick;
            FormClosing += DownloadManagerUI_FormClosing;

            BindToDownloadManager();
            //
            btnSendError.Enabled = false; // TODO: implement send error logic
            redownloadControl1.Enabled = false;
        }

        #endregion

        #region Properties

        private bool IsAutoScrollDetailsLog
        {
            get { return chbAutoScroll.Checked; }
        }

        #endregion

        #region Private methods

        void dgvDownloadData_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (_manager.IsBusy) return;

            var menu = new ContextMenuStrip();

            var seriesWithErrors = GetAllSelectedRows(DownloadInfoStatus.Error);
            if (seriesWithErrors != null &&
                seriesWithErrors.Count > 0)
            {
                var item = menu.Items.Add("Redownload selected series (status = Error)");
                item.Tag = seriesWithErrors;
                item.Click += item_RedownloadRows;
            }
            var allSelected = GetAllSelectedRows();
            if (allSelected != null &&
                allSelected.Count > 0)
            {
                var item = menu.Items.Add("Redownload selected series (any status)");
                item.Tag = allSelected;
                item.Click += item_RedownloadRows;
            }

            // show menu only if it has at least one item
            if (menu.Items.Count > 0)
            {
                menu.Show(dgvDownloadData, e.Location);
            }
        }

        private ICollection<int> GetAllSelectedRows(DownloadInfoStatus status)
        {
            return GetAllSelectedRows().Where(ind => _manager.Information.StartDownloadArg.ItemsToDownload[ind].Status == status).ToList();
        }

        private ICollection<int> GetAllRows(DownloadInfoStatus status)
        {
            var result = new List<int>();
            for(int i = 0; i< _manager.Information.StartDownloadArg.ItemsToDownload.Count; i++)
            {
                if (_manager.Information.StartDownloadArg.ItemsToDownload[i].Status == status)
                    result.Add(i);
            }
            return result;
        }

        private ICollection<int> GetAllSelectedRows()
        {
            ICollection<int> indeces;
            
            if (dgvDownloadData.SelectedRows.Count > 0)
            {
                // get rows from SelectedRows
                var rows = new List<int>(dgvDownloadData.SelectedRows.Count);
                foreach (DataGridViewRow row in dgvDownloadData.SelectedRows)
                {
                    rows.Add(row.Index);
                }
                indeces = rows;
            }
            else
            {
                // get rows from selected cells
                var rows = new HashSet<int>();
                foreach (DataGridViewCell cell in dgvDownloadData.SelectedCells)
                {
                    var dInfo = cell.RowIndex;
                    if (!rows.Contains(dInfo))
                    {
                        rows.Add(dInfo);
                    }
                }
                indeces = rows;
            }

            return indeces;
        }

        void item_RedownloadRows(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripItem;
            if (menuItem == null) return;

            var indeces = menuItem.Tag as ICollection<int>;
            if (indeces == null) return;
            Debug.Assert(indeces != null);

            DoRedownload(indeces);
        }

        private void DoRedownload(ICollection<int> indeces = null)
        {
            btnCancel.Enabled = true;
            redownloadControl1.Enabled = false;

            SubcribeToManagerEvents();
            _manager.SubStart(indeces);
        }

        private void BindToDownloadManager()
        {
            SubcribeToManagerEvents();
            BindDownloadInfoTable();
            BindDownloadProgressInfo();
        }

        private void SubcribeToManagerEvents()
        {
            _manager.ProgressChanged += _manager_ProgressChanged;
            _manager.Completed += _manager_Completed;
            _manager.Canceled += _manager_Canceled;
            _manager.OnMessage += _manager_OnMessage;
            _manager.Information.PropertyChanged += dpInfo_PropertyChanged;
        }
        private void UnSubcribeFromManagerEvents()
        {
            _manager.ProgressChanged -= _manager_ProgressChanged;
            _manager.Completed -= _manager_Completed;
            _manager.Canceled -= _manager_Canceled;
            _manager.OnMessage -= _manager_OnMessage;
            _manager.Information.PropertyChanged -= dpInfo_PropertyChanged;
        }

        void dgvDownloadData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0 ||
                dgvDownloadData.Columns[e.ColumnIndex].DataPropertyName != "Status") return;

            var blist = (BindingList<OneSeriesDownloadInfo>)dgvDownloadData.DataSource;
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
        
        private void InitDownloadProgressInfo()
        {
            lcDownloadedAndSavedInfo.Text = null;
            lcWithErrorInfo.Text = null;
            lcTotalSeriesInfo.Text = null;
            lcRemainingSeriesInfo.Text = null;
            lcEstimatedTimeInfo.Text = null;
        }
        private void BindDownloadProgressInfo()
        {
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("DownloadedAndSaved"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("WithError"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("TotalSeries"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("RemainingSeries"));
            dpInfo_PropertyChanged(this, new PropertyChangedEventArgs("EstimatedTime"));
        }

        void dpInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var dpInfo = _manager.Information; // to avoid long names
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
                label.BeginInvoke((Action<Label, string>)SetTextToLabel, label, value);
            }
            else
                SetTextToLabel(label, value);
        }
        private static void SetTextToLabel(Label label, string value)
        {
            label.Text = value;
        }

        private void InitDownloadInfoTable()
        {
            dgvDownloadData.DataSource = null;
            dgvDownloadData.AutoGenerateColumns = false;

            var serviceUrlColumn = new DataGridViewTextBoxColumn { DataPropertyName = OneSeriesDownloadInfo.PROPERTY_Wsdl, HeaderText = "ServiceUrl" };
            var fullSiteCodeColumn = new DataGridViewTextBoxColumn { DataPropertyName = OneSeriesDownloadInfo.PROPERTY_FullSiteCode, HeaderText = "SiteCode" };
            var fullVariableCodeColumn = new DataGridViewTextBoxColumn { DataPropertyName = OneSeriesDownloadInfo.PROPERTY_FullVariableCode, HeaderText = "VariableCode" };
            var siteNameColumn = new DataGridViewTextBoxColumn { DataPropertyName = OneSeriesDownloadInfo.PROPERTY_SiteName, HeaderText = "SiteName" };
            var variableNameColumn = new DataGridViewTextBoxColumn { DataPropertyName = OneSeriesDownloadInfo.PROPERTY_VariableName, HeaderText = "VariableName" };
            var statusColumn = new DataGridViewTextBoxColumn { DataPropertyName = OneSeriesDownloadInfo.PROPERTY_Status, HeaderText = "Status" };

            dgvDownloadData.Columns.Clear();
            dgvDownloadData.Columns.Add(serviceUrlColumn);
            dgvDownloadData.Columns.Add(fullSiteCodeColumn);
            dgvDownloadData.Columns.Add(fullVariableCodeColumn);
            dgvDownloadData.Columns.Add(siteNameColumn);
            dgvDownloadData.Columns.Add(variableNameColumn);
            dgvDownloadData.Columns.Add(statusColumn);
        }
        private void BindDownloadInfoTable()
        {
            dgvDownloadData.DataSource = new BindingList<OneSeriesDownloadInfo>(_manager.Information.StartDownloadArg.ItemsToDownload);
        }

        void _manager_OnMessage(object sender, LogMessageEventArgs e)
        {
            var split = e.Message.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            split[0] = DateTime.Now.ToLongTimeString() + " " + split[0];
            foreach (var mes in split)
                ThreadSafeAddItemToLog(lbOutput, mes);

            if (e.Exception != null)
            {
                var message = string.Format("Exception details:" + Environment.NewLine +
                                            "Message: {0}" + Environment.NewLine + 
                                            "Stacktrace: {1}", e.Exception.Message,
                                            e.Exception.StackTrace);
                split = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var mes in split)
                    ThreadSafeAddItemToLog(lbOutput, mes);

                if (e.Exception.InnerException != null)
                    ThreadSafeAddItemToLog(lbOutput, "Inner exception: " + e.Exception.InnerException.Message);
            }
        }
        
        private void ThreadSafeAddItemToLog(ListBox listBox, object value)
        {
            if (listBox.InvokeRequired)
            {
                listBox.BeginInvoke((Action<ListBox, object>)AddItemToLog, listBox, value);
            }
            else
                AddItemToLog(listBox, value);
        }
        private void AddItemToLog(ListBox listBox, object value)
        {
            listBox.Items.Add(value);

            // scroll to last item if need
            if (IsAutoScrollDetailsLog)
            {
                listBox.SelectedIndex = listBox.Items.Count - 1;
            }
        }

        void _manager_Canceled(object sender, EventArgs e)
        {
            btnCancel.Enabled = false;
        }

        void _manager_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            UnSubcribeFromManagerEvents();

            btnCancel.Enabled = false;
            redownloadControl1.Enabled = true;
            
            if (e.Error != null)
            {
                MessageBox.Show("Error occurred during data download." + Environment.NewLine + e.Error, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else
            {
                if (e.Cancelled)
                {
                    MessageBox.Show(string.Format("Data download has been cancelled." + Environment.NewLine +
                                                  "{0} out of {1} series were saved to database.",
                                                  _manager.Information.DownloadedAndSaved,
                                                  _manager.Information.TotalSeries),
                                    "Cancelled", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(string.Format("Data download complete." + Environment.NewLine +
                                                  "Downloaded and saved: {0}" + Environment.NewLine +
                                                  "Failed series: {1}",
                                                  _manager.Information.DownloadedAndSaved,
                                                  _manager.Information.WithError),
                                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //-----
                    var themeName = _manager.Information.StartDownloadArg.DataTheme == null
                                        ? string.Empty
                                        : _manager.Information.StartDownloadArg.DataTheme.Name;

                    //Display theme in the main map
                    AddThemeToMap(themeName);
                }
            }

            if (_closeAfterCompleted)
                Close();
        }

        private void AddThemeToMap(string themeName)
        {
            try
            {
                var _mapArgs = Global.PluginEntryPoint.MapArgs;

                //to refresh the series selector control
                //TODO: need other way to send this message
                var mainApplication = _mapArgs.AppManager as IHydroAppManager;
                if (mainApplication != null)
                {
                    mainApplication.SeriesView.SeriesSelector.RefreshSelection();
                }

                var db = new DbOperations(Settings.Instance.DataRepositoryConnectionString, DatabaseTypes.SQLite);
                var manager = new Controls.Themes.ThemeManager(db);
                var fs = manager.GetFeatureSet(themeName, _mapArgs.Map.Projection);
                manager.AddThemeToMap(fs, themeName, _mapArgs.Map as DotSpatial.Controls.Map);
            }
            catch { }
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

                chbAutoScroll.Enabled = true;
            }else
            {
                tlpMain.RowStyles[3].SizeType = SizeType.Absolute;
                tlpMain.RowStyles[3].Height = 0;

                chbAutoScroll.Enabled = false;
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

        private void redownloadControl1_DoRedownload(object sender, RedownloadArgs e)
        {
            switch (e.RedownloadOption)
            {
                case RedownloadOption.All:
                    DoRedownload();
                    break;
                case RedownloadOption.AllSelected:
                    DoRedownload(GetAllSelectedRows());
                    break;
                case RedownloadOption.SelectedWithErrors:
                    DoRedownload(GetAllSelectedRows(DownloadInfoStatus.Error));
                    break;
                case RedownloadOption.AllWithErrors:
                    DoRedownload(GetAllRows(DownloadInfoStatus.Error));
                    break;
            }
        }

        #endregion
    }
}
