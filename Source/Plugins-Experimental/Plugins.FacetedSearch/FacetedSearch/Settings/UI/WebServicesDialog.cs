using System;
using System.Windows.Forms;

namespace FacetedSearch3.Settings.UI
{
    public partial class WebServicesDialog : Form
    {
        private readonly WebServicesSettings _settings;

        private WebServicesDialog(WebServicesSettings settings)
        {
            InitializeComponent();

            _settings = settings;
            webServicesUserControl1.SetSettings(settings);
        }

        public static DialogResult ShowDialog(WebServicesSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            using (var form = new WebServicesDialog(settings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    settings.Copy(form._settings);
                }

                return form.DialogResult;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            webServicesUserControl1.RefreshWebServices();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            webServicesUserControl1.CheckAllWebServices(true);
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            webServicesUserControl1.CheckAllWebServices(false);
        }
    }
}
