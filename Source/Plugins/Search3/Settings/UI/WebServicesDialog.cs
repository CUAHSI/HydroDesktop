using System;
using System.Windows.Forms;

namespace Search3.Settings.UI
{
    public partial class WebServicesDialog : Form
    {
        private WebServicesDialog(WebServicesSettings settings)
        {
            InitializeComponent();

            webServicesUserControl1.SetWebServices(settings.WebServices);
        }

        public static void ShowDialog(WebServicesSettings settings)
        {
            if (settings == null) throw new ArgumentNullException("settings");

            using (var form = new WebServicesDialog(settings.Copy()))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    settings.WebServices = form.webServicesUserControl1.GetWebServices();
                }
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
