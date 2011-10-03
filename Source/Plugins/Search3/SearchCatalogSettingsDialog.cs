using System;
using System.Windows.Forms;
using Search3.Settings;

namespace Search3
{
    public partial class SearchCatalogSettingsDialog : Form
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
		//private static readonly string _ontologyFilename = Properties.Settings.Default.ontologyFilename;

		#endregion

        #region Constructors

        private SearchCatalogSettingsDialog(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            
            InitializeComponent();

            _catalogSettings = catalogSettings;

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

        public static void ShowDialog(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");

            using(var form = new SearchCatalogSettingsDialog(catalogSettings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // todo: check for valid  _catalogSettings.HISCentralUrl
                    if (false)
                    {
                        form.DialogResult = DialogResult.None;
                        return;
                    }

                    catalogSettings.Copy(form._catalogSettings);
                }
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
            //todo: implement Refresh Services

            /*
            //check the custom url
            if (!txtCustomUrl.Text.ToLower().StartsWith("http:"))
            {
                MessageBox.Show("Please enter a valid HIS Central URL.");
                txtCustomUrl.Focus();
                DialogResult = DialogResult.None;
                return;
            }


            //refresh the services in search control
            var searcher = new HISCentralSearcher(txtCustomUrl.Text);
            string webServicesXmlPath = Path.Combine(Settings.Instance.ApplicationDataDirectory, 
                Properties.Settings.Default.WebServicesFileName);

            try
            {
                searcher.GetWebServicesXml(webServicesXmlPath);
            }
            catch (Exception ex)
            {
                const string error = "Error refreshing web services from HIS Central. Using existing list of web services.";
                MessageBox.Show(error + " " + ex.Message);
            }

            if (_searchControl != null)
            {
                _searchControl.RefreshWebServices(false, false);
            }*/
        }

        private void btnRefreshKeywords_Click(object sender, EventArgs e)
        {
            // todo: Implement Refresh Keywords
            /*
            var searcher = new HISCentralSearcher(Settings.Instance.SelectedHISCentralURL);
            string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string xmlFileName = Path.Combine ( pluginPath, _ontologyFilename );
            searcher.GetOntologyTreeXml(xmlFileName);*/
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
