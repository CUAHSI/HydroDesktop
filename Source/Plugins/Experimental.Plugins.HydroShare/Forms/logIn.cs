using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HydroDesktop.Plugins.HydroShare.Forms
{
    public partial class logIn : Form
    {
        public logIn()
        {
            InitializeComponent();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            string username = this.userNameTB.Text;
            string password = this.passwordTB.Text;

            //compare user name and pw against database
            this.Visible = false;
            hydroshareLogIn log = new hydroshareLogIn(username, password);
            log.StartPosition = FormStartPosition.CenterScreen;
            log.Visible = false;

            chooseUploadType uploadType = new chooseUploadType();
            uploadType.StartPosition = FormStartPosition.CenterScreen;
            uploadType.Visible = true;
        }
    }
}
