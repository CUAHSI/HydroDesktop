using System;
using System.Diagnostics;
using System.Windows.Forms;
using HydroDesktop.Common;

namespace HydroDesktop.Plugins.HelpTab
{
    internal partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            lblVersionInfo.Text += AppContext.Instance.ProductVersion;
            DateTime copyDateTime = DateTime.Now; 
            string year = copyDateTime.Year.ToString();
            lblCopyright.Text += year;
            linkLabel1.Links.Remove(linkLabel1.Links[0]);
            linkLabel1.Links.Add(0, linkLabel1.Text.Length, "http://www.hydrodesktop.org");

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }
    }
}
