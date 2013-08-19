using System;
using System.IO;
using System.Windows.Forms;

namespace HydroR
{
    public partial class frmInstallR : Form
    {
        private string PathToR;
        public string getPathToR
        {
            get { return PathToR; }
            set 
            { 
                PathToR = value;
                txtPathToR.Text = Path.Combine(PathToR, "R.exe");
            }
        }
        public enum buttonType { OK, Cancel };
        private buttonType RPathResult;
        public buttonType getRPathResult
        {
            get { return RPathResult; }
        }

        public frmInstallR()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Launches the R dialog to change an existing path to R
        /// </summary>
        public frmInstallR(bool changeRPath)
        {
            InitializeComponent();
            Text = "Set Path to R";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtPathToR.Text != "")
            {
                RPathResult = buttonType.OK;
                this.Close();
            }
            else
                MessageBox.Show("You must select a path before you can continue");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            RPathResult = buttonType.Cancel;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofdGetPath = new OpenFileDialog();
            ofdGetPath.DefaultExt = "exe";
            ofdGetPath.FileName = "R.exe";
            ofdGetPath.Title = @"Find The Path to R.exe. Default Location: C:\Program Files\R\R-*version number*\bin";
            ofdGetPath.Filter = "R Executable(*.exe)|*.exe";
            ofdGetPath.ShowDialog();
            txtPathToR.Text = ofdGetPath.FileName;
            PathToR = Path.GetDirectoryName(ofdGetPath.FileName);
        }

        private void txtPathToR_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtPathToR.Text))
                PathToR = Path.GetDirectoryName(txtPathToR.Text);
        }

       

   

      


    }
}
