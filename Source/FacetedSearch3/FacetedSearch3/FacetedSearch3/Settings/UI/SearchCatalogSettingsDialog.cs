using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using FacetedSearch3.WebServices;

namespace FacetedSearch3.Settings.UI
{
    public partial class SearchCatalogSettingsDialog : Form
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly WebServicesSettings _webServicesSettings;
        private readonly KeywordsSettings _keywordsSettings;

		#endregion

        #region Constructors

        private SearchCatalogSettingsDialog(CatalogSettings catalogSettings, 
            WebServicesSettings webServicesSettings,
            KeywordsSettings keywordsSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            if (webServicesSettings == null) throw new ArgumentNullException("webServicesSettings");
            if (keywordsSettings == null) throw new ArgumentNullException("keywordsSettings");

            InitializeComponent();

            _catalogSettings = catalogSettings;
            _webServicesSettings = webServicesSettings;
            _keywordsSettings = keywordsSettings;

            rbHisCentral.Tag = TypeOfCatalog.HisCentral;
            rbLocalMetadataCache.Tag = TypeOfCatalog.LocalMetadataCache;

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

            txtCustomUrl.TextChanged += new EventHandler(txtCustomUrl_TextChanged);
            rbHisCentral1.Tag = CatalogSettings.HISCENTRAL_URL_1;
            rbHisCentral2.Tag = CatalogSettings.HISCENTRAL_URL_2;
            rbHisCentral1.CheckedChanged += rbHisCentral_CheckedChanged;
            rbHisCentral2.CheckedChanged += rbHisCentral_CheckedChanged;
            rbHisCentalCustom.CheckedChanged += rbHisCentral_CheckedChanged;
            switch (_catalogSettings.HISCentralUrl)
            {
                case CatalogSettings.HISCENTRAL_URL_1:
                    rbHisCentral1.Checked = true;
                    break;
                case CatalogSettings.HISCENTRAL_URL_2:
                    rbHisCentral2.Checked = true;
                    break;
                default:
                    rbHisCentalCustom.Checked = true;
                    break;
            }
        }

        #endregion

        public static DialogResult ShowDialog(CatalogSettings catalogSettings, 
                                              WebServicesSettings webServicesSettings,
                                              KeywordsSettings keywordsSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            if (webServicesSettings == null) throw new ArgumentNullException("webServicesSettings");
            if (keywordsSettings == null) throw new ArgumentNullException("keywordSettings");

            using (var form = new SearchCatalogSettingsDialog(catalogSettings.Copy(), 
                                                              webServicesSettings.Copy(), 
                                                              keywordsSettings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (catalogSettings.TypeOfCatalog != form._catalogSettings.TypeOfCatalog ||
                        catalogSettings.HISCentralUrl != form._catalogSettings.HISCentralUrl)
                    {
                        form.RefreshWebServices();
                        form.RefresKeywords();
                    }


                    catalogSettings.Copy(form._catalogSettings);
                    webServicesSettings.Copy(form._webServicesSettings);
                    keywordsSettings.Copy(form._keywordsSettings);
                }

                return form.DialogResult;
            }
        }
       

        void txtCustomUrl_TextChanged(object sender, EventArgs e)
        {
            _catalogSettings.HISCentralUrl = txtCustomUrl.Text;
        }

        void rbTypeOfCatalog_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null || 
                !radioButton.Checked) return;

            var typeOfCatalog = (TypeOfCatalog)radioButton.Tag;
            _catalogSettings.TypeOfCatalog = typeOfCatalog;

            // Show groupbox with urls only for HisCentral
            gbHisCentralUrl.Visible = _catalogSettings.TypeOfCatalog == TypeOfCatalog.HisCentral;
        }
        
        private void btnRefreshServices_Click(object sender, EventArgs e)
        {
            RefreshWebServices();
        }

        private void btnRefreshKeywords_Click(object sender, EventArgs e)
        {
            RefresKeywords();
        }

        private void RefreshWebServices()
        {
            try
            {
                _webServicesSettings.RefreshWebServices(_catalogSettings);
            }catch(Exception ex)
            {
                MessageBox.Show("Unable to refresh WebServices." + Environment.NewLine + "Error: " + ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefresKeywords()
        {
            _keywordsSettings.UpdateKeywordsAndOntology(_catalogSettings);
        }

        private void rbHisCentral_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null) return;

            var url = radioButton.Tag as string;
            if (string.IsNullOrEmpty(url))
            {
                url = "Type-in the Custom URL here...";
            }

            txtCustomUrl.Text = url;

            if (url != CatalogSettings.HISCENTRAL_URL_1 &&
                url != CatalogSettings.HISCENTRAL_URL_2)
            {
                txtCustomUrl.Enabled = true;
                txtCustomUrl.Focus();
            }
            else
            {
                txtCustomUrl.Enabled = false;
            }
        }
    }
}
