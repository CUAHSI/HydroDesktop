using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using HydroDesktop.Interfaces;
using DotSpatial.Controls;
using HydroDesktop.Database;
using HydroDesktop.Configuration;

namespace HydroDesktop.Main
{
    public partial class ConfigurationForm : Form
    {
        private int formLeft = 0;
        private int formTop = 0;
        private AppManager _app = null;
        
        public ConfigurationForm()
        {
            InitializeComponent();
        }

        public ConfigurationForm(int x, int y)
        {
            InitializeComponent();
            formLeft = x;
            formTop = y;
        }

        public ConfigurationForm(int x, int y, AppManager app)
        {
            InitializeComponent();
            formLeft = x;
            formTop = y;
            _app = app;
        }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            this.Left = formLeft - this.Width / 2;
            this.Top = formTop - this.Height / 2;

            tabControl1.TabPages.RemoveAt(1);
            
            //Version versionInfo = Application.Prod
            txtDataRepository.Text = SQLiteHelper.GetSQLiteFileName(Settings.Instance.DataRepositoryConnectionString);
            txtMetadataCache.Text = SQLiteHelper.GetSQLiteFileName(Settings.Instance.MetadataCacheConnectionString);
            txtProjectFileName.Text = Settings.Instance.CurrentProjectFile;
            txtProjection.Text = _app.Map.Projection.ToEsriString();
        }

        

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(e.Link.LinkData.ToString());
            Process.Start(sInfo);
        }
    }
}
