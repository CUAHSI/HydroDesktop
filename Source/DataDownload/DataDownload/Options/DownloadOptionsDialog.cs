using System.Windows.Forms;
using System.Text.RegularExpressions;
using HydroDesktop.WebServices.WaterOneFlow;

namespace HydroDesktop.DataDownload.Options
{
    public partial class DownloadOptionsDialog : Form
    {
        string regex = "^(?![0]+$)[0-9]+$";

        public DownloadOptionsDialog()
        {
            InitializeComponent();
            WaterOneFlowClient client = new WaterOneFlowClient();
            if (client.AllInOneRequest == true)
            {
                checkBox1.Checked = true;
            }
            else
            {
                textBox1.Text = client.ValuesPerReq.ToString();
            }

            if (Downloading.DownloadManager.singleThread == true)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                label2.Text = "";
                this.DialogResult = DialogResult.OK;
                WaterOneFlowClient client = new WaterOneFlowClient();
                client.AllInOneRequest = true;
            }
            else if (checkValue() == true)
            {
                label2.Text = ""; 
                this.DialogResult = DialogResult.OK;
                WaterOneFlowClient client = new WaterOneFlowClient();
                client.AllInOneRequest = false;
                client.ValuesPerReq = int.Parse(this.textBox1.Text);
            }
            else
            {
                label2.Text = "Invalid Input";  
            }

            if (checkBox2.Checked == true)
            {
                Downloading.DownloadManager.singleThread = true;
            }
            else
            {
                Downloading.DownloadManager.singleThread = false;
            }
        }

        private bool checkValue()
        {
            Match match;
            match = Regex.Match(this.textBox1.Text, regex);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.Text = "";
                label2.Text = "";
                textBox1.Enabled = false;
            }
            else
            {
                label2.Text = "";
                textBox1.Enabled = true;
            }
        }     
    }
}
