using System;
using System.Windows.Forms;
using HydroDesktop.Common;
using HydroDesktop.Common.UserMessage;

namespace HydroDesktop.Plugins.Search.Settings.UI
{
    public partial class AdvancedHisCentralOptionsDialog : Form
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        
		#endregion

        #region Constructors

        private AdvancedHisCentralOptionsDialog(CatalogSettings catalogSettings)
        {
            if (catalogSettings == null) throw new ArgumentNullException("catalogSettings");
            
            InitializeComponent();

            _catalogSettings = catalogSettings;

            cbHisCentralUrl.Items.Add(Properties.Settings.Default.HISCENTRAL_URL_1);
            cbHisCentralUrl.Items.Add(Properties.Settings.Default.HISCENTRAL_URL_2);
            if (!cbHisCentralUrl.Items.Contains(catalogSettings.HISCentralUrl))
            {
                cbHisCentralUrl.Items.Add(catalogSettings.HISCentralUrl);
            }
            cbHisCentralUrl.Text = catalogSettings.HISCentralUrl;
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
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(cbHisCentralUrl.Text))
            {
                MessageBox.Show("Url must be non empty.", "Information", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            _catalogSettings.HISCentralUrl = cbHisCentralUrl.Text;

            try
            {
                Properties.Settings.Default.HISCENTRAL_URL = _catalogSettings.HISCentralUrl;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                AppContext.Instance.Get<IUserMessage>().Error("Unable to save settings.", ex);
                throw;
            }
        }
    }
}
