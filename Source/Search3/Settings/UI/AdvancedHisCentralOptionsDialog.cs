using System;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class AdvancedHisCentralOptionsDialog : Form
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private const string NULL_PROMPT = "Type-in the Custom URL here...";

		#endregion

        #region Constructors

        private AdvancedHisCentralOptionsDialog(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            
            InitializeComponent();

            _catalogSettings = catalogSettings;
            
            txtCustomUrl.TextChanged += txtCustomUrl_TextChanged;
            rbHisCentalCustom.Tag = catalogSettings.HISCentralUrl != CatalogSettings.HISCENTRAL_URL_1 &&
                                    catalogSettings.HISCentralUrl != CatalogSettings.HISCENTRAL_URL_2
                                        ? catalogSettings.HISCentralUrl
                                        : null;
            rbHisCentral1.Tag = CatalogSettings.HISCENTRAL_URL_1;
            rbHisCentral2.Tag = CatalogSettings.HISCENTRAL_URL_2;
            rbHisCentral1.CheckedChanged += rbHisCentral_CheckedChanged;
            rbHisCentral2.CheckedChanged += rbHisCentral_CheckedChanged;
            rbHisCentalCustom.CheckedChanged += rbHisCentral_CheckedChanged;
            if (_catalogSettings.HISCentralUrl == CatalogSettings.HISCENTRAL_URL_1)
            {
                rbHisCentral1.Checked = true;
            }
            else if (_catalogSettings.HISCentralUrl == CatalogSettings.HISCENTRAL_URL_2)
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
