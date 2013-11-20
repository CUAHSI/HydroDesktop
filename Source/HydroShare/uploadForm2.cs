using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace HydroShare
{
    public partial class uploadForm2 : Form
    {
        string[] prevInputs;

        public uploadForm2(string[] inputs)
        {
            InitializeComponent();
            prevInputs = inputs;
        }

        
        //cancel CLICK
        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete all information in this form. Are you sure you want to proceed?", "Caution", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                this.Close();
            }
            else
            {
                //do nothing
            }
        }

        //finish and upload CLICK
        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.File.WriteAllLines(@"C:\Users\Temp\Desktop\WriteLines.txt", prevInputs);
            /*
            string url = "http://dev.hyrdroshare.org";
            string file = fileTB.Text;
            WebClient client = new WebClient();
            client.UploadFile(url, "post", file);
            */

            this.Close();
            printForm print = new printForm();
            print.StartPosition = FormStartPosition.CenterScreen;
            print.Visible = true;
        }

        //browse CLICK
        private void button2_Click(object sender, EventArgs e)
        {
            /*
            OpenFileDialog fDialog = new OpenFileDialog();
            fDialog.Title = "Open File";
            fDialog.Filter = "XML Files|*.xml|UML Files|*.uml";
            fDialog.InitialDirectory = @"C:\";
            if (fDialog.ShowDialog() == DialogResult.OK)
                MessageBox.Show(fDialog.FileName.ToString());
            */
            
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string folderPath = folder.SelectedPath;
                fileTB.Text = folderPath;
            }
        }

        private void backBT_Click(object sender, EventArgs e)
        {
            this.Close();
            uploadForm1 form1 = new uploadForm1();
            form1.StartPosition = FormStartPosition.CenterScreen;
            form1.Visible = true;
        }

        private void LaunchBrowser_Click(object sender, EventArgs e)
        {
            this.Close();
            gotoWeb browser = new gotoWeb();
            browser.StartPosition = FormStartPosition.CenterScreen;
            browser.Visible = true;
        }
    }
}
