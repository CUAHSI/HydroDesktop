using System;
using System.Windows.Forms;
using HydroDesktop.Interfaces.PluginContracts;

namespace Search3.Settings.UI
{
    public partial class WebServicesDialog : Form
    {
        #region Fields

        private readonly WebServicesSettings _settings;
        private readonly CatalogSettings _catalogSettings;
        private readonly KeywordsSettings _keywordsSettings;
        private readonly IMetadataFetcherPlugin _metadataFetcher;

        #endregion

        #region Constructors

        private WebServicesDialog(WebServicesSettings settings, CatalogSettings catalogSettings, KeywordsSettings keywordsSettings,
            IMetadataFetcherPlugin metadataFetcher)
        {
            InitializeComponent();

            _settings = settings;
            _catalogSettings = catalogSettings;
            _keywordsSettings = keywordsSettings;
            _metadataFetcher = metadataFetcher;
            webServicesUserControl1.SetSettings(settings, catalogSettings);

            rbHisCentral.CheckedChanged += rbTypeOfCatalog_CheckedChanged;
            rbLocalMetadataCache.CheckedChanged += rbTypeOfCatalog_CheckedChanged;
            switch (_catalogSettings.TypeOfCatalog)
            {
                case TypeOfCatalog.HisCentral:
                    rbHisCentral.Checked = true;
                    break;
                case TypeOfCatalog.LocalMetadataCache:
                    rbLocalMetadataCache.Checked = true;
                    break;
            }
        }

        #endregion

        #region Public methods

        public static DialogResult ShowDialog(WebServicesSettings settings, CatalogSettings catalogSettings, 
             KeywordsSettings keywordsSettings, IMetadataFetcherPlugin metadataFetcher)
        {
            if (settings == null) throw new ArgumentNullException("settings");
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            if (keywordsSettings == null) throw new ArgumentNullException("keywordsSettings");

            using (var form = new WebServicesDialog(settings.Copy(), catalogSettings.Copy(), keywordsSettings.Copy(), metadataFetcher))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (catalogSettings.TypeOfCatalog != form._catalogSettings.TypeOfCatalog ||
                        catalogSettings.HISCentralUrl != form._catalogSettings.HISCentralUrl)
                    {
                        form.RefreshWebServices();
                        form.RefreshKeywords();
                    }

                    settings.Copy(form._settings);
                    catalogSettings.Copy(form._catalogSettings);
                    keywordsSettings.Copy(form._keywordsSettings);
                }

                return form.DialogResult;
            }
        }

        #endregion

        #region Private methods

        private TypeOfCatalog CurrentTypeOfCatalog
        {
            get
            {
                if (rbHisCentral.Checked)
                    return TypeOfCatalog.HisCentral;
                if (rbLocalMetadataCache.Checked)
                    return TypeOfCatalog.LocalMetadataCache;
                throw new Exception("Unknown CurrentTypeOfCatalog");
            }
        }
        
        void rbTypeOfCatalog_CheckedChanged(object sender, EventArgs e)
        {
            var typeOfCatalog = CurrentTypeOfCatalog;
            _catalogSettings.TypeOfCatalog = typeOfCatalog;

            switch (typeOfCatalog)
            {
                case TypeOfCatalog.LocalMetadataCache:
                    bntAddLocalDataSource.Text = "Add Data Source...";
                    break;
                case TypeOfCatalog.HisCentral:
                    bntAddLocalDataSource.Text = "Advanced Options...";
                    break;
            }
        }

        private void RefreshWebServices()
        {
            webServicesUserControl1.RefreshWebServices();
        }

        private void RefreshKeywords()
        {
            _keywordsSettings.UpdateKeywordsAndOntology(_catalogSettings);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshWebServices();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            webServicesUserControl1.CheckAllWebServices(true);
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            webServicesUserControl1.CheckAllWebServices(false);
        }
     
        private void bntAddLocalDataSource_Click(object sender, EventArgs e)
        {
            var typeOfCatalog = CurrentTypeOfCatalog;
            switch (typeOfCatalog)
            {
                case TypeOfCatalog.HisCentral:
                    AdvancedHisCentralOptionsDialog.ShowDialog(_catalogSettings);
                    break;
                case TypeOfCatalog.LocalMetadataCache:
                    if (_metadataFetcher != null)
                    {
                        _metadataFetcher.AddServices();
                    }
                    break;
            }
        }

        #endregion
    }
}
