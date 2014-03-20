using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterML;

namespace HydroDesktop.Plugins.WaterML2Client
{
    public partial class WMLDownloadForm : Form
    {
        private string _file;
        private Thread _workerThread;

        public WMLDownloadForm()
        {
            InitializeComponent();

            var themeTable = RepositoryFactory.Instance.Get<IDataThemesRepository>().GetAll();

            cbTheme.DataSource = themeTable;
            cbTheme.DisplayMember = "Name";
            cbTheme.ValueMember = "Id";
        }

        private void SetControlsForDownloading(bool downloading)
        {
            tbTimeSeriesUrl.Enabled = !downloading;
            cbTheme.Enabled = !downloading;
            btnAdd.Enabled = !downloading;
            btnClose.Enabled = !downloading;

            if (!downloading)
            {
                btnDownload.Text = "Download";
                lblDownloading.Visible = false;
            }
            else
            {
                btnDownload.Text = "Cancel";
                lblDownloading.Visible = true;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var wt = _workerThread;
            if (wt != null)
            {
                wt.Abort();
                return;
            }

            var url = tbTimeSeriesUrl.Text;
            if (String.IsNullOrEmpty(url))
            {
                MessageBox.Show(this, "Not valid url.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetControlsForDownloading(true);
            var bw = new BackgroundWorker();
            bw.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                _workerThread = new Thread((ThreadStart) delegate
                {
                    try
                    {
                        using (var wb = new WebClient())
                        {
                            var file = Path.GetTempFileName();
                            wb.DownloadFile(tbTimeSeriesUrl.Text, file);
                            args.Result = file;
                        }
                    }
                    catch (ThreadAbortException)
                    {
                        args.Cancel = true;
                    }
                    catch (Exception ex)
                    {
                        args.Result = ex;
                    }
                });
                _workerThread.Start();
                _workerThread.Join();
            };
            bw.RunWorkerCompleted += delegate(object o, RunWorkerCompletedEventArgs args)
            {
                _workerThread = null;
                SetControlsForDownloading(false);

                if (args.Cancelled)
                {
                    MessageBox.Show(this, "Cancelled.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var ex = args.Result as Exception ?? args.Error;
                    if (ex != null)
                    {
                        MessageBox.Show(this, ex.Message, "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        _file = (string) args.Result;
                        MessageBox.Show(this, "File downloaded.", "Information", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            };
            bw.RunWorkerAsync();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var themeName = cbTheme.Text;
            if (String.IsNullOrEmpty(themeName))
            {
                MessageBox.Show(this, "Not valid theme.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(_file) || !File.Exists(_file))
            {
                MessageBox.Show(this, "Not valid file to add.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            
            var parser = new WaterML20Parser();
            IList<Series> seriesList;
            try
            {
                seriesList = parser.ParseGetValues(_file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Parse error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var db = RepositoryFactory.Instance.Get<IRepositoryManager>();
                var theme = new Theme(themeName);
                foreach (var series in seriesList)
                {
                    db.SaveSeries(series, theme, OverwriteOptions.Copy);
                }
                MessageBox.Show(this, "Saved.", "Information", MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
