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

            listBoxStations.SelectedIndex = -1;

            TryGuessPathToR();

            TryCreateDefaultOutputFolder();
        }

        private void TryGuessPathToR()
        {
            try
            {
                string programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string baseRPath1 = "R\\R-{0}\\bin\\i386\\R.exe";
                string[] possibleRVersionsNew = new string[] { "2.14.0", "2.13.2", "2.13.1", "2.13.0", "2.12.2", "2.12.1", "2.12.0", };
                string baseRPathOld = "R\\R-{0}\\bin\\R.exe";
                string[] possibleRVersionsOld = new string[] { "2.11.1", "2.11.0", "2.10.1", "2.10.0" };
                foreach (string rVersion in possibleRVersionsNew)
                {
                    string rPath = Path.Combine(programFilesDir, string.Format(baseRPath1, rVersion));
                    if (File.Exists(rPath))
                    {
                        Settings.PathToR = rPath;
                        txtPathToR.Text = rPath;
                        return;
                    }
                }
                foreach (string rVersion in possibleRVersionsOld)
                {
                    string rPath = Path.Combine(programFilesDir, string.Format(baseRPathOld, rVersion));
                    if (File.Exists(rPath))
                    {
                        Settings.PathToR = rPath;
                        txtPathToR.Text = rPath;
                        return;
                    }
                }

            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
        }

        private void TryCreateDefaultOutputFolder()
        {
            try
            {
                string documentsDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string hydroDocsDir = Path.Combine(documentsDir, "HydroDesktop");
                if (!Directory.Exists(hydroDocsDir))
                {
                    Directory.CreateDirectory(hydroDocsDir);
                }
                if (Directory.Exists(hydroDocsDir))
                {
                    string droughtDir = Path.Combine(hydroDocsDir, "Drought");
                    if (!Directory.Exists(droughtDir))
                    {
                        Directory.CreateDirectory(droughtDir);
                    }

                    if (Directory.Exists(droughtDir))
                    {
                        Settings.OutputDirectory = droughtDir;
                        txtOutputFolder.Text = Settings.OutputDirectory;
                    }
                }

                

            }
            catch (UnauthorizedAccessException)
            {
                return;
            }
            catch (IOException)
            {
                return;
            }

        }


        private void btnPathToR_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog fd = new OpenFileDialog();
            string initialDir = @"C:\Program Files\R";
            if (Directory.Exists(initialDir))
            {
                fd.InitialDirectory = initialDir;
            }
            fd.Filter = "R.exe file|*.exe";

            if (fd.ShowDialog() == DialogResult.OK)
            {
                Settings.PathToR = fd.FileName;
                txtPathToR.Text = Settings.PathToR;
            }
        }

        private void TextBoxR_TextChanged(object sender, EventArgs e)
        {
            if (txtPathToR.Text.ToLower().EndsWith("r.exe"))
            {
                Settings.PathToR = txtPathToR.Text;
            }
        }

        private void btnOutputFolder_Click(object sender, EventArgs e)
        {
           FolderBrowserDialog fd = new FolderBrowserDialog();
           if (Settings.OutputDirectory != null)
               if (Directory.Exists(Settings.OutputDirectory))
                   fd.SelectedPath = Settings.OutputDirectory;

            if (fd.ShowDialog() == DialogResult.OK)
            {
                fd.ShowNewFolderButton = true;
                Settings.OutputDirectory = fd.SelectedPath;
                txtOutputFolder.Text = Settings.OutputDirectory;
            }
        }

        private void TextBoxOutputFolder_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txtOutputFolder.Text))
            {
                Settings.OutputDirectory = txtOutputFolder.Text;
            }
        }

        //analyze drought button
        private void button3_Click(object sender, EventArgs e)
        {
            if (Settings.SelectedSite == null)
            {
                MessageBox.Show("Please select a station.");
                return;
            }
            if (!Directory.Exists(Settings.OutputDirectory))
            {
                MessageBox.Show("Please specify a valid output directory.");
                return;
            }

            string dataFile = Path.Combine(Settings.OutputDirectory, "meteo.dat");

            //process the selected site
            DataExporter exp = new DataExporter();
            exp.ExportDataForStation(Settings.SelectedSite, dataFile);
            MessageBox.Show("Created data file: " + dataFile + ". Please run the R-script using this file.");
        }

        private void Stations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxStations.SelectedIndex < 0) Settings.SelectedSite = null;
            Settings.SelectedSite = (Site)listBoxStations.SelectedItem;
        }
    }
}
