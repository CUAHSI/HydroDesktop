using System;
using System.Windows.Forms;
using System.Configuration;

namespace Search3.Settings.UI
{
    public partial class AdvancedHisCentralOptionsDialog : Form
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private const string NULL_PROMPT = "Type-in the Custom URL here...";
        private Configuration myDllConfig;
        private AppSettingsSection myDllConfigAppSettings;
		#endregion

        #region Constructors

        private AdvancedHisCentralOptionsDialog(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            
            InitializeComponent();

            _catalogSettings = catalogSettings;

            //Open the configuration file using the dll location
            //Configuration myDllConfig = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            myDllConfig = ConfigurationManager.OpenExeConfiguration(this.GetType().Assembly.Location);
            // Get the appSettings section
            //AppSettingsSection myDllConfigAppSettings = (AppSettingsSection)myDllConfig.GetSection("appSettings");
            myDllConfigAppSettings = (AppSettingsSection)myDllConfig.GetSection("appSettings");
            // return the desired field 
            catalogSettings.HISCENTRAL_URL_1 = myDllConfigAppSettings.Settings["HISCENTRAL_URL_1"].Value;
            catalogSettings.HISCENTRAL_URL_2 = myDllConfigAppSettings.Settings["HISCENTRAL_URL_2"].Value;
            
            txtCustomUrl.TextChanged += txtCustomUrl_TextChanged;
            rbHisCentalCustom.Tag = catalogSettings.HISCentralUrl != catalogSettings.HISCENTRAL_URL_1 &&
                                    catalogSettings.HISCentralUrl != catalogSettings.HISCENTRAL_URL_2
                                        ? catalogSettings.HISCentralUrl
                                        : null;
            rbHisCentral1.Tag = catalogSettings.HISCENTRAL_URL_1;
            rbHisCentral2.Tag = catalogSettings.HISCENTRAL_URL_2;
            rbHisCentalCustom.Tag = myDllConfigAppSettings.Settings["HISCENTRAL_CUSTOM"].Value;
            rbHisCentral1.CheckedChanged += rbHisCentral_CheckedChanged;
            rbHisCentral2.CheckedChanged += rbHisCentral_CheckedChanged;
            rbHisCentalCustom.CheckedChanged += rbHisCentral_CheckedChanged;
            if (_catalogSettings.HISCentralUrl == catalogSettings.HISCENTRAL_URL_1)
            {
                rbHisCentral1.Checked = true;
            }
            else if (_catalogSettings.HISCentralUrl == catalogSettings.HISCENTRAL_URL_2)
            {
                rbHisCentral2.Checked = true;
            }
            else
            {
                rbHisCentalCustom.Checked = true;
            }
        }

        #endregion

        public static DialogResult ShowDialog(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");

            using (var form = new AdvancedHisCentralOptionsDialog(catalogSettings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    catalogSettings.Copy(form._catalogSettings);
                }

                return form.DialogResult;
            }
        }
       

        void txtCustomUrl_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomUrl.Text == NULL_PROMPT) return;

            _catalogSettings.HISCentralUrl = txtCustomUrl.Text;
            if (rbHisCentalCustom.Checked)
            {
                rbHisCentalCustom.Tag = _catalogSettings.HISCentralUrl;
            }
        }

        private void rbHisCentral_CheckedChanged(object sender, EventArgs e)
        {
            var radioButton = sender as RadioButton;
            if (radioButton == null) return;

            var url = radioButton.Tag as string;
            if (string.IsNullOrEmpty(url))
            {
                url = NULL_PROMPT;
            }

            txtCustomUrl.Text = url;

           // if (url != CatalogSettings.HISCENTRAL_URL_1 &&
           //      url != CatalogSettings.HISCENTRAL_URL_2)
           // {
           //     txtCustomUrl.Enabled = true;
           //     txtCustomUrl.Focus();
           // }
           // else
           // {
           //     txtCustomUrl.Enabled = false;
           // }

            if (rbHisCentalCustom.Checked)
            {
                txtCustomUrl.Enabled = true;
                txtCustomUrl.Focus();
            }
            else
            {
                txtCustomUrl.Enabled = false;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbHisCentalCustom.Checked)
            {
                myDllConfigAppSettings.Settings["HISCENTRAL_CUSTOM"].Value = _catalogSettings.HISCentralUrl;
                myDllConfig.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
}
