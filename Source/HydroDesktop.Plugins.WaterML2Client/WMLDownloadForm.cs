using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.Plugins.WaterML2Client
{
    public partial class WMLDownloadForm : Form
    {
        private string _file;

        public WMLDownloadForm()
        {
            InitializeComponent();

            var themeTable = RepositoryFactory.Instance.Get<IDataThemesRepository>().GetAll();

            cbTheme.DataSource = themeTable;
            cbTheme.DisplayMember = "Name";
            cbTheme.ValueMember = "Id";
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var url = tbTimeSeriesUrl.Text;
            if (String.IsNullOrEmpty(url))
            {
                MessageBox.Show(this, "Not valid url.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            using (var wb = new WebClient())
            {
                try
                {
                    var file = Path.GetTempFileName();
                    wb.DownloadFile(tbTimeSeriesUrl.Text, file);
                    _file = file;
                    MessageBox.Show(this, "File downloaded.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Download failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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

            
            var parser = new WaterOneFlow20Parser();
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

            var db = RepositoryFactory.Instance.Get<IRepositoryManager>();
            var theme = new Theme(themeName);
            foreach (var series in seriesList)
            {
                db.SaveSeries(series, theme, OverwriteOptions.Copy);
            }
        }
    }
}
