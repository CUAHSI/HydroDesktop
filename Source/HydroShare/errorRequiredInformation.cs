using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroShare
{
    public partial class errorRequiredInformation : Form
    {
        public errorRequiredInformation()
        {
            InitializeComponent();
        }

        private void okBT_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void errorMessage_Click(object sender, EventArgs e)
        {

        }
    }
}
