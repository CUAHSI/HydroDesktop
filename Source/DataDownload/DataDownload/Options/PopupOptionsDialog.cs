using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.DataDownload.Options
{
    public partial class PopupOptionsDialog : Form
    {
       private bool showPopups;
       private bool showLabels;

        public PopupOptionsDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (checkBox1.Checked == true)
            {
                showPopups = true;
            }
            if (checkBox2.Checked == true)
            {
                showLabels = true;
            }

        }
    }
}
