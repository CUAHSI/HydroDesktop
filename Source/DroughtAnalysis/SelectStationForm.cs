using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HydroDesktop.Interfaces.ObjectModel;
using System.IO;

namespace DroughtAnalysis
{
    public partial class SelectStationForm : Form
    {
        public SelectStationForm()
        {
            InitializeComponent();
        }

        public SelectStationForm(DroughtSettings settings)
        {
            InitializeComponent();
            Settings = settings;
        }

        public DroughtSettings Settings { get; set; }

        private void SelectStationForm_Load(object sender, EventArgs e)
        {
            bindingSource1.DataSource = Settings.SuitableSites;

            CheckRPath();
        }

        private void CheckRPath()
        {
            string defaultRPath = @"C:\Program Files\R\R-2.14.0\bin\i386\R.exe";
            if (File.Exists(defaultRPath))
            {
                Settings.PathToR = defaultRPath;
                textBox1.Text = Settings.PathToR;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string defaultRPath = @"C:\Program Files\R\R-2.14.0\bin\i386\R.exe";
            
            OpenFileDialog fd = new OpenFileDialog();
            string initialDir = @"C:\Program Files\R";
            if (Directory.Exists(initialDir))
            {
                fd.InitialDirectory = initialDir;
            }
            fd.Filter = "*.exe";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                Settings.PathToR = fd.FileName;
                textBox1.Text = Settings.PathToR;
            }
        }

        private void TextBoxR_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.ToLower().EndsWith("r.exe"))
            {
                Settings.PathToR = textBox1.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
           FolderBrowserDialog fd = new FolderBrowserDialog();

            if (fd.ShowDialog() == DialogResult.OK)
            {
                fd.ShowNewFolderButton = true;
                Settings.OutputDirectory = fd.SelectedPath;
                textBox1.Text = Settings.PathToR;
            }
        }

        private void TextBoxOutputFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(textBox1.Text))
            {
                Settings.OutputDirectory = textBox2.Text;
            }
        }
    }
}
