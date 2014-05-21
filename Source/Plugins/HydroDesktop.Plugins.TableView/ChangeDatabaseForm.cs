using System;
using System.IO;
using System.Windows.Forms;
using DotSpatial.Controls;
using HydroDesktop.Configuration;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Interfaces.PluginContracts;

namespace TableView
{
    public partial class ChangeDatabaseForm : Form
    {
        private string _oldDataRepositoryConnString;

        private string _oldMetadataCacheConnString;

        private readonly Map _mainMap;
        private readonly ISearchPlugin _searchPlugin;

        public ChangeDatabaseForm(Map mainMap, ISearchPlugin searchPlugin)
        {
            InitializeComponent();
            LoadSettings();

            _mainMap = mainMap;
            _searchPlugin = searchPlugin;
        }

        private void LoadSettings()
        {
            //settings for DataRepository
            _oldDataRepositoryConnString = Settings.Instance.DataRepositoryConnectionString;
            if (!String.IsNullOrEmpty(_oldDataRepositoryConnString))
            {
                txtDataRepository.Text = SQLiteHelper.GetSQLiteFileName(_oldDataRepositoryConnString);
            }

            //settings for MetadataCache
            _oldMetadataCacheConnString = Settings.Instance.MetadataCacheConnectionString;
            if (!String.IsNullOrEmpty(_oldMetadataCacheConnString))
            {
                txtMetadataCache.Text = SQLiteHelper.GetSQLiteFileName(_oldMetadataCacheConnString);
            }
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
                SQLiteHelper.CreateMetadataCacheDb(newMetadataCachePath);
            }

            // Check databases schema

            // temporarily changed by JK - don't check the DefaultDatabase schema (to fix HydroModeler DB opening for AGU demo)
            //if (!CheckDatabaseSchema(newDataRepositoryPath, DatabaseType.DefaulDatabase) ||
            //    !CheckDatabaseSchema(newMetadataCachePath, DatabaseType.MetadataCacheDatabase))
            if (!CheckDatabaseSchema(newMetadataCachePath, DatabaseType.MetadataCacheDatabase))
            {
                DialogResult = DialogResult.None;
                return;
            }

            //(2) Set the global settings
            Settings.Instance.DataRepositoryConnectionString = SQLiteHelper.GetSQLiteConnectionString(newDataRepositoryPath);
            Settings.Instance.MetadataCacheConnectionString = SQLiteHelper.GetSQLiteConnectionString(newMetadataCachePath);

            //(3) Update map layers
            if (_mainMap != null)
            {
                var manager = new ThemeManager(_searchPlugin);
                manager.RefreshAllThemes(_mainMap);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool CheckDatabaseSchema(string dataBasePath, DatabaseType databaseType)
        {
            try
            {
                SQLiteHelper.CheckDatabaseSchema(dataBasePath, databaseType);
            }
            catch (InvalidDatabaseSchemaException ex)
            {
                MessageBox.Show(
                    string.Format("The selected database ({0}) has incorrect schema:", Path.GetFileName(dataBasePath)) +
                    Environment.NewLine + Environment.NewLine +
                    ex.Message, "Incorrect schema", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
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
            var fileDialog = new OpenFileDialog
                                 {
                                     Filter = "SQLite databases|*.sqlite",
                                     FileName = txtDataRepository.Text,
                                     Title = "Select the Data Repository Database"
                                 };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SQLiteHelper.GetSQLiteConnectionString(fileDialog.FileName);
                txtDataRepository.Text = fileDialog.FileName;
            }
        }

        private void btnMetadataCacheFileDialog_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
                                 {
                                     Filter = "SQLite databases|*.sqlite",
                                     FileName = txtDataRepository.Text,
                                     Title = "Select the Metadata Cache Database"
                                 };
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                SQLiteHelper.GetSQLiteConnectionString(fileDialog.FileName);
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
