using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Data.Plugins;
using HydroDesktop.Configuration;

namespace SeriesSelectorTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

            this.Shown += new EventHandler(Form2_Shown);
        }

        void Form2_Shown(object sender, EventArgs e)
        {
            //Settings.Instance.Load();
            //this.hydroDatabase1.ConnectionString = Settings.Instance.DataRepositoryConnectionString;
            //this.seriesSelector41.HydroDatabase = this.hydroDatabase1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form3 frm = new Form3();
            frm.seriesSelector41.Database = this.hydroDatabase1;
            frm.Show();

        }
    }
}
