using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace HelpTab
{
    public partial class AboutBox : Form
    {
        private int formLeft = 0;
        private int formTop = 0;
        
        public AboutBox()
        {
            InitializeComponent();
        }

        public AboutBox(int x, int y)
        {
            InitializeComponent();
            formLeft = x;
            formTop = y;
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            if (formLeft > 0 && formTop > 0)
            {
                this.Left = formLeft - this.Width / 2;
                this.Top = formTop - this.Height / 2;
            }

            //Version versionInfo = Application.Prod
            //string asmVer = System.Reflection.Assembly.GetAssembly(typeof(AboutBox)).
            //lblVersionInfo.Text = "Version:  " + Application.ProductVersion;

            string appName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Application Extensions\HydroDesktop.MainPlugin.dll");
            if (System.IO.File.Exists(appName))
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(appName);
                lblVersionInfo.Text = assemblyName.Version.ToString();
            }
            else
            {
                lblVersionInfo.Text = "1.3";
            }

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
