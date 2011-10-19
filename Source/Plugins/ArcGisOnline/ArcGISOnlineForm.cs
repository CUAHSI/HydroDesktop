using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.ArcGisOnline
{
    public partial class ArcGISOnlineForm : Form
    {
        public ArcGISOnlineForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //TODO: validate URL
            ServiceRequest.GetFeatures(this.textBox1.Text);
        }
    }
}
