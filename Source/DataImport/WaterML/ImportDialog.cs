using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.ObjectModel;
using HydroDesktop.Configuration;

namespace ImportFromWaterML
{
    public partial class ImportDialog : Form
    {
        private readonly string _fileName;

        public ImportDialog(string fileName)
        {
            _fileName = fileName;
            InitializeComponent();
        }

        private void ExportDialog_Load(object sender, EventArgs e)
        {
            //populate combo box
            DbOperations dbTools = new DbOperations(Settings.Instance.DataRepositoryConnectionString , DatabaseTypes.SQLite);
            DataTable themeTable = dbTools.LoadTable("themes", "select * from DataThemeDescriptions");
            
            cbTheme.DataSource = themeTable;
            cbTheme.DisplayMember = "ThemeName";
            cbTheme.ValueMember = "ThemeId";

            txtFileName.Text = _fileName;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string themeName = cbTheme.Text;

            if (String.IsNullOrEmpty(themeName))
            {
                MessageBox.Show("Please specify the theme name.");
                cbTheme.Focus();
                return;
            }
            
            Downloader objDownloader = new Downloader();
            string fileName = txtFileName.Text;

            string shortFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string siteName = shortFileName;
            string variableName = shortFileName;

            int separatorIndex = shortFileName.IndexOf("_");
            if (separatorIndex > 0 && separatorIndex < shortFileName.Length - 1)
            {
                siteName = shortFileName.Substring(0, shortFileName.IndexOf("_"));
                variableName = shortFileName.Substring(shortFileName.IndexOf("_"));
            }


            IList<Series> seriesList = objDownloader.DataSeriesFromXml(fileName);
            if (seriesList == null)
            {
                MessageBox.Show("error converting xml file");
                return;
            }
            if (objDownloader.ValidateSeriesList(seriesList) == true)
            {

                foreach (Series series in seriesList)
                {
                    objDownloader.SaveDataSeries(series, themeName, siteName, variableName);
                }
            }
            
            MessageBox.Show("Data Import Complete!");
            this.Close();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select the WaterML file to import";
            ofd.Filter = "XML File (*.xml) | *.xml";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtFileName.Text = ofd.FileName;
            }
        }
    }
}
