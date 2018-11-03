using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterML;

namespace HydroDesktop.Plugins.WaterML2Client
{
    public partial class WMLDownloadForm : Form
    {
        private readonly ISeriesSelector _seriesControl;
        private WebClient _webClient;

        public WMLDownloadForm(ISeriesSelector seriesControl)
        {
            if (seriesControl == null) throw new ArgumentNullException("seriesControl");
            _seriesControl = seriesControl;

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
            btnClose.Enabled = !downloading;
            btnOpenFile.Enabled = !downloading;

            if (!downloading)
            {
                btnImport.Text = "Import";
                lblDownloading.Visible = false;
            }
            else
            {
                btnImport.Text = "Cancel";
                lblDownloading.Visible = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var wt = _webClient;
            if (wt != null)
            {
                wt.CancelAsync();
                return;
            }

            var url = tbTimeSeriesUrl.Text;
            if (String.IsNullOrEmpty(url))
            {
                MessageBox.Show(this, "Not valid url or file path.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var themeName = cbTheme.Text;
            if (String.IsNullOrEmpty(themeName))
            {
                MessageBox.Show(this, "Not valid theme.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Check what selected:  url of file
            if (!File.Exists(url))
            {
                Uri uri;
                try
                {
                    uri = new Uri(url);
                }
                catch (UriFormatException)
                {
                    MessageBox.Show(this, "Not valid url.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                SetControlsForDownloading(true);

                _webClient = new WebClient();
                var file = Path.GetTempFileName();
                _webClient.DownloadFileAsync(uri, file);
                _webClient.DownloadFileCompleted += delegate(object o, AsyncCompletedEventArgs args)
                {
                    _webClient.Dispose();
                    _webClient = null;
                    SetControlsForDownloading(false);
                    if (args.Cancelled)
                    {
                        MessageBox.Show(this, "Download Cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (args.Error != null)
                    {
                        MessageBox.Show(this, args.Error.Message, "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    ImportFromFile(file, themeName);
                };
            }
            else
            {
                ImportFromFile(url, themeName);
            }
        }

        private void ImportFromFile(string fileName, string themeName)
        {
            var parser = new WaterML20Parser();
            IList<Series> seriesList;
            try
            {
                seriesList = parser.ParseGetValues(fileName);
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
                _seriesControl.RefreshSelection();
                MessageBox.Show(this, "Data imported successfully.", "Information", MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    tbTimeSeriesUrl.Text = ofd.FileName;
                }
            }
        }
    }
}
