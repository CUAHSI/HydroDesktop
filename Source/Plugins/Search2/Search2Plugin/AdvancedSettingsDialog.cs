using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.WebServices;
using HydroDesktop.Database;
using HydroDesktop.Interfaces;
using HydroDesktop.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace HydroDesktop.Search
{
    public partial class AdvancedSettingsDialog : Form
    {
		#region Private Member Variables

		//private readonly string SERVICES_XML_NAME = Properties.Settings.Default.SERVICES_XML_NAME;
		private static readonly string _ontologyFilename = Properties.Settings.Default.ontologyFilename;

        private SearchControl _searchControl = null;

        private const string HISCENTRAL_URL_1 = "http://hiscentral.cuahsi.org/webservices/hiscentral.asmx";
        private const string HISCENTRAL_URL_2 = "http://water.sdsc.edu/hiscentral/webservices/hiscentral.asmx";

		#endregion

        public AdvancedSettingsDialog(SearchControl ucSearch)
        {
            InitializeComponent();

            _searchControl = ucSearch;

            
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string hisCentralURL = HISCENTRAL_URL_1;
            string oldHisCentralURL = Settings.Instance.SelectedHISCentralURL;
            
            if (rbHisCentral1.Checked)
            {
                hisCentralURL = HISCENTRAL_URL_1;
            }
            else if (rbHisCentral2.Checked)
            {
                hisCentralURL = HISCENTRAL_URL_2;
            }
            else
            {
                //check if it is a valid URL
                hisCentralURL = txtCustomUrl.Text;
                try
                {
                    HISCentralClient client = new HISCentralClient(hisCentralURL);
                    hisCentralURL = txtCustomUrl.Text;
                }
                catch
                {
                    MessageBox.Show("The URL is not valid. Please enter a valid HIS Central URL.");
                    this.DialogResult = DialogResult.None;
                }
            }
            txtCustomUrl.Text = hisCentralURL;
            Settings.Instance.SelectedHISCentralURL = hisCentralURL;

            //to save the settings
            if (rbAppend.Checked)
            {
                Settings.Instance.DownloadOption = OverwriteOptions.Append.ToString();
            }
            else if (rbCopy.Checked)
            {
                Settings.Instance.DownloadOption = OverwriteOptions.Copy.ToString();
            }
            else if (rbOverwrite.Checked)
            {
                Settings.Instance.DownloadOption = OverwriteOptions.Overwrite.ToString();
            }

            //refresh web services
            if (Settings.Instance.SelectedHISCentralURL != oldHisCentralURL)
            {
                _searchControl.RefreshWebServices(true, true);
            }
        }

        private void btnRefreshServices_Click(object sender, EventArgs e)
        {
            //check the custom url
            if (!txtCustomUrl.Text.ToLower().StartsWith("http:"))
            {
                MessageBox.Show("Please enter a valid HIS Central URL.");
                txtCustomUrl.Focus();
                DialogResult = DialogResult.None;
                return;
            }


            //refresh the services in search control
            HISCentralSearcher searcher = new HISCentralSearcher(txtCustomUrl.Text);
            string webServicesXmlPath = Path.Combine(Settings.Instance.ApplicationDataDirectory, 
                Properties.Settings.Default.WebServicesFileName);

            try
            {
                searcher.GetWebServicesXml(webServicesXmlPath);
            }
            catch (Exception ex)
            {
                string error = "Error refreshing web services from HIS Central. Using existing list of web services.";
                MessageBox.Show(error + " " + ex.Message);
            }

            if (_searchControl != null)
            {
                _searchControl.RefreshWebServices(false, false);
            }
        }

        private void btnRefreshKeywords_Click(object sender, EventArgs e)
        {
            HISCentralSearcher searcher = new HISCentralSearcher(Settings.Instance.SelectedHISCentralURL);
            string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string xmlFileName = Path.Combine ( pluginPath, _ontologyFilename );
            searcher.GetOntologyTreeXml(xmlFileName);
        }

        private void rbHisCentral1_CheckedChanged(object sender, EventArgs e)
        {
            if (rbHisCentral1.Checked)
            {
                txtCustomUrl.Text = HISCENTRAL_URL_1;
                txtCustomUrl.Enabled = false;
            }
        }

        private void rbHisCentral2_CheckedChanged(object sender, EventArgs e)
        {
            if (rbHisCentral2.Checked)
            {
                txtCustomUrl.Text = HISCENTRAL_URL_2;
                txtCustomUrl.Enabled = false;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                if (Settings.Instance.SelectedHISCentralURL == HISCENTRAL_URL_1 || Settings.Instance.SelectedHISCentralURL == HISCENTRAL_URL_2)
                {
                    txtCustomUrl.Text = "Type-in the Custom URL here...";
                }
                else
                {
                    txtCustomUrl.Text = Settings.Instance.SelectedHISCentralURL;
                }
                txtCustomUrl.Enabled = true;
                txtCustomUrl.Focus();
            }
        }

        //when this dialog is loaded
        private void AdvancedSettingsDialog_Load(object sender, EventArgs e)
        {
            //to get the URL
            string url = Settings.Instance.SelectedHISCentralURL;
            if (!url.StartsWith("http://"))
                url = "http://" + url;

            //to restore the settings:

            if (url == HISCENTRAL_URL_1)
            {
                rbHisCentral1.Checked = true;
                lblHisCentralURL.Text = url;
                txtCustomUrl.Enabled = false;
            }
            else if (url == HISCENTRAL_URL_2)
            {
                rbHisCentral2.Checked = true;
                lblHisCentralURL.Text = url;
                txtCustomUrl.Enabled = false;
            }
            else
            {
                radioButton7.Checked = true;
                txtCustomUrl.Enabled = true;
                txtCustomUrl.Text = url;
            }

            //to restore download options settings
            switch (Settings.Instance.DownloadOption)
            {
                case "Append":
                case "Fill":
                    rbAppend.Checked = true;
                    break;
                case "Copy":
                    rbCopy.Checked = true;
                    break;
                case "Overwrite":
                    rbOverwrite.Checked = true;
                    break;
            }
        }
    }
}
