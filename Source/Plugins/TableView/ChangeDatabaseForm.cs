using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Configuration;
using HydroDesktop.Interfaces;
using System.IO;
using HydroDesktop.Controls.Themes;

namespace TableView
{
    public partial class ChangeDatabaseForm : Form
    {
        private string _oldDataRepositoryConnString;
        private string _newDataRepositoryConnString;

        private string _oldMetadataCacheConnString;
        private string _newMetadataCacheConnString;

        private IHydroAppManager _appManager;

        public ChangeDatabaseForm(IHydroAppManager appManager)
        {
            InitializeComponent();
            LoadSettings();

            _appManager = appManager;
        }

        private void LoadSettings()
        {
            //settings for DataRepository
            _oldDataRepositoryConnString = Settings.Instance.DataRepositoryConnectionString;
            txtDataRepository.Text = SQLiteHelper.GetSQLiteFileName(_oldDataRepositoryConnString);

            //settings for MetadataCache
            _oldMetadataCacheConnString = Settings.Instance.MetadataCacheConnectionString;
            txtMetadataCache.Text = SQLiteHelper.GetSQLiteFileName(_oldMetadataCacheConnString);
        }

        private void SaveSettings()
        {
            //Settings.Instance.Save();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string newDataRepositoryPath = txtDataRepository.Text;
            string newMetadataCachePath = txtMetadataCache.Text;
            
            //(1) check if new DB exist and create if needed
            if (!SQLiteHelper.DatabaseExists(newDataRepositoryPath))
            {
                SQLiteHelper.CreateSQLiteDatabase(newDataRepositoryPath);
            }

            if (!SQLiteHelper.DatabaseExists(newMetadataCachePath))
            {
                SQLiteHelper.CreateSQLiteDatabase(newMetadataCachePath);
            }

            //(2) Set the global settings
            Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(newDataRepositoryPath);
            Settings.Instance.MetadataCacheConnectionString = SQLiteHelper.GetSQLiteConnectionString(newMetadataCachePath);

            //(3) Refresh 'SeriesSelector' control
            _appManager.SeriesView.SeriesSelector.SetupDatabase();
            _appManager.SeriesView.SeriesSelector.RefreshSelection();

            ThemeManager manager = new ThemeManager(Settings.Instance.DataRepositoryConnectionString);
            AppManager dotSpatialManager = (AppManager)_appManager;
            Map mainMap = dotSpatialManager.Map as Map;
            if (mainMap != null)
            {
                manager.RefreshAllThemes(mainMap);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnRestoreDefault_Click(object sender, EventArgs e)
        {
            string defaultDataRepositoryPath = Path.ChangeExtension(Settings.Instance.CurrentProjectFile, ".sqlite");

            txtDataRepository.Text = defaultDataRepositoryPath;
            
            //see if they are different from original
            if (_oldDataRepositoryConnString != SQLiteHelper.GetSQLiteConnectionString(txtDataRepository.Text))
            {
                //restore default..
                Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(txtDataRepository.Text);
            }
        }

        private void btnDataRepositoryFileDialog_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "SQLite databases|*.sqlite";
            fileDialog.FileName = txtDataRepository.Text;
            fileDialog.Title = "Select the Data Repository Database";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _newDataRepositoryConnString = SQLiteHelper.GetSQLiteConnectionString(fileDialog.FileName);
                txtDataRepository.Text = fileDialog.FileName;
            }
        }

        private void btnMetadataCacheFileDialog_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "SQLite databases|*.sqlite";
            fileDialog.FileName = txtDataRepository.Text;
            fileDialog.Title = "Select the Metadata Cache Database";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                _newMetadataCacheConnString = SQLiteHelper.GetSQLiteConnectionString(fileDialog.FileName);
                txtMetadataCache.Text = fileDialog.FileName;
            }
        }

        private void btnMetadataCacheRestoreDefault_Click(object sender, EventArgs e)
        {
            txtMetadataCache.Text = Settings.Instance.CurrentProjectFile.Replace(".dspx", "_cache.sqlite");
            if (_oldMetadataCacheConnString != SQLiteHelper.GetSQLiteConnectionString(txtMetadataCache.Text))
            {
                Settings.Instance.MetadataCacheConnectionString = SQLiteHelper.GetSQLiteConnectionString(txtMetadataCache.Text);
            }
        }
    }
}
