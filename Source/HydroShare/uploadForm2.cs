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
        public uploadForm2()
        {
            InitializeComponent();
        }

        //cancel CLICK
        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //finish and upload CLICK
        private void button1_Click(object sender, EventArgs e)
        {
            /*
            string url = "http://dev.hyrdroshare.org";
            string file = fileTB.Text;
            WebClient client = new WebClient();
            client.UploadFile(url, "post", file);
            */

            this.Close();
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
    }
}
