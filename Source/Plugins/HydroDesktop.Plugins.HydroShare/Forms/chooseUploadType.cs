using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.HydroShare
{
    public partial class chooseUploadType : Form
    {
        public chooseUploadType()
        {
            InitializeComponent();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (geoRB.Checked)
            {
                this.Close();
                uploadForm1_geo form1 = new uploadForm1_geo();
                form1.StartPosition = FormStartPosition.CenterScreen;
                form1.Visible = true;
            }
            if (otherRB.Checked)
            {
                this.Close();
                uploadForm1_other form1 = new uploadForm1_other();
                form1.StartPosition = FormStartPosition.CenterScreen;
                form1.Visible = true;
            }
            if (timeRB.Checked)
            {
                this.Close();
                uploadForm1_time form1 = new uploadForm1_time();
                form1.StartPosition = FormStartPosition.CenterScreen;
                form1.Visible = true;
            }

            if ((!geoRB.Checked)&&(!otherRB.Checked)&&(!timeRB.Checked))
            {
                MessageBox.Show("Please select an upload type.", "Error", MessageBoxButtons.OK);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
